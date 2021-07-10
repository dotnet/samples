'  Copyright (c) Microsoft Corporation. All rights reserved.

Imports System.Globalization
Imports System.Net
Imports System.Net.Sockets
Imports System.ServiceModel
Imports System.ServiceModel.Channels
Imports System.ServiceModel.Description
Imports System.Threading
Imports System.Transactions
Imports System.Xml

Namespace Microsoft.ServiceModel.Samples
	''' <summary>
	''' IListenerFactory implementation for Udp.
	''' 
	''' Supports IInputChannel only, as Udp is fundamentally
	''' a datagram protocol. Uses a listening UDP socket for each
	''' Factory instance, then demuxes to the appropriate IInputListener
	''' based on the registered filters. More advanced implementations could factor
	''' out the listening socket and demux from this listening object
	''' to separate factories based on a framing Via.
	''' </summary>
	Friend Class UdpChannelListener
		Inherits ChannelListenerBase(Of IInputChannel)
		Private bufferManager As BufferManager

		'The UDP network sockets.
		Private listenSockets As List(Of Socket)

		Private maxMessageSize As Integer

        Private messageEncoderFactory_local As MessageEncoderFactory
        Private multicast As Boolean


        Private onReceive_local As AsyncCallback

        Private uri_local As Uri

        Private channelQueue As InputQueue(Of IInputChannel)

        'The channel associated with this listener.
        Private currentChannel As UdpInputChannel

        Private currentChannelLock As Object

        Friend Sub New(ByVal bindingElement As UdpTransportBindingElement, ByVal context As BindingContext)
            MyBase.New(context.Binding)
            Me.maxMessageSize = CInt(Fix(bindingElement.MaxReceivedMessageSize))
            Me.multicast = bindingElement.Multicast
            Me.bufferManager = BufferManager.CreateBufferManager(bindingElement.MaxBufferPoolSize, Me.maxMessageSize)
            Dim messageEncoderBindingElement As MessageEncodingBindingElement = context.BindingParameters.Remove(Of MessageEncodingBindingElement)()
            If messageEncoderBindingElement IsNot Nothing Then
                Me.messageEncoderFactory_local = messageEncoderBindingElement.CreateMessageEncoderFactory()
            Else
                Me.messageEncoderFactory_local = UdpConstants.DefaultMessageEncoderFactory
            End If
            Me.channelQueue = New InputQueue(Of IInputChannel)()
            Me.currentChannelLock = New Object()
            Me.listenSockets = New List(Of Socket)(2)

            Dim baseAddress As Uri = context.ListenUriBaseAddress
            If baseAddress Is Nothing Then
                If context.ListenUriMode = ListenUriMode.Unique Then
                    Dim uriBuilder As New UriBuilder(Me.Scheme, Dns.GetHostEntry(String.Empty).HostName)
                    uriBuilder.Path = Guid.NewGuid().ToString()
                    baseAddress = uriBuilder.Uri
                Else
                    Throw New InvalidOperationException("Null is only a supported value for ListenUriBaseAddress when using ListenUriMode.Unique.")
                End If
            End If

            Me.InitializeUri(baseAddress, context.ListenUriRelativeAddress, context.ListenUriMode)
        End Sub

        Friend ReadOnly Property InternalReceiveTimeout() As TimeSpan
            Get
                Return Me.DefaultReceiveTimeout
            End Get
        End Property

        Public ReadOnly Property MessageEncoderFactory() As MessageEncoderFactory
            Get
                Return messageEncoderFactory_local
            End Get
        End Property

        Private ReadOnly Property Scheme() As String
            Get
                Return UdpConstants.Scheme
            End Get
        End Property

        Public Overloads Overrides ReadOnly Property Uri() As Uri
            Get
                Return Me.uri_local
            End Get
        End Property

        Public Overloads Overrides Function GetProperty(Of T As Class)() As T
            Dim messageEncoderProperty As T = Me.MessageEncoderFactory.Encoder.GetProperty(Of T)()
            If messageEncoderProperty IsNot Nothing Then
                Return messageEncoderProperty
            End If

            If GetType(T) Is GetType(MessageVersion) Then
                Return CType(CObj(Me.MessageEncoderFactory.Encoder.MessageVersion), T)
            End If

            Return MyBase.GetProperty(Of T)()
        End Function

