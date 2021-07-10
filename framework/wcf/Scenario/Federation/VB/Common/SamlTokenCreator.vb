' Copyright (c) Microsoft Corporation.  All Rights Reserved.

Imports System

Imports System.Collections.Generic
Imports System.Collections.ObjectModel

Imports System.IdentityModel.Tokens

Imports System.ServiceModel
Imports System.ServiceModel.Security.Tokens

Namespace Microsoft.Samples.Federation

    Public Module SamlTokenCreator
#Region "CreateSamlToken()"
        ''' <summary>
        ''' Creates a SAML Token with the input parameters
        ''' </summary>
        ''' <param name="stsName">Name of the STS issuing the SAML Token</param>
        ''' <param name="proofToken">Associated Proof Token</param>
        ''' <param name="issuerToken">Associated Issuer Token</param>
        ''' <param name="proofKeyEncryptionToken">Token to encrypt the proof key with</param>
        ''' <param name="samlConditions">The Saml Conditions to be used in the construction of the SAML Token</param>
        ''' <param name="samlAttributes">The Saml Attributes to be used in the construction of the SAML Token</param>
        ''' <returns>A SAML Token</returns>
        Public Function CreateSamlToken(ByVal stsName As String, ByVal proofToken As BinarySecretSecurityToken, ByVal issuerToken As SecurityToken, ByVal proofKeyEncryptionToken As SecurityToken, ByVal samlConditions As SamlConditions, ByVal samlAttributes As IEnumerable(Of SamlAttribute)) As SamlSecurityToken

            ' Create a security token reference to the issuer certificate 
            Dim skic As SecurityKeyIdentifierClause = issuerToken.CreateKeyIdentifierClause(Of X509ThumbprintKeyIdentifierClause)()
            Dim issuerKeyIdentifier As New SecurityKeyIdentifier(skic)

            ' Create an encrypted key clause containing the encrypted proof key
            Dim wrappedKey() As Byte = proofKeyEncryptionToken.SecurityKeys(0).EncryptKey(SecurityAlgorithms.RsaOaepKeyWrap, proofToken.GetKeyBytes())
            Dim encryptingTokenClause As SecurityKeyIdentifierClause = proofKeyEncryptionToken.CreateKeyIdentifierClause(Of X509ThumbprintKeyIdentifierClause)()
            Dim encryptedKeyClause As New EncryptedKeyIdentifierClause(wrappedKey, SecurityAlgorithms.RsaOaepKeyWrap, New SecurityKeyIdentifier(encryptingTokenClause))
            Dim proofKeyIdentifier As New SecurityKeyIdentifier(encryptedKeyClause)

            ' Create a comfirmationMethod for HolderOfKey
            Dim confirmationMethods As New List(Of String)(1)
            confirmationMethods.Add(SamlConstants.HolderOfKey)

            ' Create a SamlSubject with proof key and confirmation method from above
            Dim samlSubject As New SamlSubject(Nothing, Nothing, Nothing, confirmationMethods, Nothing, proofKeyIdentifier)

            ' Create a SamlAttributeStatement from the passed in SamlAttribute collection and the SamlSubject from above
            Dim samlAttributeStatement As New SamlAttributeStatement(samlSubject, samlAttributes)

            ' Put the SamlAttributeStatement into a list of SamlStatements
            Dim samlSubjectStatements As New List(Of SamlStatement)()
            samlSubjectStatements.Add(samlAttributeStatement)

            ' Create a SigningCredentials instance from the key associated with the issuerToken.
            Dim signingCredentials As New SigningCredentials(issuerToken.SecurityKeys(0), SecurityAlgorithms.RsaSha1Signature, SecurityAlgorithms.Sha1Digest, issuerKeyIdentifier)

            ' Create a SamlAssertion from the list of SamlStatements created above and the passed in
            ' SamlConditions.
            Dim samlAssertion As New SamlAssertion("_" + Guid.NewGuid().ToString(), stsName, DateTime.UtcNow, samlConditions, New SamlAdvice(), samlSubjectStatements)

            ' Set the SigningCredentials for the SamlAssertion
            samlAssertion.SigningCredentials = signingCredentials

            ' Create a SamlSecurityToken from the SamlAssertion and return it
            Return New SamlSecurityToken(samlAssertion)

        End Function
#End Region

    End Module

End Namespace

