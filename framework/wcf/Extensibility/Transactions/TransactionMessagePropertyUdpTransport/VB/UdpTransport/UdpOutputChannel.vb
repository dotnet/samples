'  Copyright (c) Microsoft Corporation. All rights reserved.

Imports System.Globalization
Imports System.Net
Imports System.Net.Sockets
Imports System.ServiceModel
Imports System.ServiceModel.Channels

Namespace Microsoft.ServiceModel.Samples
	''' <summary>
	''' IOutputChannel implementation for Udp.
	''' </summary>
	Friend Class UdpOutputChannel
		Inherits ChannelBase
		Implements IOutputChannel

        Private remoteAddress_local As EndpointAddress

        Private via_local As Uri
        Private remoteEndPoint As EndPoint
        Private socket As Socket
        Private encoder As MessageEncoder
        Private parent As UdpChannelFactory

        Friend Sub New(ByVal factory As UdpChannelFactory, ByVal remoteAddress As EndpointAddress,
                       ByVal via As Uri, ByVal encoder As MessageEncoder)
            MyBase.New(factory)
            ' validate addressing arguments
            If Not String.Equals(via.Scheme, UdpConstants.Scheme, StringComparison.InvariantCultureIgnoreCase) Then
                Throw New ArgumentException(String.Format(CultureInfo.CurrentCulture,
                                                          "The scheme {0} specified in address is not supported.",
                                                          via.Scheme), "via")
            End If

            If via.IsDefaultPort Then
                Throw New ArgumentException(String.Format(CultureInfo.CurrentCulture,
                                                          "The address {0} must specify a remote port.", via), "via")
            End If

            ' convert the Uri host into an IP Address
            Dim remoteIP As IPAddress = Nothing
            Select Case via.HostNameType

                Case UriHostNameType.IPv4, UriHostNameType.IPv6
                    remoteIP = IPAddress.Parse(via.Host)

                Case UriHostNameType.Basic, UriHostNameType.Dns
                    Dim hostEntry As IPHostEntry = Dns.GetHostEntry(via.Host)
                    If hostEntry.AddressList.Length > 0 Then
                        remoteIP = hostEntry.AddressList(0)
                    Else
                        Throw New ArgumentException(String.Format(CultureInfo.CurrentCulture,
                                                                  "Failed to resolve remote host: {0}.",
                                                                  via.Host), "via")
                    End If
                Case Else
                    Throw New ArgumentException(String.Format(CultureInfo.CurrentCulture,
                                                              "Cannot determine the remote host address from {0}.",
                                                              Me.via_local.ToString()), "via")
                    Exit Select
            End Select

            If factory.Multicast AndAlso (Not UdpChannelHelpers.IsInMulticastRange(remoteIP)) Then
                Throw New ArgumentOutOfRangeException("remoteEndPoint", "Via must be in the valid multicast range.")
            End If

            Me.parent = factory
            Me.remoteAddress_local = remoteAddress
            Me.via_local = via
            Me.encoder = encoder
            Me.remoteEndPoint = New IPEndPoint(remoteIP, via.Port)
            Me.socket = New Socket(Me.remoteEndPoint.AddressFamily, SocketType.Dgram, ProtocolType.Udp)

            If parent.Multicast Then
                Me.socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1)
#If LATER Then ' Support outgoing interface
				If Me.remoteEndPoint.AddressFamily = AddressFamily.InterNetwork Then
					Me.socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastInterface, 
					                          IPAddress.HostToNetworkOrder(CInt(Fix(interfaceIndex))))
				Else ' we're IPv6
					Me.sendSocketV6.SetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.MulticastInterface, 
					                                CInt(Fix(interfaceIndex)))
				End If
#End If
            End If
        End Sub

#Region "IOutputChannel_Properties"
        Private ReadOnly Property RemoteAddress() As EndpointAddress Implements IOutputChannel.RemoteAddress
            Get
                Return Me.remoteAddress_local
            End Get
        End Property

        Private ReadOnly Property Via() As Uri Implements IOutputChannel.Via
            Get
                Return Me.via_local
            End Get
        End Property
#End Region

        Public ReadOnly Property Factory() As UdpChannelFactory
            Get
                Return Me.parent
            End Get
        End Property

      

        ''' <summary>
        ''' Open the channel for use. We don't have any blocking work to perform so this is a no-op
        ''' </summary>
        Protected Overrides Sub OnOpen(ByVal timeout As TimeSpan)
        End Sub

        Protected Overrides Function OnBeginOpen(ByVal timeout As TimeSpan, ByVal callback As AsyncCallback,
                                                 ByVal state As Object) As IAsyncResult
            Return New CompletedAsyncResult(callback, state)
        End Function

        Protected Overrides Sub OnEndOpen(ByVal result As IAsyncResult)
            CompletedAsyncResult.End(result)
        End Sub


