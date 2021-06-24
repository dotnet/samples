' Copyright (c) Microsoft Corporation.  All Rights Reserved.

Imports System

Imports System.Runtime.InteropServices

Imports System.ServiceModel
Imports System.ServiceModel.Channels

Namespace Microsoft.Samples.Federation

    ''' <summary>
    ''' abstract base class containing properties that are shared by RST and RSTR classes
    ''' </summary>
    <ComVisible(False)> _
    Public MustInherit Class RequestSecurityTokenBase
        Inherits BodyWriter

        ' private members
        Private m_context As String
        Private m_tokenType As String
        Private m_keySize As Integer
        Private m_appliesTo As EndpointAddress

        ' Constructors
        ''' <summary>
        ''' Default constructor
        ''' </summary>
        Protected Sub New()

            Me.New([String].Empty, [String].Empty, 0, Nothing)

        End Sub

        ''' <summary>
        ''' Parameterized constructor
        ''' </summary>
        ''' <param name="context">The value of the wst:RequestSecurityToken/@Context attribute in the request message, if any</param>
        ''' <param name="tokenType">The content of the wst:RequestSecurityToken/wst:TokenType element in the request message, if any</param>
        ''' <param name="keySize">The content of the wst:RequestSecurityToken/wst:KeySize element in the request message, if any</param>
        ''' <param name="appliesTo">An EndpointRefernece corresponding to the content of the wst:RequestSecurityToken/wsp:AppliesTo element in the request message, if any</param>
        Protected Sub New(ByVal context As String, ByVal tokenType As String, ByVal keySize As Integer, ByVal appliesTo As EndpointAddress)

            MyBase.New(True)
            Me.m_context = context
            Me.m_tokenType = tokenType
            Me.m_keySize = keySize
            Me.m_appliesTo = appliesTo

        End Sub

        ' public properties

        ''' <summary>
        ''' Context for the RST/RSTR exchange. 
        ''' The value of the wst:RequestSecurityToken/@Context attribute from RequestSecurityToken messages
        ''' The value of the wst:RequestSecurityTokenResponse/@Context attribute from RequestSecurityTokenResponse messages        
        ''' </summary>
        Public Property Context() As String

            Get

                Return m_context

            End Get
            Set(ByVal value As String)

                m_context = value

            End Set

        End Property

        ''' <summary>
        ''' The type of token requested or returned.
        ''' The value of the wst:RequestSecurityToken/wst:TokenType element from RequestSecurityToken messages
        ''' The value of the wst:RequestSecurityTokenResponse/wst:TokenType element from RequestSecurityTokenResponse messages       
        ''' </summary>
        Public Property TokenType() As String

            Get

                Return m_tokenType

            End Get
            Set(ByVal value As String)

                m_tokenType = value

            End Set

        End Property

        ''' <summary>
        ''' The size of the requested proof key
        ''' The value of the wst:RequestSecurityToken/wst:KeySize element from RequestSecurityToken messages
        ''' The value of the wst:RequestSecurityTokenResponse/wst:KeySize element from RequestSecurityTokenResponse messages       
        ''' </summary>
        Public Property KeySize() As Integer

            Get

                Return m_keySize

            End Get
            Set(ByVal value As Integer)

                m_keySize = value

            End Set

        End Property

        ''' <summary>
        ''' The EndpointAddress a token is being requested or returned for 
        ''' The content of the wst:RequestSecurityToken/wsp:AppliesTo element from RequestSecurityToken messages
        ''' The content of the wst:RequestSecurityTokenResponse/wsp:AppliesTo element from RequestSecurityTokenResponse messages       
        ''' </summary>public int KeySize
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

