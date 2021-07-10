' Copyright (c) Microsoft Corporation. All rights reserved.

Imports System

Imports System.IdentityModel.Selectors
Imports System.IdentityModel.Tokens

Imports System.Security.Cryptography

Imports System.ServiceModel
Imports System.ServiceModel.Channels
Imports System.ServiceModel.Security
Imports System.ServiceModel.Security.Tokens

Imports System.Xml

Namespace Microsoft.Samples.WS2007FederationHttpBinding
    Public Class RequestSecurityTokenResponseWSTrust13
        Inherits RequestSecurityTokenBase
        ' private members
        Private m_requestedSecurityToken As SecurityToken
        Private m_requestedProofToken As SecurityToken
        Private m_issuerEntropy As SecurityToken
        Private m_requestedAttachedReference As SecurityKeyIdentifierClause
        Private m_requestedUnattachedReference As SecurityKeyIdentifierClause
        Private m_computeKey As Boolean

        ' Constructors
        Public Sub New()
            Me.New([String].Empty, [String].Empty, 0, Nothing, Nothing, Nothing, _
             False)
        End Sub

        Public Sub New(ByVal context As String, ByVal tokenType As String, ByVal keySize As Integer, ByVal appliesTo As EndpointAddress, ByVal requestedSecurityToken As SecurityToken, ByVal requestedProofToken As SecurityToken, _
         ByVal computeKey As Boolean)
            MyBase.New(context, tokenType, keySize, appliesTo)
            Me.m_requestedSecurityToken = requestedSecurityToken
            Me.m_requestedProofToken = requestedProofToken
            Me.m_computeKey = computeKey
        End Sub

        ' public properties
        Public Property RequestedSecurityToken() As SecurityToken
            Get
                Return m_requestedSecurityToken
            End Get
            Set(ByVal value As SecurityToken)
                m_requestedSecurityToken = value
            End Set
        End Property

        Public Property RequestedProofToken() As SecurityToken
            Get
                Return m_requestedProofToken
            End Get
            Set(ByVal value As SecurityToken)
                m_requestedProofToken = value
            End Set
        End Property

        Public Property RequestedAttachedReference() As SecurityKeyIdentifierClause
            Get
                Return m_requestedAttachedReference
            End Get
            Set(ByVal value As SecurityKeyIdentifierClause)
                m_requestedAttachedReference = value
            End Set
        End Property

        Public Property RequestedUnattachedReference() As SecurityKeyIdentifierClause
            Get
                Return m_requestedUnattachedReference
            End Get
            Set(ByVal value As SecurityKeyIdentifierClause)
                m_requestedUnattachedReference = value
            End Set
        End Property

        Public Property IssuerEntropy() As SecurityToken
            Get
                Return m_issuerEntropy
            End Get
            Set(ByVal value As SecurityToken)
                m_issuerEntropy = value
            End Set
        End Property

        Public Property ComputeKey() As Boolean
            Get
                Return m_computeKey
            End Get
            Set(ByVal value As Boolean)
                m_computeKey = value
            End Set
        End Property

        ' public methods
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="requestorEntropy"></param>
        ''' <param name="issuerEntropy"></param>
        ''' <param name="keySize"></param>
        ''' <returns></returns>
        Public Shared Function ComputeCombinedKey(ByVal requestorEntropy As Byte(), ByVal issuerEntropy As Byte(), ByVal keySize As Integer) As Byte()
            If keySize < 64 OrElse keySize > 4096 Then
                Throw New ArgumentOutOfRangeException("keySize")
            End If

            Dim kha As KeyedHashAlgorithm = New HMACSHA1(requestorEntropy, True)

            Dim key As Byte() = New Byte(keySize / 8 - 1) {} ' Final key
            Dim a As Byte() = issuerEntropy ' A(0)
            Dim b As Byte() = New Byte(kha.HashSize / 8 + a.Length - 1) {} ' Buffer for A(i) + seed
            Dim i As Integer = 0
            While i < key.Length
                ' Calculate A(i+1).                
                kha.Initialize()
                a = kha.ComputeHash(a)

                ' Calculate A(i) + seed
                a.CopyTo(b, 0)
                issuerEntropy.CopyTo(b, a.Length)
                kha.Initialize()
                Dim result As Byte() = kha.ComputeHash(b)
                For j As Integer = 0 To result.Length - 1

                    If i < key.Length Then
                        key(i) = result(j)
                        i = i + 1
                    Else
                        Exit For
                    End If
                Next
            End While

            Return key
        End Function

        ' Methods of BodyWriter
        Protected Overloads Overrides Sub OnWriteBodyContents(ByVal writer As XmlDictionaryWriter)
            writer.WriteStartElement(Constants.Trust13.Elements.RequestSecurityTokenResponseCollection, Constants.Trust13.NamespaceUri)
            writer.WriteStartElement(Constants.Trust13.Elements.RequestSecurityTokenResponse, Constants.Trust13.NamespaceUri)

            If Me.TokenType IsNot Nothing AndAlso Me.TokenType.Length > 0 Then
                writer.WriteStartElement(Constants.Trust13.Elements.TokenType, Constants.Trust13.NamespaceUri)
                writer.WriteString(Me.TokenType)
                writer.WriteEndElement() ' wst:TokenType
            End If

            Dim ser As New WSSecurityTokenSerializer(SecurityVersion.WSSecurity11, TrustVersion.WSTrust13, SecureConversationVersion.WSSecureConversation13, False, Nothing, Nothing, Nothing)

            If Me.RequestedSecurityToken IsNot Nothing Then
                writer.WriteStartElement(Constants.Trust13.Elements.RequestedSecurityToken, Constants.Trust13.NamespaceUri)
                ser.WriteToken(writer, Me.RequestedSecurityToken)
                writer.WriteEndElement() ' wst:RequestedSecurityToken
            End If

            If Me.RequestedAttachedReference IsNot Nothing Then
                writer.WriteStartElement(Constants.Trust13.Elements.RequestedAttachedReference, Constants.Trust13.NamespaceUri)
                ser.WriteKeyIdentifierClause(writer, Me.RequestedAttachedReference)
                writer.WriteEndElement() ' wst:RequestedAttachedReference
            End If

            If Me.RequestedUnattachedReference IsNot Nothing Then
                writer.WriteStartElement(Constants.Trust13.Elements.RequestedUnattachedReference, Constants.Trust13.NamespaceUri)
                ser.WriteKeyIdentifierClause(writer, Me.RequestedUnattachedReference)
                writer.WriteEndElement() ' wst:RequestedAttachedReference
            End If

            If Me.AppliesTo IsNot Nothing Then
                writer.WriteStartElement(Constants.Policy.Elements.AppliesTo, Constants.Policy.NamespaceUri)
                Me.AppliesTo.WriteTo(AddressingVersion.WSAddressing10, writer)
                writer.WriteEndElement() ' wsp:AppliesTo
            End If

            If Me.RequestedProofToken IsNot Nothing Then ' Issuer entropy; write RPT only
                writer.WriteStartElement(Constants.Trust13.Elements.RequestedProofToken, Constants.Trust13.NamespaceUri)
                ser.WriteToken(writer, Me.RequestedProofToken)
                writer.WriteEndElement() ' wst:RequestedSecurityToken
            End If

            If Me.IssuerEntropy IsNot Nothing AndAlso Me.ComputeKey Then ' Combined entropy; write RPT and Entropy

                writer.WriteStartElement(Constants.Trust13.Elements.RequestedProofToken, Constants.Trust13.NamespaceUri)
                writer.WriteStartElement(Constants.Trust13.Elements.ComputedKey, Constants.Trust13.NamespaceUri)
                writer.WriteValue(Constants.Trust13.ComputedKeyAlgorithms.PSHA1)
                writer.WriteEndElement() ' wst:ComputedKey
                writer.WriteEndElement() ' wst:RequestedSecurityToken

                If Me.IssuerEntropy IsNot Nothing Then
                    writer.WriteStartElement(Constants.Trust13.Elements.Entropy, Constants.Trust13.NamespaceUri)
                    ser.WriteToken(writer, Me.IssuerEntropy)

                    writer.WriteEndElement() ' wst:Entropy
                End If
            End If

            writer.WriteEndElement() ' wst:RequestSecurityTokenResponse
            writer.WriteEndElement() ' wst:RequestSecurityTokenResponseCollection
        End Sub
    End Class
End Namespace
