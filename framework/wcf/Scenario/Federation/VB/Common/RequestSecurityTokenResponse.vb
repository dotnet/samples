' Copyright (c) Microsoft Corporation.  All Rights Reserved.

Imports System

Imports System.IdentityModel.Tokens

Imports System.Runtime.InteropServices

Imports System.Security.Cryptography

Imports System.ServiceModel
Imports System.ServiceModel.Channels
Imports System.ServiceModel.Security
Imports System.ServiceModel.Security.Tokens

Imports System.Xml

Namespace Microsoft.Samples.Federation

    ''' <summary>
    ''' A class that represents a RequestSecurityTokenResponse message according to February 2005 WS-Trust
    ''' </summary>
    <ComVisible(False)> _
    Public Class RequestSecurityTokenResponse
        Inherits RequestSecurityTokenBase

        ' private members
        Private m_requestedSecurityToken As SecurityToken
        Private m_requestedProofToken As SecurityToken
        Private m_issuerEntropy As SecurityToken
        Private m_requestedAttachedReference As SecurityKeyIdentifierClause
        Private m_requestedUnattachedReference As SecurityKeyIdentifierClause
        Private m_computeKey As Boolean

        ' Constructors
        ''' <summary>
        ''' Default constructor
        ''' </summary>
        Public Sub New()

            Me.New([String].Empty, [String].Empty, 0, Nothing, Nothing, Nothing, _
             False)

        End Sub

        ''' <summary>
        ''' Parameterized constructor
        ''' </summary>
        ''' <param name="context">The value of the wst:RequestSecurityTokenResponse/@Context attribute</param>
        ''' <param name="tokenType">The content of the wst:RequestSecurityTokenResponse/wst:TokenType element</param>
        ''' <param name="keySize">The content of the wst:RequestSecurityTokenResponse/wst:KeySize element </param>
        ''' <param name="appliesTo">An EndpointReference that corresponds to the content of the wst:RequestSecurityTokenResponse/wsp:AppliesTo element</param>
        ''' <param name="requestedSecurityToken">The requested security token</param>
        ''' <param name="requestedProofToken">The proof token associated with the requested security token</param>
        ''' <param name="computeKey">A boolean that specifies whether a key value must be computed</param>
        Public Sub New(ByVal context As String, ByVal tokenType As String, ByVal keySize As Integer, ByVal appliesTo As EndpointAddress, ByVal requestedSecurityToken As SecurityToken, ByVal requestedProofToken As SecurityToken, _
         ByVal computeKey As Boolean)

            MyBase.New(context, tokenType, keySize, appliesTo)
            ' Pass first 4 params to base class
            Me.m_requestedSecurityToken = requestedSecurityToken
            Me.m_requestedProofToken = requestedProofToken
            Me.m_computeKey = computeKey

        End Sub

        ' public properties
        ''' <summary>
        ''' The requested SecurityToken
        ''' </summary>
        Public Property RequestedSecurityToken() As SecurityToken

            Get

                Return m_requestedSecurityToken

            End Get
            Set(ByVal value As SecurityToken)

                m_requestedSecurityToken = value

            End Set

        End Property

        ''' <summary>
        ''' A SecurityToken that represents the proof token associated with 
        ''' the requested SecurityToken
        ''' </summary>
        Public Property RequestedProofToken() As SecurityToken

            Get

                Return m_requestedProofToken

            End Get
            Set(ByVal value As SecurityToken)

                m_requestedProofToken = value

            End Set

        End Property

        ''' <summary>
        ''' A SecurityKeyIdentifierClause that can be used to refer to the requested 
        ''' SecurityToken when that token is present in messages
        ''' </summary>
        Public Property RequestedAttachedReference() As SecurityKeyIdentifierClause

            Get

                Return m_requestedAttachedReference

            End Get
            Set(ByVal value As SecurityKeyIdentifierClause)

                m_requestedAttachedReference = value

            End Set

        End Property

        ''' <summary>
        ''' A SecurityKeyIdentifierClause that can be used to refer to the requested 
        ''' SecurityToken when that token is present in messages
        ''' </summary>
        Public Property RequestedUnattachedReference() As SecurityKeyIdentifierClause

            Get

                Return m_requestedUnattachedReference

            End Get
            Set(ByVal value As SecurityKeyIdentifierClause)

                m_requestedUnattachedReference = value

            End Set

        End Property

        ''' <summary>
        ''' The SecurityToken that represents entropy provided by the issuer
        ''' Null if the issuer did not provide entropy
        ''' </summary>
        Public Property IssuerEntropy() As SecurityToken

            Get

                Return m_issuerEntropy

            End Get
            Set(ByVal value As SecurityToken)

                m_issuerEntropy = value

            End Set

        End Property

        ''' <summary>
        ''' Indicates whether a key must be computed (typically from the combination of issuer and requester entropy)
        ''' </summary>
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
        ''' Static method that computes a combined key from issue and requester entropy using PSHA1 per WS-Trust
        ''' </summary>
        ''' <param name="requestorEntropy">Entropy provided by the requester</param>
        ''' <param name="issuerEntropy">Entropy provided by the issuer</param>
        ''' <param name="keySize">Size of required key, in bits</param>
        ''' <returns>Array of bytes that contains key material</returns>
        Public Shared Function ComputeCombinedKey(ByVal requestorEntropy() As Byte, ByVal issuerEntropy() As Byte, ByVal keySize As Integer) As Byte()

            Dim kha As KeyedHashAlgorithm = New HMACSHA1(requestorEntropy, True)

            Dim key() As Byte = New Byte((keySize \ 8) - 1) {} ' Final key
            Dim a() As Byte = issuerEntropy ' A(0)
            Dim b() As Byte = New Byte((kha.HashSize \ 8 + a.Length) - 1) {} ' Buffer for A(i) + seed

            Dim i As Integer = 0
            While i < key.Length

                ' Calculate A(i+1).                
                kha.Initialize()
                a = kha.ComputeHash(a)

                ' Calculate A(i) + seed
                a.CopyTo(b, 0)
                issuerEntropy.CopyTo(b, a.Length)
                kha.Initialize()
                Dim result() As Byte = kha.ComputeHash(b)

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
        ''' <summary>
        ''' Writes out an XML representation of the instance.
        ''' </summary>
        ''' <param name="writer">The writer to be used to write out the XML content</param>
        Protected Overloads Overrides Sub OnWriteBodyContents(ByVal writer As XmlDictionaryWriter)

            ' Write out the wst:RequestSecurityTokenResponse start tag
            writer.WriteStartElement(Constants.Trust.Elements.RequestSecurityTokenResponse, Constants.Trust.NamespaceUri)

            ' If we have a non-null, non-empty tokenType...
            If Me.TokenType IsNot Nothing AndAlso Me.TokenType.Length > 0 Then

                ' Write out the wst:TokenType start tag
                writer.WriteStartElement(Constants.Trust.Elements.TokenType, Constants.Trust.NamespaceUri)
                ' Write out the tokenType string
                writer.WriteString(Me.TokenType)
                ' wst:TokenType
                writer.WriteEndElement()

            End If

            ' Create a serializer that knows how to write out security tokens
            Dim ser As New WSSecurityTokenSerializer()

            ' If we have a requestedSecurityToken...
            If Me.m_requestedSecurityToken IsNot Nothing Then

                ' Write out the wst:RequestedSecurityToken start tag
                writer.WriteStartElement(Constants.Trust.Elements.RequestedSecurityToken, Constants.Trust.NamespaceUri)
                ' Write out the requested token using the serializer
                ser.WriteToken(writer, m_requestedSecurityToken)
                writer.WriteEndElement() ' wst:RequestedSecurityToken

            End If

            ' If we have a requestedAttachedReference...
            If Me.m_requestedAttachedReference IsNot Nothing Then

                ' Write out the wst:RequestedAttachedReference start tag
                writer.WriteStartElement(Constants.Trust.Elements.RequestedAttachedReference, Constants.Trust.NamespaceUri)
                ' Write out the reference using the serializer
                ser.WriteKeyIdentifierClause(writer, Me.m_requestedAttachedReference)
                writer.WriteEndElement() ' wst:RequestedAttachedReference
            End If

            ' If we have a requestedUnattachedReference...
            If Me.m_requestedUnattachedReference IsNot Nothing Then

                ' Write out the wst:RequestedUnattachedReference start tag
                writer.WriteStartElement(Constants.Trust.Elements.RequestedUnattachedReference, Constants.Trust.NamespaceUri)
                ' Write out the reference using the serializer
                ser.WriteKeyIdentifierClause(writer, Me.m_requestedUnattachedReference)
                writer.WriteEndElement() ' wst:RequestedAttachedReference

            End If

            ' If we have a non-null appliesTo
            If Me.AppliesTo IsNot Nothing Then

                ' Write out the wsp:AppliesTo start tag
                writer.WriteStartElement(Constants.Policy.Elements.AppliesTo, Constants.Policy.NamespaceUri)
                ' Write the appliesTo in WS-Addressing 1.0 format
                Me.AppliesTo.WriteTo(AddressingVersion.WSAddressing10, writer)
                writer.WriteEndElement() ' wsp:AppliesTo

            End If

            ' If the requestedProofToken is non-null, then the STS is providing all the key material...
            If Me.m_requestedProofToken IsNot Nothing Then

                ' Write the wst:RequestedProofToken start tag
                writer.WriteStartElement(Constants.Trust.Elements.RequestedProofToken, Constants.Trust.NamespaceUri)
                ' Write the proof token using the serializer
                ser.WriteToken(writer, m_requestedProofToken)
                writer.WriteEndElement() ' wst:RequestedSecurityToken

            End If

            ' If issuerEntropy is non-null and computeKey is true, then combined entropy is being used...
            If Me.m_issuerEntropy IsNot Nothing AndAlso Me.m_computeKey Then

                ' Write the wst:RequestedProofToken start tag
                writer.WriteStartElement(Constants.Trust.Elements.RequestedProofToken, Constants.Trust.NamespaceUri)
                ' Write the wst:ComputeKey start tag
                writer.WriteStartElement(Constants.Trust.Elements.ComputedKey, Constants.Trust.NamespaceUri)
                ' Write the PSHA1 algorithm value
                writer.WriteValue(Constants.Trust.ComputedKeyAlgorithms.PSHA1)
                writer.WriteEndElement() ' wst:ComputedKey
                writer.WriteEndElement() ' wst:RequestedSecurityToken

                ' Write the wst:Entropy start tag
                writer.WriteStartElement(Constants.Trust.Elements.Entropy, Constants.Trust.NamespaceUri)
                ' Write the issuerEntropy out using the serializer
                ser.WriteToken(writer, Me.m_issuerEntropy)
                writer.WriteEndElement() ' wst:Entropy
            End If

            writer.WriteEndElement() ' wst:RequestSecurityTokenResponse

        End Sub

    End Class

End Namespace

