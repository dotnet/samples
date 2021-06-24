' Copyright (c) Microsoft Corporation. All rights reserved.

Imports System

Imports Microsoft.VisualBasic

Imports System.Collections.Generic

Imports System.IdentityModel.Claims
Imports System.IdentityModel.Tokens

Imports System.Security.Cryptography
Imports System.Security.Cryptography.X509Certificates

Imports System.ServiceModel
Imports System.ServiceModel.Channels
Imports System.ServiceModel.Security
Imports System.ServiceModel.Security.Tokens

Imports System.Xml

Namespace Microsoft.Samples.WS2007FederationHttpBinding
    Class SecurityTokenService
        Implements IWSTrust13
        Private issuer As String
        Private issuerToken As X509SecurityToken

        ' Public constructors
        Public Sub New()
            Me.New("STS")
        End Sub

        Public Sub New(ByVal issuerName As String)
            issuer = issuerName
            issuerToken = GetToken(issuerName, StoreName.My, StoreLocation.LocalMachine)
        End Sub

        ''' <summary>
        ''' Finds a certificate and returns an X509SecurityToken for that certificate
        ''' </summary>
        ''' <param name="subjectName">The subject name of the certificate to use as the basis for the security token</param>
        ''' <returns>An X509SecurityToken</returns>
        Private Shared Function GetToken(ByVal subjectName As String, ByVal storeName As StoreName, ByVal storeLocation As StoreLocation) As X509SecurityToken
            ' Open the CurrentUser/TrustedPeople store
            Dim store As New X509Store(storeName, storeLocation)
            store.Open(OpenFlags.[ReadOnly])
            Try
                ' Find the certificates that match the Subject Name
                Dim matches As X509Certificate2Collection = store.Certificates.Find(X509FindType.FindBySubjectDistinguishedName, "CN=" + subjectName, False)
                If matches Is Nothing OrElse matches.Count = 0 Then
                    Throw New ArgumentException([String].Format("No certificates with subjectName {0} were found in CurrentUser\TrustedPeople store", subjectName))
                End If
                If matches.Count > 1 Then
                    Throw New ArgumentException([String].Format("Multiple certificates with subjectName {0} were found in CurrentUser\TrustedPeople store", subjectName))
                End If
                Return New X509SecurityToken(matches(0))
            Finally
                store.Close()
            End Try
        End Function

#Region "IWSTrust13 Members"

        Public Function Issue(ByVal request As Message) As Message Implements IWSTrust13.Issue
            Try
                Console.WriteLine("Call to IWSTrust13::Issue")

                ' if request is null, we're toast
                If request Is Nothing Then
                    Throw New ArgumentNullException("request")
                End If

                ' Create an RST object from the request message
                Dim rst As RequestSecurityTokenWSTrust13 = RequestSecurityTokenWSTrust13.CreateFrom(request.GetReaderAtBodyContents())

                ' Check that it really is an Issue request
                If rst.RequestType Is Nothing OrElse rst.RequestType <> Constants.Trust13.RequestTypes.Issue Then
                    Throw New InvalidOperationException(rst.RequestType)
                End If

                ' Create an RSTR object
                Dim rstr As RequestSecurityTokenResponseWSTrust13 = Issue(rst)

                ' Create response message
                Dim response As Message = Message.CreateMessage(request.Version, Constants.Trust13.Actions.IssueReply, rstr)

                ' Set RelatesTo of response to message id of request
                response.Headers.RelatesTo = request.Headers.MessageId

                ' Address response to ReplyTo of request
                request.Headers.ReplyTo.ApplyTo(response)
                Return response
            Catch e As Exception
                Console.WriteLine("**** Exception thrown while processing Issue request:")
                Console.WriteLine(e.Message)
                Throw
            End Try
        End Function

#End Region

        Private Function Issue(ByVal rst As RequestSecurityTokenWSTrust13) As RequestSecurityTokenResponseWSTrust13
            ' If rst is null, we're toast
            If rst Is Nothing Then
                Throw New ArgumentNullException("rst")
            End If

            ' Create an RSTR object
            Dim rstr As New RequestSecurityTokenResponseWSTrust13()

            Dim tokenType As String = rst.TokenType
            Console.WriteLine("Issue: Request for token type {0}", tokenType)
            If tokenType IsNot Nothing AndAlso tokenType <> "http://docs.oasis-open.org/wss/oasis-wss-saml-token-profile-1.1#SAMLV1.1" Then
                Throw New NotSupportedException("Unsupported token type " + tokenType)
            End If

            Dim signingKey As SecurityKey = issuerToken.SecurityKeys(0)
            Dim signingKeyIdentifier As New SecurityKeyIdentifier(issuerToken.CreateKeyIdentifierClause(Of X509ThumbprintKeyIdentifierClause)())
            Dim proofKeyIdentifier As SecurityKeyIdentifier = Nothing

            If rst.IsProofKeyAsymmetric() Then
                Throw New NotSupportedException("Public key issuance is not supported")
            End If
            ' Symmetric proof key
            Console.WriteLine("Constructing Symmetric Proof Key")

            ' Construct session key. This is the symmetric key that the client and the service will share. 
            ' It actually appears twice in the response message; once for the service and 
            ' once for the client. In the former case, it is typically embedded in the issued token, 
            ' in the latter case, it is returned in a wst:RequestedProofToken element.
            Dim sessionKey As Byte() = GetSessionKey(rst, rstr)

            ' Get token to use when encrypting key material for the service
            Dim encryptingToken As SecurityToken = DetermineEncryptingToken(rst)

            ' Encrypt the session key for the service
            GetEncryptedKey(encryptingToken, sessionKey, proofKeyIdentifier)

            ' Issued tokens are valid for 12 hours by default
            Dim effectiveTime As DateTime = DateTime.Now
            Dim expirationTime As DateTime = DateTime.Now + New TimeSpan(12, 0, 0)
            Dim samlToken As SecurityToken = CreateSAMLToken(effectiveTime, expirationTime, signingKey, signingKeyIdentifier, proofKeyIdentifier)

            rstr.RequestedSecurityToken = samlToken
            rstr.Context = rst.Context
            rstr.TokenType = tokenType
            Dim samlReference As SecurityKeyIdentifierClause = samlToken.CreateKeyIdentifierClause(Of SamlAssertionKeyIdentifierClause)()
            rstr.RequestedAttachedReference = samlReference
            rstr.RequestedUnattachedReference = samlReference
            Return rstr
        End Function

        ' The RST message can contain a wsp:AppliesTo which can be used to indicate the service that 
        ' the issued token is intended for. This method extracts the URI for that service, if any such
        ' Uri is present in the RST. Otherwise it returns null.
        Private Shared Function DetermineEndpointUri(ByVal rst As RequestSecurityTokenWSTrust13) As Uri
            ' If rst is null, we're toast
            If rst Is Nothing Then
                Throw New ArgumentNullException("rst")
            End If

            Dim epr As EndpointAddress = rst.AppliesTo

            ' If AppliesTo is missing or doesn't contain a 2004/08 or 
            ' 2005/10 EPR then the epr local variable will be null in which case we return null.
            ' Otherwise we return the Uri portion of the EPR.
            Return IIf(epr Is Nothing, Nothing, epr.Uri)
        End Function

#Region "Session key methods"
        Private Shared Function GetSenderEntropy(ByVal rst As RequestSecurityTokenWSTrust13) As Byte()
            ' If rst is null, we're toast
            If rst Is Nothing Then
                Throw New ArgumentNullException("rst")
            End If

            Dim senderEntropyToken As SecurityToken = rst.RequestorEntropy
            Dim senderEntropy As Byte() = Nothing

            If senderEntropyToken IsNot Nothing Then
                Dim bsst As BinarySecretSecurityToken = TryCast(senderEntropyToken, BinarySecretSecurityToken)

                If bsst IsNot Nothing Then
                    senderEntropy = bsst.GetKeyBytes()
                End If
            End If

            Return senderEntropy
        End Function

        Private Shared Function GetIssuerEntropy(ByVal keySize As Integer) As Byte()
            Dim random As New RNGCryptoServiceProvider()
            Dim issuerEntropy As Byte() = New Byte(keySize / 8 - 1) {}
            random.GetNonZeroBytes(issuerEntropy)
            Return issuerEntropy
        End Function

        Private Shared Function GetSessionKey(ByVal rst As RequestSecurityTokenWSTrust13, ByVal rstr As RequestSecurityTokenResponseWSTrust13) As Byte()
            ' If rst is null, we're toast
            If rst Is Nothing Then
                Throw New ArgumentNullException("rst")
            End If

            ' If rstr is null, we're toast
            If rstr Is Nothing Then
                Throw New ArgumentNullException("rstr")
            End If

            ' Figure out the keySize
            Dim keySize As Integer = 256

            If rst.KeySize <> 0 Then
                keySize = rst.KeySize
            End If

            Console.WriteLine("Proof key size {0}", keySize)

            ' Figure out whether we're using Combined or Issuer entropy.
            Dim sessionKey As Byte() = Nothing
            Dim senderEntropy As Byte() = GetSenderEntropy(rst)
            Dim issuerEntropy As Byte() = GetIssuerEntropy(keySize)

            If senderEntropy IsNot Nothing Then
                ' Combined entropy
                Console.WriteLine("Combined Entropy")
                sessionKey = RequestSecurityTokenResponseWSTrust13.ComputeCombinedKey(senderEntropy, issuerEntropy, keySize)
                rstr.IssuerEntropy = New BinarySecretSecurityToken(issuerEntropy)
                rstr.ComputeKey = True
            Else
                ' Issuer-only entropy
                Console.WriteLine("Issuer-only entropy")
                sessionKey = issuerEntropy
                rstr.RequestedProofToken = New BinarySecretSecurityToken(sessionKey)
            End If

            rstr.KeySize = keySize
            Return sessionKey
        End Function
#End Region

        ' This method determines the security token that contains the key material that 
        ' the STS should encrypt a session key with in order 
        ' for the service the issued token is intended for to be able to extract that session key
        Private Shared Function DetermineEncryptingToken(ByVal rst As RequestSecurityTokenWSTrust13) As SecurityToken
            ' If rst is null, we're toast
            If rst Is Nothing Then
                Throw New ArgumentNullException("rst")
            End If

            ' Figure out service URI
            Dim uri As Uri = DetermineEndpointUri(rst)

            Dim encryptingTokenSubjectName As String = IIf((uri Is Nothing), "localhost", uri.DnsSafeHost)
            Return GetToken(encryptingTokenSubjectName, StoreName.TrustedPeople, StoreLocation.LocalMachine)
        End Function


        ' This method returns a security token to be used as the requested proof token portion of the RSTR
        ' The key parameter is the proof key that will be shared by the client and the actual service
        Private Shared Function GetRequestedProofToken(ByVal key As Byte()) As SecurityToken
            ' If key is null, we're toast
            If key Is Nothing Then
                Throw New ArgumentNullException("key")
            End If
            Return New BinarySecretSecurityToken(key)
        End Function

        Private Shared Function GetKeyWrapAlgorithm(ByVal key As SecurityKey) As String
            ' If key is null, we're toast
            If key Is Nothing Then
                Throw New ArgumentNullException("key")
            End If

            ' Set keywrapAlgorithm to null
            Dim keywrapAlgorithm As String = Nothing

            ' If the security key supports RsaOaep then use that ...
            If key.IsSupportedAlgorithm(SecurityAlgorithms.RsaOaepKeyWrap) Then
                keywrapAlgorithm = SecurityAlgorithms.RsaOaepKeyWrap
                ' ... otherwise if it supports RSA15 use that ...
            ElseIf key.IsSupportedAlgorithm(SecurityAlgorithms.RsaV15KeyWrap) Then
                keywrapAlgorithm = SecurityAlgorithms.RsaV15KeyWrap
                ' ... otherwise if it supports AES256 use that ...
            ElseIf key.IsSupportedAlgorithm(SecurityAlgorithms.Aes256KeyWrap) Then
                keywrapAlgorithm = SecurityAlgorithms.Aes256KeyWrap
                ' ... otherwise if it supports AES192 use that ...
            ElseIf key.IsSupportedAlgorithm(SecurityAlgorithms.Aes192KeyWrap) Then
                keywrapAlgorithm = SecurityAlgorithms.Aes192KeyWrap
                ' ... otherwise if it supports AES128 use that ...
            ElseIf key.IsSupportedAlgorithm(SecurityAlgorithms.Aes128KeyWrap) Then
                keywrapAlgorithm = SecurityAlgorithms.Aes128KeyWrap
            End If

            Return keywrapAlgorithm
        End Function

        Private Shared Function GetSignatureAlgorithm(ByVal key As SecurityKey) As String
            ' If key is null, we're toast
            If key Is Nothing Then
                Throw New ArgumentNullException("key")
            End If

            ' Set signatureAlgorithm to null
            Dim signatureAlgorithm As String = Nothing

            ' If the security key supports RsaSha1 then use that ...
            If key.IsSupportedAlgorithm(SecurityAlgorithms.RsaSha1Signature) Then
                signatureAlgorithm = SecurityAlgorithms.RsaSha1Signature
                ' ... otherwise if it supports HMACSha1 use that ...
            ElseIf key.IsSupportedAlgorithm(SecurityAlgorithms.HmacSha1Signature) Then
                signatureAlgorithm = SecurityAlgorithms.HmacSha1Signature
            End If

            Return signatureAlgorithm
        End Function

        Private Shared Function GetEncryptionAlgorithm(ByVal key As SecurityKey) As String
            ' If key is null, we're toast
            If key Is Nothing Then
                Throw New ArgumentNullException("key")
            End If

            ' Set encryptionAlgorithm to null
            Dim encryptionAlgorithm As String = Nothing

            ' If the security key supports AES256 use that ...
            If key.IsSupportedAlgorithm(SecurityAlgorithms.Aes256Encryption) Then
                encryptionAlgorithm = SecurityAlgorithms.Aes256Encryption
                ' ... otherwise if it supports AES192 use that ...
            ElseIf key.IsSupportedAlgorithm(SecurityAlgorithms.Aes192Encryption) Then
                encryptionAlgorithm = SecurityAlgorithms.Aes192Encryption
                ' ... otherwise if it supports AES128 use that ...
            ElseIf key.IsSupportedAlgorithm(SecurityAlgorithms.Aes128Encryption) Then
                encryptionAlgorithm = SecurityAlgorithms.Aes128Encryption
            End If

            Return encryptionAlgorithm
        End Function

        ' This method encrypts the provided key using the key material associated with the cert
        ' returned by DetermineEncryptingCert
        Private Shared Function GetEncryptedKey(ByVal encryptingToken As SecurityToken, ByVal key As Byte(), ByRef ski As SecurityKeyIdentifier) As Byte()
            ' If encryptingToken is null, we're toast
            If encryptingToken Is Nothing Then
                Throw New ArgumentNullException("encryptingToken")
            End If

            ' If key is null, we're toast
            If key Is Nothing Then
                Throw New ArgumentNullException("key")
            End If

            ' Get the zeroth security key
            Dim encryptingKey As SecurityKey = encryptingToken.SecurityKeys(0)

            ' Get the encryption algorithm to use
            Dim keywrapAlgorithm As String = GetKeyWrapAlgorithm(encryptingKey)

            ' encrypt the passed in key 
            Dim encryptedKey As Byte() = encryptingKey.EncryptKey(keywrapAlgorithm, key)

            ' get a key identifier for the encrypting key
            Dim eki As New SecurityKeyIdentifier(encryptingToken.CreateKeyIdentifierClause(Of X509ThumbprintKeyIdentifierClause)())

            ' return the proof key identifier
            ski = GetProofKeyIdentifier(encryptedKey, keywrapAlgorithm, eki)

            ' return the encrypted key
            Return encryptedKey
        End Function

        Private Shared Function GetProofKeyIdentifier(ByVal key As Byte(), ByVal algorithm As String, ByVal eki As SecurityKeyIdentifier) As SecurityKeyIdentifier
            ' If key is null, we're toast
            If key Is Nothing Then
                Throw New ArgumentNullException("key")
            End If

            ' Create list of SecurityKeyIdentifierClauses
            Dim skics As New List(Of SecurityKeyIdentifierClause)()
            skics.Add(New EncryptedKeyIdentifierClause(key, algorithm, eki))
            Return New SecurityKeyIdentifier(skics.ToArray())
        End Function

        Private Function CreateSAMLToken(ByVal validFrom As DateTime, ByVal validTo As DateTime, ByVal signingKey As SecurityKey, ByVal signingKeyIdentifier As SecurityKeyIdentifier, ByVal proofKeyIdentifier As SecurityKeyIdentifier) As SecurityToken
            ' Create list of confirmation strings
            Dim confirmations As New List(Of String)()

            ' Add holder-of-key string to list of confirmation strings
            confirmations.Add("urn:oasis:names:tc:SAML:1.0:cm:holder-of-key")

            ' Create SAML subject statement based on issuer member variable, confirmation string collection 
            ' local variable and proof key identifier parameter
            Dim subject As New SamlSubject(Nothing, Nothing, issuer, confirmations, Nothing, proofKeyIdentifier)

            ' Create a list of SAML attributes
            Dim attributes As New List(Of SamlAttribute)()

            ' Get the claimset we want to place into the SAML assertion
            Dim cs As ClaimSet = GetClaimSet()

            ' Iterate through the claims and add a SamlAttribute for each claim
            ' Note that GetClaimSet call above returns a claimset that only contains PossessProperty claims
            For Each c As Claim In cs
                attributes.Add(New SamlAttribute(c))
            Next

            ' Create list of SAML statements
            Dim statements As New List(Of SamlStatement)()

            ' Add a SAML attribute statement to the list of statements. Attribute statement is based on 
            ' subject statement and SAML attributes resulting from claims
            statements.Add(New SamlAttributeStatement(subject, attributes))

            ' Create a valid from/until condition
            Dim conditions As New SamlConditions(validFrom, validTo)

            ' Create the SAML assertion
            Dim assertion As New SamlAssertion("_" + Guid.NewGuid().ToString(), issuer, validFrom, conditions, Nothing, statements)

            ' Set the signing credentials for the SAML assertion
            Dim signatureAlgorithm As String = GetSignatureAlgorithm(signingKey)
            assertion.SigningCredentials = New SigningCredentials(signingKey, signatureAlgorithm, SecurityAlgorithms.Sha1Digest, signingKeyIdentifier)

            Return New SamlSecurityToken(assertion)
        End Function


        ' Return a ClaimSet to be serialized into an issued SAML token
        Private Shared Function GetClaimSet() As ClaimSet
            ' Create an empty list of claims
            Dim claims As New List(Of Claim)()

            ' Iterate through all the ClaimSets in the current AuthorizationContext
            For Each cs As ClaimSet In OperationContext.Current.ServiceSecurityContext.AuthorizationContext.ClaimSets
                Dim nameClaims As IEnumerable(Of Claim) = cs.FindClaims(ClaimTypes.Name, Rights.PossessProperty)
                If nameClaims IsNot Nothing Then
                    For Each claim As Claim In nameClaims
                        claims.Add(claim)
                    Next
                End If
            Next

            ' Create a new ClaimSet based on the claims list and return that ClaimSet
            Return New DefaultClaimSet(DefaultClaimSet.System, claims)
        End Function
    End Class
End Namespace