#Region "Lifecycle State Machine"
        ''' <summary>
        ''' Shutdown ungracefully
        ''' </summary>
        Protected Overloads Overrides Sub OnAbort()
            ' Abort can be called at anytime, so we can't assume that
            ' we've been Opened successfully (and thus may not have any listen sockets)
            SyncLock Me.ThisLock
                CloseListenSockets(TimeSpan.Zero)
                Me.channelQueue.Close()
            End SyncLock
        End Sub

        ''' <summary>
        ''' Shutdown gracefully
        ''' </summary>
        Protected Overloads Overrides Sub OnClose(ByVal timeout As TimeSpan)
            SyncLock Me.ThisLock
                CloseListenSockets(TimeSpan.Zero)
                Me.channelQueue.Close()
            End SyncLock
        End Sub

        Protected Overloads Overrides Function OnBeginClose(ByVal timeout As TimeSpan,
                                                            ByVal callback As AsyncCallback,
                                                            ByVal state As Object) As IAsyncResult
            Me.OnClose(timeout)
            Return New CompletedAsyncResult(callback, state)
        End Function

        Protected Overloads Overrides Sub OnEndClose(ByVal result As IAsyncResult)
            CompletedAsyncResult.End(result)
        End Sub

        Private Sub CloseListenSockets(ByVal timeout As TimeSpan)
            For i As Integer = 0 To listenSockets.Count - 1
                Me.listenSockets(i).Close(CInt(Fix(timeout.TotalMilliseconds)))
            Next i
            listenSockets.Clear()
        End Sub

        Protected Overloads Overrides Sub OnClosed()
            If Me.bufferManager IsNot Nothing Then
                Me.bufferManager.Clear()
            End If

            MyBase.OnClosed()
        End Sub

        ''' <summary>
        ''' Initialize any objects we're going to need for the opened factory
        ''' </summary>
        Protected Overloads Overrides Sub OnOpening()
            MyBase.OnOpening()
            Me.onReceive_local = New AsyncCallback(AddressOf Me.OnReceive)
        End Sub

        ''' <summary>
        ''' Open the listener factory for use. Ensures our UDP socket is bound
        ''' </summary>
        Protected Overloads Overrides Sub OnOpen(ByVal timeout As TimeSpan)
            If uri_local Is Nothing Then
                Throw New InvalidOperationException("Uri must be set before ChannelListener is opened.")
            End If

            If Me.listenSockets.Count = 0 Then
                If uri_local.HostNameType = UriHostNameType.IPv6 OrElse uri_local.HostNameType = UriHostNameType.IPv4 Then
                    listenSockets.Add(CreateListenSocket(IPAddress.Parse(uri_local.Host), uri_local.Port))
                Else
                    listenSockets.Add(CreateListenSocket(IPAddress.Any, uri_local.Port))
                    If Socket.OSSupportsIPv6 Then
                        listenSockets.Add(CreateListenSocket(IPAddress.IPv6Any, uri_local.Port))
                    End If
                End If
            End If
        End Sub

        Protected Overloads Overrides Function OnBeginOpen(ByVal timeout As TimeSpan, ByVal callback As AsyncCallback,
                                                           ByVal state As Object) As IAsyncResult
            Me.OnOpen(timeout)
            Return New CompletedAsyncResult(callback, state)
        End Function

        Protected Overloads Overrides Sub OnEndOpen(ByVal result As IAsyncResult)
            CompletedAsyncResult.End(result)
        End Sub

        ''' <summary>
        ''' Open has completed, start an asynchronous receive on our socket.
        ''' </summary>
        Protected Overloads Overrides Sub OnOpened()
            MyBase.OnOpened()
            Dim socketsSnapshot() As Socket = listenSockets.ToArray()
            Dim startReceivingCallback As New WaitCallback(AddressOf StartReceiving)
            For i = 0 To socketsSnapshot.Length - 1
                ThreadPool.QueueUserWorkItem(startReceivingCallback, socketsSnapshot(i))
            Next i
        End Sub
#End Region

        Private Function CreateListenSocket(ByVal ipAddress As IPAddress, ByVal port As Integer) As Socket
            Dim isIPv6 = (ipAddress.AddressFamily = AddressFamily.InterNetworkV6)
            Dim socket As Socket = Nothing

            If multicast Then
                Dim anyIPAddr As IPAddress = IPAddress.Any
                If isIPv6 Then
                    anyIPAddr = IPAddress.IPv6Any
                End If

                Dim endPoint As New IPEndPoint(anyIPAddr, port)
                socket = New Socket(endPoint.AddressFamily, SocketType.Dgram, ProtocolType.Udp)
                socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1)
                socket.Bind(endPoint)

                If isIPv6 Then
                    socket.SetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.AddMembership,
                                           New IPv6MulticastOption(ipAddress))
                Else
                    socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership,
                                           New MulticastOption(ipAddress))
                End If
            Else
                Dim endPoint As New IPEndPoint(ipAddress, port)
                socket = New Socket(endPoint.AddressFamily, SocketType.Dgram, ProtocolType.Udp)
                socket.Bind(endPoint)
            End If

            Return socket
        End Function

        Private Function CreateDummyEndPoint(ByVal socket As Socket) As EndPoint
            If socket.AddressFamily = AddressFamily.InterNetwork Then
                Return New IPEndPoint(IPAddress.Any, 0)
            Else
                Return New IPEndPoint(IPAddress.IPv6Any, 0)
            End If
        End Function

        Private Sub StartReceiving(ByVal state As Object)
            Dim listenSocket As Socket = CType(state, Socket)
            Dim result As IAsyncResult = Nothing

            Try
                SyncLock ThisLock
                    If MyBase.State = CommunicationState.Opened Then
                        Dim dummy As EndPoint = CreateDummyEndPoint(listenSocket)
                        Dim buffer() As Byte = Me.bufferManager.TakeBuffer(maxMessageSize)
                        result = listenSocket.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None,
                                                               dummy, Me.onReceive_local,
                                                               New SocketReceiveState(listenSocket, buffer))
                    End If
                End SyncLock

                If result IsNot Nothing AndAlso result.CompletedSynchronously Then
                    ContinueReceiving(result, listenSocket)
                End If
            Catch e As Exception
                Debug.WriteLine("Error in receiving from the socket.")
                Debug.WriteLine(e.ToString())
            End Try
        End Sub



        Private Sub ContinueReceiving(ByVal receiveResult As IAsyncResult, ByVal listenSocket As Socket)
            Dim continueReceiving = True

            Do While continueReceiving
                Dim receivedMessage As Message = Nothing

                If receiveResult IsNot Nothing Then
                    receivedMessage = EndReceive(listenSocket, receiveResult)
                    receiveResult = Nothing
                End If

                SyncLock ThisLock
                    If MyBase.State = CommunicationState.Opened Then
                        Dim dummy As EndPoint = CreateDummyEndPoint(listenSocket)
                        Dim buffer() As Byte = Me.bufferManager.TakeBuffer(maxMessageSize)

                        receiveResult = listenSocket.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None,
                                                                      dummy, Me.onReceive_local,
                                                                      New SocketReceiveState(listenSocket, buffer))

                    End If
                End SyncLock

                If receiveResult Is Nothing OrElse (Not receiveResult.CompletedSynchronously) Then
                    continueReceiving = False
                    Dispatch(receivedMessage)
                ElseIf receivedMessage IsNot Nothing Then
                    ThreadPool.QueueUserWorkItem(New WaitCallback(AddressOf DispatchCallback), receivedMessage)
                End If
            Loop
        End Sub

        Private Function EndReceive(ByVal listenSocket As Socket, ByVal result As IAsyncResult) As Message
            ' if we've started the shutdown process, then we've disposed
            ' the socket and calls to socket.EndReceive will throw .
            If MyBase.State <> CommunicationState.Opened Then
                Return Nothing
            End If

            Dim buffer() As Byte = (CType(result.AsyncState, SocketReceiveState)).Buffer

            Dim message As Message = Nothing

            Try
                Dim count = 0

                SyncLock ThisLock
                    ' if we've started the shutdown process, socket is disposed
                    ' and calls to socket.EndReceive will throw .
                    If MyBase.State = CommunicationState.Opened Then
                        Dim dummy As EndPoint = CreateDummyEndPoint(listenSocket)
                        count = listenSocket.EndReceiveFrom(result, dummy)
                    End If
                End SyncLock

                If count > 0 Then
                    Dim msg As ArraySegment(Of Byte) = Nothing
                    Dim transaction As Transaction = Nothing

                    ' read the transaction and message.
                    TransactionMessageBuffer.ReadTransactionMessageBuffer(buffer, count, transaction, msg)

                    Try
                        message = MessageEncoderFactory.Encoder.ReadMessage(msg, bufferManager)
                    Catch xmlException As XmlException
                        Throw New ProtocolException("There is a problem with the XML that was received from the network. See inner exception for more details.",
                                                    xmlException)
                    End Try

                    If transaction IsNot Nothing Then
                        ' This is where we set the transaction on the message in order to be picked up by 
                        ' the dispatcher, and used to call the service operation. We use the  
                        ' System.ServiceModel.Channels.TransactionMessageProperty provided by the WCF framework.
                        TransactionMessageProperty.Set(transaction, message)
                    End If
                End If
            Catch e As Exception
                Debug.WriteLine("Error in completing the async receive via EndReceiveFrom method.")
                Debug.WriteLine(e.ToString())
            Finally
                If message Is Nothing Then
                    Me.bufferManager.ReturnBuffer(buffer)
                    buffer = Nothing
                End If
            End Try

            Return message
        End Function

        'Called when an ansynchronous receieve operation completes
        'on the listening socket.
        Private Sub OnReceive(ByVal result As IAsyncResult)
            If result.CompletedSynchronously Then
                Return
            End If

            ContinueReceiving(result, (CType(result.AsyncState, SocketReceiveState)).Socket)
        End Sub

        Private Sub DispatchCallback(ByVal state As Object)
            Dispatch(CType(state, Message))
        End Sub

        ''' <summary>
        ''' Matches an incoming message to its waiting listener,
        ''' using the FilterTable to dispatch the message to the correc
        ''' listener. If no listener is waiting for the message, it is silently
        ''' discarded.
        ''' </summary>
        Private Sub Dispatch(ByVal message As Message)
            If message Is Nothing Then
                Return
            End If

            Try
                Dim newChannel As UdpInputChannel = Nothing
                Dim channelCreated As Boolean = CreateOrRetrieveChannel(newChannel)

                newChannel.Dispatch(message)

                If channelCreated Then
                    'Hand the channel off to whomever is waiting for AcceptChannel()
                    'to complete
                    Me.channelQueue.EnqueueAndDispatch(newChannel)
                End If
            Catch e As Exception
                Debug.WriteLine("Error dispatching Message.")
                Debug.WriteLine(e.ToString())
            End Try
        End Sub

        ''' <summary>
        ''' Used to get a unique uri (by CompositeDuplexChannelFactory for example).
        ''' We get a unique TCP port by binding to "port 0"
        ''' </summary>
        Public Sub InitializeUniqueUri(ByVal host As String)
            If host Is Nothing Then
                Throw New ArgumentNullException("host")
            End If

            Dim port As Integer

            SyncLock Me.ThisLock
                CloseListenSockets(TimeSpan.Zero)
                Dim ipAddress As IPAddress = Nothing
                If IPAddress.TryParse(host, ipAddress) Then
                    Dim socket As Socket = CreateListenSocket(ipAddress, 0)
                    port = (CType(socket.LocalEndPoint, IPEndPoint)).Port
                    listenSockets.Add(socket)
                Else
                    Dim socket As Socket = CreateListenSocket(IPAddress.Any, 0)
                    port = (CType(socket.LocalEndPoint, IPEndPoint)).Port
                    listenSockets.Add(socket)
                    If Socket.OSSupportsIPv6 Then
                        listenSockets.Add(CreateListenSocket(IPAddress.IPv6Any, port))
                    End If
                End If
            End SyncLock

            Dim uriBuilder As New UriBuilder(Scheme, host, port)
            InitializeUri(uriBuilder.Uri, String.Empty)
        End Sub


        Private Sub InitializeUri(ByVal baseAddress As Uri, ByVal relativeAddress As String, ByVal mode As ListenUriMode)
            Select Case mode
                Case ListenUriMode.Explicit
                    Me.InitializeUri(baseAddress, relativeAddress)
                Case ListenUriMode.Unique
                    'This listener sets unique uris using the host name only.
                    Me.InitializeUniqueUri(baseAddress.Host)
                    Exit Select
            End Select
        End Sub

        Public Sub InitializeUri(ByVal baseAddress As Uri, ByVal relativeAddress As String)
            If baseAddress Is Nothing Then
                Throw New ArgumentNullException("baseAddress")
            End If

            If relativeAddress Is Nothing Then
                Throw New ArgumentNullException("relativeAddress")
            End If

            If Not baseAddress.IsAbsoluteUri Then
                Throw New ArgumentException(String.Format(CultureInfo.CurrentCulture,
                                                          "Base address must be an absolute URI."),
                                                      "baseAddress")
            End If

            If baseAddress.Scheme <> Me.Scheme Then
                ' URI schemes are case-insensitive, so try a case insensitive compare now.
                If String.Compare(baseAddress.Scheme, Me.Scheme, True,
                                  System.Globalization.CultureInfo.InvariantCulture) <> 0 Then
                    Throw New ArgumentException(String.Format(CultureInfo.CurrentCulture,
                                                              "Invalid URI scheme: {0}.",
                                                              baseAddress.Scheme),
                                                          "baseAddress")
                End If
            End If

            Dim fullUri As Uri = baseAddress

            ' Ensure that baseAddress Path does end with a slash if we have a relative address.
            If relativeAddress <> String.Empty Then
                If Not baseAddress.AbsolutePath.EndsWith("/") Then
                    Dim uriBuilder As New UriBuilder(baseAddress)
                    uriBuilder.Path = uriBuilder.Path & "/"
                    baseAddress = uriBuilder.Uri
                End If

                fullUri = New Uri(baseAddress, relativeAddress)
            End If

            SyncLock MyBase.ThisLock
                ThrowIfDisposedOrImmutable()
                Me.uri_local = fullUri
                CloseListenSockets(TimeSpan.Zero)
            End SyncLock
        End Sub

        'Synchronously returns a channel that is attached to this listener.
        Protected Overrides Function OnAcceptChannel(ByVal timeout As TimeSpan) As IInputChannel
            UdpChannelHelpers.ValidateTimeout(timeout)
            If Not Me.IsDisposed Then
                Me.EnsureChannelAvailable()
            End If

            Dim channel As IInputChannel = Nothing
            If Me.channelQueue.Dequeue(timeout, channel) Then
                Return channel
            Else
                Throw CreateAcceptTimeoutException(timeout)
            End If
        End Function

        Private Function CreateAcceptTimeoutException(ByVal timeout As TimeSpan) As TimeoutException
            Return New TimeoutException(String.Format("Accept on listener at address {0} timed out after {1}.",
                                                      Me.Uri.AbsoluteUri, timeout))
        End Function

        Protected Overrides Function OnBeginAcceptChannel(ByVal timeout As TimeSpan,
                                                          ByVal callback As AsyncCallback,
                                                          ByVal state As Object) As IAsyncResult
            UdpChannelHelpers.ValidateTimeout(timeout)
            If Not Me.IsDisposed Then
                Me.EnsureChannelAvailable()
            End If

            Return Me.channelQueue.BeginDequeue(timeout, callback, state)
        End Function

        Protected Overrides Function OnEndAcceptChannel(ByVal result As IAsyncResult) As IInputChannel
            Dim channel As IInputChannel = Nothing
            If Me.channelQueue.EndDequeue(result, channel) Then
                Return channel
            Else
                Throw New TimeoutException()
            End If
        End Function

        Protected Overloads Overrides Function OnWaitForChannel(ByVal timeout As TimeSpan) As Boolean
            UdpChannelHelpers.ValidateTimeout(timeout)
            Return Me.channelQueue.WaitForItem(timeout)
        End Function

        Protected Overloads Overrides Function OnBeginWaitForChannel(ByVal timeout As TimeSpan,
                                                                     ByVal callback As AsyncCallback,
                                                                     ByVal state As Object) As IAsyncResult
            UdpChannelHelpers.ValidateTimeout(timeout)
            Return Me.channelQueue.BeginWaitForItem(timeout, callback, state)
        End Function

        Protected Overloads Overrides Function OnEndWaitForChannel(ByVal result As IAsyncResult) As Boolean
            Return Me.channelQueue.EndWaitForItem(result)
        End Function

        'Guarantees that channel is attached to this listener.
        Private Sub EnsureChannelAvailable()
            Dim newChannel As UdpInputChannel = Nothing
            Dim channelCreated = CreateOrRetrieveChannel(newChannel)

            If channelCreated Then
                Me.channelQueue.EnqueueAndDispatch(newChannel)
            End If
        End Sub

        Private Function CreateOrRetrieveChannel(<System.Runtime.InteropServices.Out()> ByRef newChannel As UdpInputChannel) As Boolean
            Dim channelCreated = False

            newChannel = currentChannel
            If newChannel Is Nothing Then
                SyncLock currentChannelLock
                    newChannel = currentChannel
                    If newChannel Is Nothing Then
                        newChannel = New UdpInputChannel(Me)
                        AddHandler newChannel.Closed, AddressOf OnChannelClosed
                        currentChannel = newChannel
                        channelCreated = True
                    End If
                End SyncLock
            End If

            Return channelCreated
        End Function

        Private Sub OnChannelClosed(ByVal sender As Object, ByVal args As EventArgs)
            Dim channel As UdpInputChannel = CType(sender, UdpInputChannel)

            SyncLock Me.currentChannelLock
                If channel Is Me.currentChannel Then
                    Me.currentChannel = Nothing
                End If
            End SyncLock
        End Sub

        Private Class SocketReceiveState

            Private socket_local As Socket
            Private buffer_local() As Byte
            Public Sub New(ByVal socket As Socket, ByVal buffer() As Byte)
                If socket Is Nothing Then
                    Throw New ArgumentNullException("socket")
                End If
                If buffer Is Nothing Then
                    Throw New ArgumentNullException("buffer")
                End If

                Me.socket_local = socket
                Me.buffer_local = buffer
            End Sub

            Public ReadOnly Property Socket() As Socket
                Get
                    Return Me.socket_local
                End Get
            End Property

            Public ReadOnly Property Buffer() As Byte()
                Get
                    Return Me.buffer_local
                End Get
            End Property
        End Class
	End Class
End Namespace
