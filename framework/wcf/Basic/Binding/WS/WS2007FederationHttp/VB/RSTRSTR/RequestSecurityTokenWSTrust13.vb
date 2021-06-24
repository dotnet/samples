' Copyright (c) Microsoft Corporation. All rights reserved.

Imports System
Imports System.Collections.Generic
Imports System.IdentityModel.Tokens
Imports System.Runtime.Serialization
Imports System.Security.Cryptography
Imports System.Security.Permissions
Imports System.ServiceModel
Imports System.ServiceModel.Channels
Imports System.ServiceModel.Security.Tokens
Imports System.Text
Imports System.Xml

Namespace Microsoft.Samples.WS2007FederationHttpBinding
    ' This class is specific to the version WS-Trust13
    Public Class RequestSecurityTokenWSTrust13
        Inherits RequestSecurityTokenBase

        ' private members
        Private m_keyType As String ' Tracks the type of the proof key (if any)
        Private m_requestType As String ' Tracks the request type (e.g. Issue, Renew, Cancel )        
        Private m_requestorEntropy As SecurityToken
        Private m_proofKey As SecurityToken

        ' Constructors
        ''' <summary>
        ''' Default constructor
        ''' </summary>
        Public Sub New()
            Me.New([String].Empty, [String].Empty, [String].Empty, 0, Constants.Trust13.KeyTypes.Symmetric, Nothing, _
                Nothing, Nothing)
        End Sub

        ''' <summary>
        ''' Parameterized constructor
        ''' </summary>
        ''' <param name="context">The value of the wst:RequestSecurityToken/@Context attribute</param>
        ''' <param name="tokenType">The content of the wst:RequestSecurityToken/wst:TokenType element</param>
        ''' <param name="requestType"></param>
        ''' <param name="keySize">The content of the wst:RequestSecurityToken/wst:KeySize element</param>
        ''' <param name="keyType"></param>
        ''' <param name="proofKey"></param>
        ''' <param name="entropy">A SecurityToken representing entropy provided by the requester in the wst:RequestSecurityToken/wst:Entropy element</param>
        ''' <param name="appliesTo">The content of the wst:RequestSecurityToken/wst:KeySize element</param>
        Public Sub New(ByVal context As String, ByVal tokenType As String, ByVal requestType As String, ByVal keySize As Integer, ByVal keyType As String, ByVal proofKey As SecurityToken, _
            ByVal entropy As SecurityToken, ByVal appliesTo As EndpointAddress)
            MyBase.New(context, tokenType, keySize, appliesTo)
            Me.m_keyType = keyType
            Me.m_proofKey = proofKey
            Me.m_requestType = requestType
            Me.m_requestorEntropy = entropy
        End Sub

        ' public properties
        Public Property RequestType() As String
            Get
                Return m_requestType
            End Get
            Set(ByVal value As String)
                m_requestType = value
            End Set
        End Property

        Public Property KeyType() As String
            Get
                Return m_keyType
            End Get
            Set(ByVal value As String)
                m_keyType = value
            End Set
        End Property

        Public Property ProofKey() As SecurityToken
            Get
                Return m_proofKey
            End Get
            Set(ByVal value As SecurityToken)
                m_proofKey = value
            End Set
        End Property

        ''' <summary>
        ''' The SecurityToken representing entropy provided by the requester.
        ''' Null if the requester did not provide entropy
        ''' </summary>
        Public Property RequestorEntropy() As SecurityToken
            Get
                Return m_requestorEntropy
            End Get
            Set(ByVal value As SecurityToken)
                m_requestorEntropy = value
            End Set
        End Property

        ' public methods
        Public Function IsProofKeyAsymmetric() As Boolean
            Return Constants.Trust13.KeyTypes.[Public] = m_keyType
        End Function

        ''' <summary>
        ''' Reads a wst:RequestSecurityToken element, its attributes and children and 
        ''' creates a RequestSecurityToken instance with the appropriate values
        ''' </summary>
        ''' <param name="xr">An XmlReader positioned on wst:RequestSecurityToken</param>
        ''' <returns>A RequestSecurityToken instance, initialized with the data read from the XmlReader</returns>
        Public Shared Function CreateFrom(ByVal xr As XmlReader) As RequestSecurityTokenWSTrust13
            Return ProcessRequestSecurityTokenElement(xr)
        End Function

        ' Methods of BodyWriter
        ''' <summary>
        ''' Writes out an XML representation of the instance.        
        ''' </summary>
        ''' <param name="writer">The writer to be used to write out the XML content</param>
        Protected Overloads Overrides Sub OnWriteBodyContents(ByVal writer As XmlDictionaryWriter)
            ' Write out the wst:RequestSecurityToken start tag
            writer.WriteStartElement(Constants.Trust13.Elements.RequestSecurityToken, Constants.Trust13.NamespaceUri)

            ' If we have a non-null, non-empty tokenType...
            If Me.TokenType IsNot Nothing AndAlso Me.TokenType.Length > 0 Then
                ' Write out the wst:TokenType start tag
                writer.WriteStartElement(Constants.Trust13.Elements.TokenType, Constants.Trust13.NamespaceUri)
                ' Write out the tokenType string
                writer.WriteString(Me.TokenType)
                writer.WriteEndElement() ' wst:TokenType
            End If

            ' If we have a non-null, non-empty requestType...
            If Me.m_requestType IsNot Nothing AndAlso Me.m_requestType.Length > 0 Then
                ' Write out the wst:RequestType start tag
                writer.WriteStartElement(Constants.Trust13.Elements.RequestType, Constants.Trust13.NamespaceUri)
                ' Write out the requestType string
                writer.WriteString(Me.m_requestType)
                writer.WriteEndElement() ' wst:RequestType
            End If

            ' If we have a non-null appliesTo
            If Me.AppliesTo IsNot Nothing Then
                ' Write out the wsp:AppliesTo start tag
                writer.WriteStartElement(Constants.Policy.Elements.AppliesTo, Constants.Policy.NamespaceUri)
                ' Write the appliesTo in WS-Addressing 1.0 format
                Me.AppliesTo.WriteTo(AddressingVersion.WSAddressing10, writer)
                writer.WriteEndElement() ' wsp:AppliesTo
            End If

            If Me.m_requestorEntropy IsNot Nothing Then
                writer.WriteStartElement(Constants.Trust13.Elements.Entropy, Constants.Trust13.NamespaceUri)
                Dim bsst As BinarySecretSecurityToken = TryCast(Me.m_requestorEntropy, BinarySecretSecurityToken)
                If bsst IsNot Nothing Then
                    writer.WriteStartElement(Constants.Trust13.Elements.BinarySecret, Constants.Trust13.NamespaceUri)
                    Dim key As Byte() = bsst.GetKeyBytes()
                    writer.WriteBase64(key, 0, key.Length)
                    writer.WriteEndElement() ' wst:BinarySecret
                End If
                writer.WriteEndElement() ' wst:Entropy
            End If

            If Me.m_keyType IsNot Nothing AndAlso Me.m_keyType.Length > 0 Then
                writer.WriteStartElement(Constants.Trust13.Elements.KeyType, Constants.Trust13.NamespaceUri)
                writer.WriteString(Me.m_keyType)

                writer.WriteEndElement() ' wst:KeyType
            End If

            If Me.KeySize > 0 Then
                writer.WriteStartElement(Constants.Trust13.Elements.KeySize, Constants.Trust13.NamespaceUri)
                writer.WriteValue(Me.KeySize)
                writer.WriteEndElement() ' wst:KeySize
            End If

            writer.WriteEndElement() ' wst:RequestSecurityToken
        End Sub


        ' private methods

        ''' <summary>
        ''' Reads the wst:RequestSecurityToken element
        ''' </summary>
        ''' <param name="xr">An XmlReader, positioned on the start tag of wst:RequestSecurityToken</param>
        ''' <returns>A RequestSecurityToken instance, initialized with the data read from the XmlReader</returns>
        Private Shared Function ProcessRequestSecurityTokenElement(ByVal xr As XmlReader) As RequestSecurityTokenWSTrust13
            ' If provided XmlReader is null, throw an exception
            If xr Is Nothing Then
                Throw New ArgumentNullException("xr")
            End If

            ' If the wst:RequestSecurityToken element is empty, then throw an exception.
            If xr.IsEmptyElement Then
                Throw New ArgumentException("wst:RequestSecurityToken element was empty. Unable to create RequestSecurityToken object")
            End If

            ' Store the initial depth so we can exit this function when we reach the corresponding end-tag
            Dim initialDepth As Integer = xr.Depth

            ' Extract the @Context attribute value.                           
            Dim context As String = xr.GetAttribute(Constants.Trust13.Attributes.Context, [String].Empty)

            Dim tokenType As String = [String].Empty
            Dim requestType As String = [String].Empty
            Dim keySize As Integer = 0
            Dim keyType As String = Constants.Trust13.KeyTypes.Symmetric
            Dim appliesTo As EndpointAddress = Nothing
            Dim entropy As SecurityToken = Nothing
            Dim proofKey As SecurityToken = Nothing

            ' Enter a read loop...
            While xr.Read()
                ' Process element start tags
                If XmlNodeType.Element = xr.NodeType Then
                    ' Process WS-Trust13 elements
                    If Constants.Trust13.NamespaceUri = xr.NamespaceURI Then
                        If Constants.Trust13.Elements.RequestType = xr.LocalName AndAlso Not xr.IsEmptyElement Then
                            xr.Read()
                            requestType = xr.ReadContentAsString()
                        ElseIf Constants.Trust13.Elements.TokenType = xr.LocalName AndAlso Not xr.IsEmptyElement Then
                            xr.Read()
                            tokenType = xr.ReadContentAsString()
                        ElseIf Constants.Trust13.Elements.KeySize = xr.LocalName AndAlso Not xr.IsEmptyElement Then
                            xr.Read()
                            keySize = xr.ReadContentAsInt()
                        ElseIf Constants.Trust13.Elements.KeyType = xr.LocalName AndAlso Not xr.IsEmptyElement Then
                            xr.Read()
                            keyType = xr.ReadContentAsString()
                        ElseIf Constants.Trust13.Elements.Entropy = xr.LocalName AndAlso Not xr.IsEmptyElement Then
                            entropy = ProcessEntropyElement(xr)
                        Else
                            Console.WriteLine("Not processing element: {0}:{1}", xr.NamespaceURI, xr.LocalName)
                        End If
                        ' Process WS-Policy elements
                    ElseIf Constants.Policy.NamespaceUri = xr.NamespaceURI Then
                        If Constants.Policy.Elements.AppliesTo = xr.LocalName AndAlso Not xr.IsEmptyElement Then
                            appliesTo = ProcessAppliesToElement(xr)
                        Else
                            Console.WriteLine("Not processing element: {0}:{1}", xr.NamespaceURI, xr.LocalName)
                        End If
                    Else
                        Console.WriteLine("Not processing element: {0}:{1}", xr.NamespaceURI, xr.LocalName)
                    End If
                End If

                ' Look for the end-tag corresponding to the start-tag the reader was positioned 
                ' on when the method was called
                If Constants.Trust13.Elements.RequestSecurityToken = xr.LocalName AndAlso Constants.Trust13.NamespaceUri = xr.NamespaceURI AndAlso xr.Depth = initialDepth AndAlso XmlNodeType.EndElement = xr.NodeType Then
                    Exit While
                End If
            End While

            ' Construct a new RequestSecurityToken based on the values read and return it
            Return New RequestSecurityTokenWSTrust13(context, tokenType, requestType, keySize, keyType, proofKey, _
                entropy, appliesTo)
        End Function

        ''' <summary>
        ''' Reads a wst:Entropy element and constructs a SecurityToken
        ''' Assumes that the provided entropy will never be more than 1Kb in size
        ''' </summary>
        ''' <param name="xr">An XmlReader positioned on the start tag of wst:Entropy</param>
        ''' <returns>A SecurityToken containing the entropy value</returns>
        Private Shared Function ProcessEntropyElement(ByVal xr As XmlReader) As SecurityToken
            ' If provided XmlReader is null, throw an exception
            If xr Is Nothing Then
                Throw New ArgumentNullException("xr")
            End If

            ' If the wst:Entropy element is empty, then throw an exception.
            If xr.IsEmptyElement Then
                Throw New ArgumentException("wst:Entropy element was empty. Unable to create SecurityToken object")
            End If

            ' Store the initial depth so we can exit this function when we reach the corresponding end-tag            
            Dim initialDepth As Integer = xr.Depth

            ' Set our return value to null
            Dim st As SecurityToken = Nothing

            ' Enter a read loop...
            While xr.Read()
                ' Look for a non-empty wst:BinarySecret element
                If Constants.Trust13.Elements.BinarySecret = xr.LocalName AndAlso Constants.Trust13.NamespaceUri = xr.NamespaceURI AndAlso Not xr.IsEmptyElement AndAlso XmlNodeType.Element = xr.NodeType Then
                    ' Allocate a 1024 byte buffer for the entropy
                    Dim temp As Byte() = New Byte(1023) {}

                    ' Move reader to content of wst:BinarySecret element...
                    xr.Read()

                    ' ...and read that content as base64. Store the actual number of bytes we get.                    
                    Dim nBytes As Integer = xr.ReadContentAsBase64(temp, 0, temp.Length)

                    ' Allocate a new array of the correct size to hold the provided entropy
                    Dim entropy As Byte() = New Byte(nBytes) {}
                    For i As Integer = 0 To nBytes - 1
                        entropy(i) = temp(i)
                    Next

                    ' Copy the entropy from the temporary array into the new array.

                    ' Create new BinarySecretSecurityToken from the provided entropy
                    st = New BinarySecretSecurityToken(entropy)
                End If

                ' Look for the end-tag corresponding to the start-tag the reader was positioned 
                ' on when the method was called. When we find it, break out of the read loop.
                If Constants.Trust13.Elements.Entropy = xr.LocalName AndAlso Constants.Trust13.NamespaceUri = xr.NamespaceURI AndAlso xr.Depth = initialDepth AndAlso XmlNodeType.EndElement = xr.NodeType Then
                    Exit While
                End If
            End While

            Return st
        End Function

        ''' <summary>
        ''' Reads a wsp:AppliesTo element
        ''' </summary>
        ''' <param name="xr">An XmlReader positioned on the start tag of wsp:AppliesTo</param>
        ''' <returns>An EndpointAddress</returns>
        Private Shared Function ProcessAppliesToElement(ByVal xr As XmlReader) As EndpointAddress
            ' If provided XmlReader is null, throw an exception
            If xr Is Nothing Then
                Throw New ArgumentNullException("xr")
            End If

            ' If the wsp:AppliesTo element is empty, then throw an exception.
            If xr.IsEmptyElement Then
                Throw New ArgumentException("wsp:AppliesTo element was empty. Unable to create EndpointAddress object")
            End If

            ' Store the initial depth so we can exit this function when we reach the corresponding end-tag
            Dim initialDepth As Integer = xr.Depth

            ' Set our return value to null
            Dim ea As EndpointAddress = Nothing

            ' Enter a read loop...
            While xr.Read()
                ' Look for a WS-Addressing 1.0 Endpoint Reference...
                If Constants.Addressing.Elements.EndpointReference = xr.LocalName AndAlso Constants.Addressing.NamespaceUri = xr.NamespaceURI AndAlso Not xr.IsEmptyElement AndAlso XmlNodeType.Element = xr.NodeType Then
                    ' Create a DataContractSerializer for an EndpointAddress10
                    Dim dcs As New DataContractSerializer(GetType(EndpointAddress10))
                    ' Read the EndpointAddress10 from the DataContractSerializer
                    Dim ea10 As EndpointAddress10 = DirectCast(dcs.ReadObject(xr, False), EndpointAddress10)
                    ' Convert the EndpointAddress10 into an EndpointAddress
                    ea = ea10.ToEndpointAddress()
                    ' Look for a WS-Addressing 2004/08 Endpoint Reference...
                ElseIf Constants.Addressing.Elements.EndpointReference = xr.LocalName AndAlso Constants.Addressing.NamespaceUriAugust2004 = xr.NamespaceURI AndAlso Not xr.IsEmptyElement AndAlso XmlNodeType.Element = xr.NodeType Then
                    ' Create a DataContractSerializer for an EndpointAddressAugust2004
                    Dim dcs As New DataContractSerializer(GetType(EndpointAddressAugust2004))
                    ' Read the EndpointAddressAugust2004 from the DataContractSerializer
                    Dim eaAugust2004 As EndpointAddressAugust2004 = DirectCast(dcs.ReadObject(xr, False), EndpointAddressAugust2004)
                    ' Convert the EndpointAddressAugust2004 into an EndpointAddress
                    ea = eaAugust2004.ToEndpointAddress()
                End If

                ' Look for the end-tag corresponding to the start-tag the reader was positioned 
                ' on when the method was called. When we find it, break out of the read loop.
                If Constants.Policy.Elements.AppliesTo = xr.LocalName AndAlso Constants.Policy.NamespaceUri = xr.NamespaceURI AndAlso xr.Depth = initialDepth AndAlso XmlNodeType.EndElement = xr.NodeType Then
                    Exit While
                End If
            End While

            ' Return the EndpointAddress
            Return ea
        End Function
    End Class
End Namespace