#Region "Socket_Shutdown"
        ''' <summary>
        ''' Shutdown ungracefully
        ''' </summary>
        Protected Overrides Sub OnAbort()
            Me.socket.Close(0)
        End Sub

        ''' <summary>
        ''' Shutdown gracefully
        ''' </summary>
        Protected Overrides Sub OnClose(ByVal timeout As TimeSpan)
            Me.socket.Close()
        End Sub

        Protected Overrides Function OnBeginClose(ByVal timeout As TimeSpan, ByVal callback As AsyncCallback,
                                                  ByVal state As Object) As IAsyncResult
            Me.OnClose(timeout)
            Return New CompletedAsyncResult(callback, state)
        End Function

        Protected Overrides Sub OnEndClose(ByVal result As IAsyncResult)
            CompletedAsyncResult.End(result)
        End Sub
#End Region

#Region "Send_Synchronous"
        ''' <summary>
        ''' Address the Message and serialize it into a byte array.
        ''' </summary>
        Private Function EncodeMessage(ByVal message As Message) As ArraySegment(Of Byte)
            Try
                Me.remoteAddress_local.ApplyTo(message)
                Return encoder.WriteMessage(message, Integer.MaxValue, parent.BufferManager)
            Finally
                ' we've consumed the message by serializing it, so clean up.
                message.Close()
            End Try
        End Function

        Public Sub Send(ByVal message As Message) Implements IOutputChannel.Send
            If message Is Nothing Then
                Throw New ArgumentNullException("message")
            End If

            MyBase.ThrowIfDisposedOrNotOpen()

            'Obtain the transaction propagation token from the TransactionFlowProperty on the message.
            Dim txPropToken() As Byte = TransactionFlowProperty.Get(message)

            Dim messageBuffer As ArraySegment(Of Byte) = EncodeMessage(message)

            Dim txmsgBuffer() As Byte = TransactionMessageBuffer.WriteTransactionMessageBuffer(txPropToken, messageBuffer)
            If CLng(Fix(txmsgBuffer.Length)) > Me.Factory.MaxPacketSize Then
                Throw New CommunicationException("The output packet size is greater than the maximum size supported.")
            End If

            Try
                Dim bytesSent As Integer = Me.socket.SendTo(txmsgBuffer, 0, txmsgBuffer.Length,
                                                            SocketFlags.None, Me.remoteEndPoint)

                If bytesSent <> txmsgBuffer.Length Then
                    Throw New CommunicationException(String.Format(CultureInfo.CurrentCulture,
                                                                   "A Udp error occurred sending a message to {0}.",
                                                                   Me.remoteEndPoint))
                End If
            Catch socketException As SocketException
                Throw UdpChannelHelpers.ConvertTransferException(socketException)
            Finally
                ' we need to make sure buffers are always returned to the BufferManager.
                parent.BufferManager.ReturnBuffer(messageBuffer.Array)
            End Try
        End Sub

        Public Sub Send(ByVal message As Message, ByVal timeout As TimeSpan) Implements IOutputChannel.Send
            ' UDP doesn't block so we don't need timeouts.
            Me.Send(message)
        End Sub
#End Region

		#Region "Send_Asynchronous"
        Public Function BeginSend(ByVal message As Message, ByVal callback As AsyncCallback,
                                  ByVal state As Object) As IAsyncResult Implements IOutputChannel.BeginSend
            MyBase.ThrowIfDisposedOrNotOpen()
            Return New SendAsyncResult(Me, message, callback, state)
        End Function

        Public Function BeginSend(ByVal message As Message, ByVal timeout As TimeSpan, ByVal callback As AsyncCallback,
                                  ByVal state As Object) As IAsyncResult Implements IOutputChannel.BeginSend
            ' UDP doesn't block so we don't need timeouts.
            Return Me.BeginSend(message, callback, state)
        End Function

		Public Sub EndSend(ByVal result As IAsyncResult) Implements IOutputChannel.EndSend
			SendAsyncResult.End(result)
		End Sub

		''' <summary>
		''' Implementation of async send for Udp. 
		''' </summary>
		Private Class SendAsyncResult
			Inherits AsyncResult
			Private messageBuffer As ArraySegment(Of Byte)
			Private channel As UdpOutputChannel
			Private txmsgBuffer() As Byte

            Public Sub New(ByVal channel As UdpOutputChannel, ByVal message As Message,
                           ByVal callback As AsyncCallback, ByVal state As Object)
                MyBase.New(callback, state)
                Me.channel = channel

                'obtain the transaction propagation token from the TransactionFlowProperty on the message.
                Dim txPropToken() As Byte = TransactionFlowProperty.Get(message)

                Me.messageBuffer = channel.EncodeMessage(message)

                txmsgBuffer = TransactionMessageBuffer.WriteTransactionMessageBuffer(txPropToken, messageBuffer)
                If CLng(Fix(txmsgBuffer.Length)) > channel.Factory.MaxPacketSize Then
                    Throw New CommunicationException("The output packet size is greater than the maximum size supported.")
                End If

                Try
                    Dim result As IAsyncResult = Nothing
                    Try
                        result = channel.socket.BeginSendTo(txmsgBuffer, 0, txmsgBuffer.Length, SocketFlags.None,
                                                            channel.remoteEndPoint, New AsyncCallback(AddressOf OnSend), Me)
                    Catch socketException As SocketException
                        Throw UdpChannelHelpers.ConvertTransferException(socketException)
                    End Try

                    If Not result.CompletedSynchronously Then
                        Return
                    End If

                    CompleteSend(result, True)
                Catch
                    CleanupBuffer()
                    Throw
                End Try
            End Sub

			Private Sub CleanupBuffer()
				If messageBuffer.Array IsNot Nothing Then
					Me.channel.parent.BufferManager.ReturnBuffer(messageBuffer.Array)
					messageBuffer = New ArraySegment(Of Byte)()
				End If
			End Sub

			Private Sub CompleteSend(ByVal result As IAsyncResult, ByVal synchronous As Boolean)
				Try
					Dim bytesSent As Integer = channel.socket.EndSendTo(result)

					If bytesSent <> txmsgBuffer.Length Then
                        Throw New CommunicationException(String.Format(CultureInfo.CurrentCulture,
                                                                       "A Udp error occurred sending a message to {0}.",
                                                                       channel.remoteEndPoint))
					End If
				Catch socketException As SocketException
					Throw UdpChannelHelpers.ConvertTransferException(socketException)
				Finally
					CleanupBuffer()
				End Try

				MyBase.Complete(synchronous)
			End Sub

			Private Sub OnSend(ByVal result As IAsyncResult)
				If result.CompletedSynchronously Then
					Return
				End If

				Try
					CompleteSend(result, False)
				Catch e As Exception
					MyBase.Complete(False, e)
				End Try
			End Sub

            Public Overloads Shared Sub [End](ByVal result As IAsyncResult)
                AsyncResult.End(Of SendAsyncResult)(result)
            End Sub
		End Class
		#End Region
	End Class
End Namespace
