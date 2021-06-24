' Copyright (c) Microsoft Corporation.  All Rights Reserved.

Imports System

Imports System.Collections.ObjectModel

Imports System.IdentityModel.Tokens

Imports System.Security.Cryptography
Imports System.Security.Cryptography.X509Certificates

Imports System.ServiceModel
Imports System.ServiceModel.Channels
Imports System.ServiceModel.Security.Tokens

Imports System.Xml

Imports Microsoft.VisualBasic

Namespace Microsoft.Samples.Federation

    ''' <summary>
    ''' Abstract base class for STS implementations
    ''' </summary>
    Public MustInherit Class SecurityTokenService
        Implements ISecurityTokenService

        Private stsName As String ' The name of the STS. Used to populate saml:Assertion/@Issuer
        Private m_issuerToken As SecurityToken ' The SecurityToken used to sign issued tokens
        Private m_proofKeyEncryptionToken As SecurityToken ' The SecurityToken used to encrypt the proof key in the issued token.

        ''' <summary>
        ''' constructor 
        ''' </summary>
        ''' <param name="stsName">The name of the STS. Used to populate saml:Assertion/@Issuer</param>
        ''' <param name="token">The X509SecurityToken that the STS uses to sign SAML assertions</param>
        ''' <param name="targetServiceName">The X509SecurityToken that is used to encrypt the proof key in the SAML token.</param>
        Protected Sub New(ByVal stsName As String, ByVal issuerToken As X509SecurityToken, ByVal encryptionToken As X509SecurityToken)

            Me.stsName = stsName
            Me.m_issuerToken = issuerToken
            Me.m_proofKeyEncryptionToken = encryptionToken

        End Sub

        ''' <summary>
        ''' The name of the STS.
        ''' </summary>
        Protected ReadOnly Property SecurityTokenServiceName() As String

            Get

                Return Me.stsName

            End Get

        End Property

        ''' <summary>
        ''' The SecurityToken used to sign tokens the STS issues.
        ''' </summary>
        Protected ReadOnly Property IssuerToken() As SecurityToken

            Get

                Return Me.m_issuerToken

            End Get

        End Property

        ''' <summary>
        ''' The SecurityToken used to encrypt the proof key in the issued token.
        ''' </summary>
        Protected ReadOnly Property ProofKeyEncryptionToken() As SecurityToken

            Get

                Return Me.m_proofKeyEncryptionToken

            End Get

        End Property

#Region "Abstract methods"

        ''' <summary>
        ''' abstract method for setting up claims in the SAML Token issued by the STS
        ''' Should be overridden by STS implementations that derive from this base class
        ''' to set up appropriate claims
        ''' </summary>
        Protected MustOverride Function GetIssuedClaims(ByVal requestSecurityToken As RequestSecurityToken) As Collection(Of SamlAttribute)

#End Region

#Region "Helper Methods"
        ''' <summary>
        ''' Validate action header and discard messages with inappropriate headers
        ''' </summary>
        Protected Shared Sub EnsureRequestSecurityTokenAction(ByVal message As Message)

            If message Is Nothing Then
                Throw New ArgumentNullException("message")
            End If

            If message.Headers.Action <> Constants.Trust.Actions.Issue Then
                Throw New InvalidOperationException([String].Format("Bad or Unsupported Action: {0}", message.Headers.Action))
            End If

        End Sub

        ''' <summary>
        ''' Helper Method to Create Proof Token. Creates BinarySecretSecuryToken 
        ''' with the requested number of bits of random key material
        ''' </summary>
        ''' <param name="keySize">keySize</param>
        ''' <returns>Proof Token</returns>
        Protected Shared Function CreateProofToken(ByVal keySize As Integer) As BinarySecretSecurityToken

            ' Create an array to store the key bytes
            Dim key() As Byte = New Byte((keySize / 8) - 1) {}
            ' Create some random bytes
            Dim random As New RNGCryptoServiceProvider()
            random.GetNonZeroBytes(key)
            ' Create a BinarySecretSecurityToken from the random bytes and return it
            Return New BinarySecretSecurityToken(key)

        End Function

        ''' <summary>
        ''' Helper Method to set up the RSTR
        ''' </summary>
        ''' <param name="rst">RequestSecurityToken</param>
        ''' <param name="keySize">keySize</param>
        ''' <param name="proofToken">proofToken</param>
        ''' <param name="samlToken">The SAML Token to be issued</param>
        ''' <returns>RequestSecurityTokenResponse</returns>
        Protected Shared Function GetRequestSecurityTokenResponse(ByVal requestSecurityToken As RequestSecurityToken, ByVal keySize As Integer, ByVal proofToken As SecurityToken, ByVal samlToken As SecurityToken, ByVal senderEntropy() As Byte, ByVal stsEntropy() As Byte) As RequestSecurityTokenResponse

            ' Create an uninitialized RequestSecurityTokenResponse object and set the various properties
            Dim rstr As New RequestSecurityTokenResponse()
            rstr.TokenType = Constants.SamlTokenTypeUri
            rstr.RequestedSecurityToken = samlToken
            rstr.RequestedUnattachedReference = samlToken.CreateKeyIdentifierClause(Of SamlAssertionKeyIdentifierClause)()
            rstr.RequestedAttachedReference = samlToken.CreateKeyIdentifierClause(Of SamlAssertionKeyIdentifierClause)()
            rstr.Context = requestSecurityToken.Context
            rstr.KeySize = keySize

            ' If sender provided entropy then use combined entropy so set the IssuerEntropy
            If senderEntropy IsNot Nothing Then

                rstr.IssuerEntropy = New BinarySecretSecurityToken(stsEntropy)
                rstr.ComputeKey = True

            Else

                ' Issuer entropy only...
                rstr.RequestedProofToken = proofToken

            End If

            Return rstr

        End Function
