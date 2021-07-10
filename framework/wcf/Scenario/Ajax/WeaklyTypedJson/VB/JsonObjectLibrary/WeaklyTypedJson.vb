'----------------------------------------------------------------
' Copyright (c) Microsoft Corporation.  All rights reserved. 
'----------------------------------------------------------------

Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.Runtime.Serialization.Json
Imports System.Xml

Namespace Microsoft.Samples.WeaklyTypedJson
	Public Class JsonObject
        Inherits JsonCollection

        Public Sub New(ByVal reader As XmlReader)
            MyBase.New(reader, New Dictionary(Of String, JsonBaseType)())
        End Sub

        ' In an object (dictionary), the [string] indexer is appropriate, so implement it.
        ' The [int] indexer will throw as per JsonBaseType.
        Default Public Overrides Property Item(ByVal key As String) As JsonBaseType
            Get
                Return (CType(InternalValue, Dictionary(Of String, JsonBaseType)))(key)
            End Get
            Set(ByVal value As JsonBaseType)
                CType(InternalValue, Dictionary(Of String, JsonBaseType))(key) = CType(value, JsonBaseType)
            End Set
        End Property

        ' Implement the correct add method for an object (dictionary)
        Friend Sub Add(ByVal key As String, ByVal value As JsonBaseType)
            CType(InternalValue, Dictionary(Of String, JsonBaseType)).Add(key, value)
        End Sub
    End Class

	Public Class JsonArray
        Inherits JsonCollection

        Friend Sub New(ByVal reader As XmlReader)
            MyBase.New(reader, New List(Of JsonBaseType)())
        End Sub

        ' In an array, the [int] indexer is appropriate, so implement it.
        ' The [string] indexer will throw as per JsonBaseType.
        Default Public Overrides Property Item(ByVal index As Integer) As JsonBaseType
            Get
                Return (CType(InternalValue, List(Of JsonBaseType)))(index)
            End Get
            Set(ByVal value As JsonBaseType)
                CType(InternalValue, List(Of JsonBaseType))(index) = CType(value, JsonBaseType)
            End Set
        End Property

        ' Implement the correct add method for an array
        Friend Sub Add(ByVal value As JsonBaseType)
            CType(InternalValue, List(Of JsonBaseType)).Add(value)
        End Sub

	End Class

	' JSON contains two collection types: an object and an array. This class abstracts some 
	' common functionality used by both.
	Public Class JsonCollection
        Inherits JsonBaseType

		Private Const exceptionString As String = "You cannot use this Add method on this JSON type"

        Friend Sub New(ByVal reader As XmlReader, ByVal value As Object)
            MyBase.New(value)
            Dim nodeName, nodeType As String
            Dim rootName As String = reader.Name

            Do While reader.Read()
                If reader.IsStartElement() Then

                    nodeName = reader.Name
                    reader.MoveToAttribute("type")
                    nodeType = reader.Value
                    reader.MoveToElement()

                    Select Case nodeType
                        ' The object JSON type needs to be handled by us recursively, since 
                        ' DataContractJsonSerializer cannot deserialize it without a DataContract, 
                        ' which we don't have
                        Case "object"
                            AddSelector(nodeName, New JsonObject(reader))

                            ' Normally DataContractJson serializer can deserialize arrays, but we could have
                            ' an array with an object as one if its items. In that case the serializer
                            ' wouldn't work, so we need to handle the entire array case manually
                        Case "array"
                            AddSelector(nodeName, New JsonArray(reader))

                            ' The number, string, and bool JSON types can be deferred to 
                            ' DataContractJsonSerializer
                        Case "number"
                            AddValueType(Of Double)(reader)

                        Case "string"
                            AddValueType(Of String)(reader)

                        Case "boolean"
                            AddValueType(Of Boolean)(reader)

                            ' For null values, we just use the CLR null type
                        Case "null"
                            AddSelector(nodeName, Nothing)
                    End Select
                ElseIf reader.Name = rootName Then
                    Exit Do
                End If
            Loop

        End Sub

        ' We will be working with one if this class' children - object or array - so we need to 
        ' use the add method that's appropriate for that child
        Private Sub AddSelector(ByVal key As String, ByVal value As JsonBaseType)
            Dim thisArray As JsonArray = TryCast(Me, JsonArray)

            If thisArray IsNot Nothing Then
                thisArray.Add(value)
            Else
                CType(Me, JsonObject).Add(key, value)
            End If

        End Sub

        Private Sub AddValueType(Of T)(ByVal reader As XmlReader)
            ' Use DataContractJsonSerialzier to deserialize JXML into a CLR type
            Dim serializer As New DataContractJsonSerializer(GetType(T), reader.Name)
            Dim jsonElement As New JsonBaseType(CType(serializer.ReadObject(reader.ReadSubtree()), T))
            AddSelector(reader.Name, jsonElement)
        End Sub
	End Class

    Public Class JsonBaseType

        Private Const exceptionString As String = "You cannot use indexers on this JSON type"
        Private _internalValue As Object

        ' Constructor
        Friend Sub New(ByVal value As Object)
            InternalValue = value
        End Sub

        Protected Property InternalValue() As Object
            Get
                Return _internalValue
            End Get
            Set(ByVal value As Object)
                _internalValue = value
            End Set
        End Property

        ' The generic JSON type needs to support indexers of both [string] and [int] (for the JSON
        ' object and array) and also cast easily into int, bool, and string (for the JSON number,
        ' boolean, and string types)

        ' Indexers
        Default Public Overridable Property Item(ByVal key As String) As JsonBaseType
            Get
                Throw New NotSupportedException(exceptionString)
            End Get
            Set(ByVal value As JsonBaseType)
                Throw New NotSupportedException(exceptionString)
            End Set
        End Property

        Default Public Overridable Property Item(ByVal index As Integer) As JsonBaseType
            Get
                Throw New NotSupportedException(exceptionString)
            End Get
            Set(ByVal value As JsonBaseType)
                Throw New NotSupportedException(exceptionString)
            End Set
        End Property

        ' Type cast operator overloads
        Public Shared Widening Operator CType(ByVal type As JsonBaseType) As Integer
            ' Have to do this to unbox correctly
            Return CInt(Fix(CDbl(type.InternalValue)))
        End Operator

        Public Shared Widening Operator CType(ByVal type As JsonBaseType) As Double
            ' Have to do this to unbox correctly
            Return CDbl(type.InternalValue)
        End Operator

        Public Shared Widening Operator CType(ByVal type As JsonBaseType) As Boolean
            Return CBool(type.InternalValue)
        End Operator

        Public Shared Widening Operator CType(ByVal type As JsonBaseType) As String
            Return CStr(type.InternalValue)
        End Operator

    End Class
End Namespace
