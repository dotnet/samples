' Copyright (c) Microsoft Corporation.  All Rights Reserved.

Imports System

Imports System.IdentityModel.Tokens

Imports System.Runtime.InteropServices
Imports System.Runtime.Serialization

Imports System.ServiceModel
Imports System.ServiceModel.Channels
Imports System.ServiceModel.Security.Tokens

Imports System.Xml

Namespace Microsoft.Samples.Federation

    ''' <summary>
    ''' A class that represents a RequestSecurityToken message per February 2005 WS-Trust
    ''' </summary>
    <ComVisible(False)> _
    Public Class RequestSecurityToken
        Inherits RequestSecurityTokenBase

        ' private members
        Private m_requestorEntropy As SecurityToken

        ' Constructors
        ''' <summary>
        ''' Default constructor
        ''' </summary>
        Public Sub New()

            Me.New([String].Empty, [String].Empty, 0, Nothing, Nothing)

        End Sub

        ''' <summary>
        ''' Parameterized constructor
        ''' </summary>
        ''' <param name="context">The value of the wst:RequestSecurityToken/@Context attribute</param>
        ''' <param name="tokenType">The content of the wst:RequestSecurityToken/wst:TokenType element</param>
        ''' <param name="keySize">The content of the wst:RequestSecurityToken/wst:KeySize element</param>
        ''' <param name="appliesTo">An EndpointReference that corresponds to the content of the wst:RequestSecurityToken/wsp:AppliesTo element</param>
        ''' <param name="entropy">A SecurityToken representing entropy provided by the requester in the wst:RequestSecurityToken/wst:Entropy element</param>        
        Public Sub New(ByVal context As String, ByVal tokenType As String, ByVal keySize As Integer, ByVal appliesTo As EndpointAddress, ByVal entropy As SecurityToken)

            MyBase.New(context, tokenType, keySize, appliesTo)
            ' Pass first 4 params to base class
            Me.m_requestorEntropy = entropy

        End Sub

        ' public properties

        ''' <summary>
        ''' The SecurityToken that represents entropy provided by the requester.
        ''' Null if the requester did not provide entropy.
        ''' </summary>
        Public Property RequestorEntropy() As SecurityToken

            Get

                Return m_requestorEntropy

            End Get
            Set(ByVal value As SecurityToken)

                m_requestorEntropy = value

            End Set

        End Property

        ' Static methods

        ''' <summary>
        ''' Reads a wst:RequestSecurityToken element, its attributes and children and 
        ''' creates a RequestSecurityToken instance with the appropriate values
        ''' </summary>
        ''' <param name="xr">An XmlReader positioned on wst:RequestSecurityToken</param>
        ''' <returns>A RequestSecurityToken instance, initialized with the data read from the XmlReader</returns>
        Public Shared Function CreateFrom(ByVal xr As XmlReader) As RequestSecurityToken

            Return ProcessRequestSecurityTokenElement(xr)

        End Function

        ' Methods of BodyWriter

        ''' <summary>
        ''' Writes out an XML representation of the instance.
        ''' Provided here for completeness. Not actually called by this sample.
        ''' </summary>
        ''' <param name="writer">The writer to be used to write out the XML content</param>
        Protected Overloads Overrides Sub OnWriteBodyContents(ByVal writer As XmlDictionaryWriter)

            ' Write out the wst:RequestSecurityToken start tag
            writer.WriteStartElement(Constants.Trust.Elements.RequestSecurityToken, Constants.Trust.NamespaceUri)

            ' If we have a non-null, non-empty tokenType...
            If Me.TokenType IsNot Nothing AndAlso Me.TokenType.Length > 0 Then

                ' Write out the wst:TokenType start tag
                writer.WriteStartElement(Constants.Trust.Elements.TokenType, Constants.Trust.NamespaceUri)
                ' Write out the tokenType string
                writer.WriteString(Me.TokenType)
                ' wst:TokenType
                writer.WriteEndElement()

            End If

            ' If we have a non-null appliesTo
            If Me.AppliesTo IsNot Nothing Then

                ' Write out the wsp:AppliesTo start tag
                writer.WriteStartElement(Constants.Policy.Elements.AppliesTo, Constants.Policy.NamespaceUri)
                ' Write the appliesTo in WS-Addressing 1.0 format
                Me.AppliesTo.WriteTo(AddressingVersion.WSAddressing10, writer)
                ' wsp:AppliesTo
                writer.WriteEndElement()

            End If

            ' If the requester provided entropy
            If Me.m_requestorEntropy IsNot Nothing Then

                ' Write out the wst:Entropy start tag
                writer.WriteStartElement(Constants.Trust.Elements.Entropy, Constants.Trust.NamespaceUri)

                ' Try to make the requesterEntropy SecurityToken into a BinaerySecretSecurityToken
                Dim bsst As BinarySecretSecurityToken = TryCast(Me.m_requestorEntropy, BinarySecretSecurityToken)

                ' If requesterEntropy is a BinaerySecretSecurityToken
                If bsst IsNot Nothing Then

                    ' Write out the wst:BinarySecret start tag
                    writer.WriteStartElement(Constants.Trust.Elements.BinarySecret, Constants.Trust.NamespaceUri)

                    ' Get the entropy bytes
                    Dim key() As Byte = bsst.GetKeyBytes()

                    ' Write them out as base64
                    writer.WriteBase64(key, 0, key.Length)
                    ' wst:BinarySecret
                    writer.WriteEndElement()

                End If

                ' wst:Entropy
                writer.WriteEndElement()

            End If

            ' If there is a specified keySize
            If Me.KeySize > 0 Then

                ' Write the wst:KeySize start tag
                writer.WriteStartElement(Constants.Trust.Elements.KeySize, Constants.Trust.NamespaceUri)
                ' Write the keySize
                writer.WriteValue(Me.KeySize)
                ' wst:KeySize
                writer.WriteEndElement()

            End If

            ' wst:RequestSecurityToken
            writer.WriteEndElement()

        End Sub

        ' private methods

        ''' <summary>
        ''' Reads the wst:RequestSecurityToken element
        ''' </summary>
        ''' <param name="xr">An XmlReader, positioned on the start tag of wst:RequestSecurityToken</param>
        ''' <returns>A RequestSecurityToken instance, initialized with the data read from the XmlReader</returns>
        Private Shared Function ProcessRequestSecurityTokenElement(ByVal xr As XmlReader) As RequestSecurityToken

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
            Dim context As String = xr.GetAttribute(Constants.Trust.Attributes.Context, [String].Empty)

            ' Set up some default values
            Dim tokenType As String = [String].Empty
            Dim keySize As Integer = 0
            Dim appliesTo As EndpointAddress = Nothing
            Dim entropy As SecurityToken = Nothing

            ' Enter a read loop...
            While xr.Read()

                ' Process element start tags
                If XmlNodeType.Element = xr.NodeType Then

                    ' Process WS-Trust elements
                    If Constants.Trust.NamespaceUri = xr.NamespaceURI Then

                        If Constants.Trust.Elements.TokenType = xr.LocalName AndAlso Not xr.IsEmptyElement Then

                            xr.Read()
                            tokenType = xr.ReadContentAsString()

                        ElseIf Constants.Trust.Elements.KeySize = xr.LocalName AndAlso Not xr.IsEmptyElement Then

                            xr.Read()
                            keySize = xr.ReadContentAsInt()

                        ElseIf Constants.Trust.Elements.Entropy = xr.LocalName AndAlso Not xr.IsEmptyElement Then

                            entropy = ProcessEntropyElement(xr)

                        End If

                    ElseIf Constants.Policy.NamespaceUri = xr.NamespaceURI Then

                        ' Process WS-Policy elements
                        If Constants.Policy.Elements.AppliesTo = xr.LocalName AndAlso Not xr.IsEmptyElement Then

                            appliesTo = ProcessAppliesToElement(xr)

                        End If

                    End If

                End If

                ' Look for the end-tag that corresponds to the start-tag the reader was positioned 
                ' on when the method was called. When we find it, break out of the read loop.
                If Constants.Trust.Elements.RequestSecurityToken = xr.LocalName AndAlso Constants.Trust.NamespaceUri = xr.NamespaceURI AndAlso xr.Depth = initialDepth AndAlso XmlNodeType.EndElement = xr.NodeType Then
                    Exit While
                End If

            End While

            ' Construct a new RequestSecurityToken based on the values read and return it
            Return New RequestSecurityToken(context, tokenType, keySize, appliesTo, entropy)

        End Function

        ''' <summary>
        ''' Reads a wst:Entropy element and constructs a SecurityToken
        ''' Assumes that the provided entropy is never more than 1Kb in size
        ''' </summary>
        ''' <param name="xr">An XmlReader positioned on the start tag of wst:Entropy</param>
        ''' <returns>A SecurityToken that contains the entropy value</returns>
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
                If Constants.Trust.Elements.BinarySecret = xr.LocalName AndAlso Constants.Trust.NamespaceUri = xr.NamespaceURI AndAlso Not xr.IsEmptyElement AndAlso XmlNodeType.Element = xr.NodeType Then

                    ' Allocate a 1024 byte buffer for the entropy
                    Dim temp() As Byte = New Byte(1023) {}
                    ' Move reader to content of wst:BinarySecret element...
                    xr.Read()

                    ' ...and read that content as base64. Store the actual number of bytes we get.                    
                    Dim nBytes As Integer = xr.ReadContentAsBase64(temp, 0, temp.Length)

                    ' Allocate a new array of the correct size to hold the provided entropy
                    Dim entropy() As Byte = New Byte(nBytes - 1) {}

                    ' Copy the entropy from the temporary array into the new array.
                    For i As Integer = 0 To nBytes - 1
                        entropy(i) = temp(i)
                    Next

                    ' Create new BinarySecretSecurityToken from the provided entropy
                    st = New BinarySecretSecurityToken(entropy)

                End If

                ' Look for the end-tag that corresponds to the start-tag the reader was positioned 
                ' on when the method was called. When we find it, break out of the read loop.
                If Constants.Trust.Elements.Entropy = xr.LocalName AndAlso Constants.Trust.NamespaceUri = xr.NamespaceURI AndAlso xr.Depth = initialDepth AndAlso XmlNodeType.EndElement = xr.NodeType Then
                    Exit While
                End If

            End While

            ' return the SecurityToken
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

                End If

                ' Look for the end-tag that corresponds to the start-tag the reader was positioned 
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