#End Region

        ''' <summary>
        ''' Virtual method for ProcessRequestSecurityToken
        ''' Should be overridden by STS implementations that derive from this base class
        ''' </summary>
        Public Overridable Function ProcessRequestSecurityToken(ByVal msg As Message) As Message Implements ISecurityTokenService.ProcessRequestSecurityToken

            ' Check for appropriate action header
            EnsureRequestSecurityTokenAction(msg)

            ' Extract the MessageID from the request message
            Dim requestMessageID As UniqueId = msg.Headers.MessageId
            If requestMessageID Is Nothing Then
                Throw New InvalidOperationException("The request message does not have a message ID.")
            End If

            ' Get the RST from the message
            Dim rst As RequestSecurityToken = RequestSecurityToken.CreateFrom(msg.GetReaderAtBodyContents())

            ' Set up the claims we are going to issue
            Dim samlAttributes As Collection(Of SamlAttribute) = GetIssuedClaims(rst)

            ' get the key size, default to 192
            Dim keySize As Integer = IIf((rst.KeySize <> 0), rst.KeySize, 192)

            ' Create proof token
            ' Get requester entropy, if any
            Dim senderEntropy() As Byte = Nothing
            Dim entropyToken As SecurityToken = rst.RequestorEntropy
            If entropyToken IsNot Nothing Then

                senderEntropy = (DirectCast(entropyToken, BinarySecretSecurityToken)).GetKeyBytes()

            End If

            Dim key() As Byte = Nothing
            Dim stsEntropy() As Byte = Nothing

            ' If sender provided entropy, then use combined entropy
            If senderEntropy IsNot Nothing Then

                ' Create an array to store the entropy bytes
                stsEntropy = New Byte((keySize \ 8) - 1) {}
                ' Create some random bytes
                Dim random As New RNGCryptoServiceProvider()
                random.GetNonZeroBytes(stsEntropy)
                ' Compute the combined key
                key = RequestSecurityTokenResponse.ComputeCombinedKey(senderEntropy, stsEntropy, keySize)

            Else

                ' Issuer entropy only...
                ' Create an array to store the entropy bytes
                key = New Byte((keySize \ 8) - 1) {}
                ' Create some random bytes
                Dim random As New RNGCryptoServiceProvider()
                random.GetNonZeroBytes(key)

            End If

            ' Create a BinarySecretSecurityToken to be the proof token, based on the key material
            ' in key. The key is the combined key in the combined entropy case, or the issuer entropy
            ' otherwise
            Dim proofToken As New BinarySecretSecurityToken(key)

            ' Create the saml condition
            Dim samlConditions As New SamlConditions(DateTime.UtcNow - TimeSpan.FromMinutes(5), DateTime.UtcNow + TimeSpan.FromHours(10))
            AddAudienceRestrictionCondition(samlConditions)

            ' Create a SAML token, valid for around 10 hours
            Dim samlToken As SamlSecurityToken = SamlTokenCreator.CreateSamlToken(Me.stsName, proofToken, Me.IssuerToken, Me.ProofKeyEncryptionToken, samlConditions, samlAttributes)

            ' Set up RSTR
            Dim rstr As RequestSecurityTokenResponse = GetRequestSecurityTokenResponse(rst, keySize, proofToken, samlToken, senderEntropy, stsEntropy)

            ' Create a message from the RSTR
            Dim rstrMessage As Message = Message.CreateMessage(msg.Version, Constants.Trust.Actions.IssueReply, rstr)

            ' Set RelatesTo of response message to MessageID of request message
            rstrMessage.Headers.RelatesTo = requestMessageID

            ' Return the create message
            Return rstrMessage

        End Function

        ''' <summary>
        ''' This method adds the audience uri restriction condition to the SAML assetion.
        ''' </summary>
        ''' <param name="samlConditions">The saml condition collection where the audience uri restriction condition will be added.</param>
        Public MustOverride Sub AddAudienceRestrictionCondition(ByVal samlConditions As SamlConditions)

    End Class

End Namespace

