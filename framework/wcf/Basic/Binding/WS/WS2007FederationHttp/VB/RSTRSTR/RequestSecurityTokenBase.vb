' Copyright (c) Microsoft Corporation. All rights reserved.

Imports System

Imports System.ServiceModel
Imports System.ServiceModel.Channels

Namespace Microsoft.Samples.WS2007FederationHttpBinding
    Public MustInherit Class RequestSecurityTokenBase
        Inherits BodyWriter
        ' private members
        Private m_context As String
        Private m_tokenType As String
        Private m_keySize As Integer
        Private m_appliesTo As EndpointAddress

        ' Constructors
        Protected Sub New()
            Me.New([String].Empty, [String].Empty, 0, Nothing)
        End Sub

        Protected Sub New(ByVal context As String, ByVal tokenType As String, ByVal keySize As Integer, ByVal appliesTo As EndpointAddress)
            MyBase.New(True)
            Me.m_context = context
            Me.m_tokenType = tokenType
            Me.m_keySize = keySize
            Me.m_appliesTo = appliesTo
        End Sub

        ' public properties
        Public Property Context() As String
            Get
                Return m_context
            End Get
            Set(ByVal value As String)
                m_context = value
            End Set
        End Property

        Public Property TokenType() As String
            Get
                Return m_tokenType
            End Get
            Set(ByVal value As String)
                m_tokenType = value
            End Set
        End Property

        Public Property KeySize() As Integer
            Get
                Return m_keySize
            End Get
            Set(ByVal value As Integer)
                m_keySize = value
            End Set
        End Property

        Public Property AppliesTo() As EndpointAddress
            Get
                Return m_appliesTo
            End Get
            Set(ByVal value As EndpointAddress)
                m_appliesTo = value
            End Set
        End Property
    End Class
End Namespace
