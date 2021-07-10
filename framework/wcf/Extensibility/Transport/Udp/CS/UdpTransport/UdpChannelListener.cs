//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.Threading;
using System.Xml;

namespace Microsoft.Samples.Udp
{
    /// <summary>
    /// IListenerFactory implementation for Udp.
    /// 
    /// Supports IInputChannel only, as Udp is fundamentally
    /// a datagram protocol. Uses a listening UDP socket for each
    /// Factory instance, then demuxes to the appropriate IInputListener
    /// based on the registered filters. More advanced implementations could factor
    /// out the listening socket and demux from this listening object
    /// to separate factories based on a framing Via.
    /// </summary>
    class UdpChannelListener : ChannelListenerBase<IInputChannel>
    {
        BufferManager bufferManager;

        //The UDP network sockets.
        List<Socket> listenSockets;

        int maxMessageSize;
        MessageEncoderFactory messageEncoderFactory;
        bool multicast;

        AsyncCallback onReceive;
        Uri uri;

        InputQueue<IInputChannel> channelQueue;

        //The channel associated with this listener.
        UdpInputChannel currentChannel;

        object currentChannelLock;

        internal UdpChannelListener(UdpTransportBindingElement bindingElement, BindingContext context)
            : base(context.Binding)
        {
            this.maxMessageSize = (int)bindingElement.MaxReceivedMessageSize;
            this.multicast = bindingElement.Multicast;
            this.bufferManager = BufferManager.CreateBufferManager(bindingElement.MaxBufferPoolSize, this.maxMessageSize);
            MessageEncodingBindingElement messageEncoderBindingElement = context.BindingParameters.Remove<MessageEncodingBindingElement>();
            if (messageEncoderBindingElement != null)
            {
                this.messageEncoderFactory = messageEncoderBindingElement.CreateMessageEncoderFactory();
            }
            else
            {
                this.messageEncoderFactory = UdpConstants.DefaultMessageEncoderFactory;
            }
            this.channelQueue = new InputQueue<IInputChannel>();
            this.currentChannelLock = new object();
            this.listenSockets = new List<Socket>(2);

            Uri baseAddress = context.ListenUriBaseAddress;
            if (baseAddress == null)
            {
                if (context.ListenUriMode == ListenUriMode.Unique)
                {
                    UriBuilder uriBuilder = new UriBuilder(this.Scheme, Dns.GetHostEntry(String.Empty).HostName);
                    uriBuilder.Path = Guid.NewGuid().ToString();
                    baseAddress = uriBuilder.Uri;
                }
                else
                {
                    throw new InvalidOperationException(
                        "Null is only a supported value for ListenUriBaseAddress when using ListenUriMode.Unique.");
                }
            }

            this.InitializeUri(baseAddress, context.ListenUriRelativeAddress, context.ListenUriMode);
        }

        internal TimeSpan InternalReceiveTimeout
        {
            get { return this.DefaultReceiveTimeout; }
        }

        public MessageEncoderFactory MessageEncoderFactory
        {
            get
            {
                return messageEncoderFactory;
            }
        }

        string Scheme
        {
            get
            {
                return UdpConstants.Scheme;
            }
        }

        public override Uri Uri
        {
            get
            {
                return this.uri;
            }
        }

        public override T GetProperty<T>()
        {
            T messageEncoderProperty = this.MessageEncoderFactory.Encoder.GetProperty<T>();
            if (messageEncoderProperty != null)
            {
                return messageEncoderProperty;
            }
            
            if (typeof(T) == typeof(MessageVersion))
            {
                return (T)(object)this.MessageEncoderFactory.Encoder.MessageVersion;
            }

            return base.GetProperty<T>();
        }

        #region Lifecycle State Machine
        /// <summary>
        /// Shutdown ungracefully
        /// </summary>
        protected override void OnAbort()
        {
            // Abort can be called at anytime, so we can't assume that
            // we've been Opened successfully (and thus may not have any listen sockets)
            lock (this.ThisLock)
            {
                CloseListenSockets(TimeSpan.Zero);
                this.channelQueue.Close();
            }
        }

        /// <summary>
        /// Shutdown gracefully
        /// </summary>
        protected override void OnClose(TimeSpan timeout)
        {
            lock (this.ThisLock)
            {
                CloseListenSockets(TimeSpan.Zero);
                this.channelQueue.Close();
            }
        }

        protected override IAsyncResult OnBeginClose(TimeSpan timeout, AsyncCallback callback, object state)
        {
            this.OnClose(timeout);
            return new CompletedAsyncResult(callback, state);
        }

        protected override void OnEndClose(IAsyncResult result)
        {
            CompletedAsyncResult.End(result);
        }

        void CloseListenSockets(TimeSpan timeout)
        {
            for (int i = 0; i < listenSockets.Count; i++)
            {
                this.listenSockets[i].Close((int)timeout.TotalMilliseconds);
            }
            listenSockets.Clear();
        }

        protected override void OnClosed()
        {
            if (this.bufferManager != null)
            {
                this.bufferManager.Clear();
            }

            base.OnClosed();
        }

        /// <summary>
        /// Initialize any objects we're going to need for the opened factory
        /// </summary>
        protected override void OnOpening()
        {
            base.OnOpening();
            this.onReceive = new AsyncCallback(this.OnReceive);
        }

        /// <summary>
        /// Open the listener factory for use. Ensures our UDP socket is bound
        /// </summary>
        protected override void OnOpen(TimeSpan timeout)
        {
            if (uri == null)
            {
                throw new InvalidOperationException("Uri must be set before ChannelListener is opened.");
            }

            if (this.listenSockets.Count == 0)
            {
                if (uri.HostNameType == UriHostNameType.IPv6 ||
                    uri.HostNameType == UriHostNameType.IPv4)
                {
                    listenSockets.Add(CreateListenSocket(IPAddress.Parse(uri.Host), uri.Port));
                }
                else
                {
                    listenSockets.Add(CreateListenSocket(IPAddress.Any, uri.Port));
                    if (Socket.OSSupportsIPv6)
                    {
                        listenSockets.Add(CreateListenSocket(IPAddress.IPv6Any, uri.Port));
                    }
                }
            }
        }

        protected override IAsyncResult OnBeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
        {
            this.OnOpen(timeout);
            return new CompletedAsyncResult(callback, state);
        }

        protected override void OnEndOpen(IAsyncResult result)
        {
            CompletedAsyncResult.End(result);
        }

        /// <summary>
        /// Open has completed, start an asynchronous receive on our socket.
        /// </summary>
        protected override void OnOpened()
        {
            base.OnOpened();
            Socket[] socketsSnapshot = listenSockets.ToArray();
            WaitCallback startReceivingCallback = new WaitCallback(StartReceiving);
            for (int i = 0; i < socketsSnapshot.Length; i++)
            {
                ThreadPool.QueueUserWorkItem(startReceivingCallback, socketsSnapshot[i]);
            }
        }
        #endregion

        Socket CreateListenSocket(IPAddress ipAddress, int port)
        {
            bool isIPv6 = (ipAddress.AddressFamily == AddressFamily.InterNetworkV6);
            Socket socket = null;

            if (multicast)
            {
                IPAddress anyIPAddr = IPAddress.Any;
                if (isIPv6)
                    anyIPAddr = IPAddress.IPv6Any;

                IPEndPoint endPoint = new IPEndPoint(anyIPAddr, port);
                socket = new Socket(endPoint.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
                socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);
                socket.Bind(endPoint);

                if (isIPv6)
                {
                    socket.SetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.AddMembership,
                        new IPv6MulticastOption(ipAddress));
                }
                else
                {
                    socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership,
                        new MulticastOption(ipAddress));
                }
            }
            else
            {
                IPEndPoint endPoint = new IPEndPoint(ipAddress, port);
                socket = new Socket(endPoint.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
                socket.Bind(endPoint);
            }

            return socket;
        }

        EndPoint CreateDummyEndPoint(Socket socket)
        {
            if (socket.AddressFamily == AddressFamily.InterNetwork)
            {
                return new IPEndPoint(IPAddress.Any, 0);
            }
            else
            {
                return new IPEndPoint(IPAddress.IPv6Any, 0);
            }
        }

        void StartReceiving(object state)
        {
            Socket listenSocket = (Socket)state;
            IAsyncResult result = null;

            try
            {
                lock (ThisLock)
                {
                    if (base.State == CommunicationState.Opened)
                    {
                        EndPoint dummy = CreateDummyEndPoint(listenSocket);
                        byte[] buffer = this.bufferManager.TakeBuffer(maxMessageSize);
                        result = listenSocket.BeginReceiveFrom(buffer, 0, buffer.Length,
                            SocketFlags.None, ref dummy, this.onReceive, new SocketReceiveState(listenSocket, buffer));
                    }
                }

                if (result != null && result.CompletedSynchronously)
                {
                    ContinueReceiving(result, listenSocket);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error in receiving from the socket.");
                Debug.WriteLine(e.ToString());
            }
        }

        void ContinueReceiving(IAsyncResult receiveResult, Socket listenSocket)
        {
            bool continueReceiving = true;

            while (continueReceiving)
            {
                Message receivedMessage = null;

                if (receiveResult != null)
                {
                    receivedMessage = EndReceive(listenSocket, receiveResult);
                    receiveResult = null;
                }

                lock (ThisLock)
                {
                    if (base.State == CommunicationState.Opened)
                    {
                        EndPoint dummy = CreateDummyEndPoint(listenSocket);
                        byte[] buffer = this.bufferManager.TakeBuffer(maxMessageSize);
                        receiveResult = listenSocket.BeginReceiveFrom(buffer, 0, buffer.Length,
                            SocketFlags.None, ref dummy, this.onReceive, new SocketReceiveState(listenSocket, buffer));
                    }
                }

                if (receiveResult == null || !receiveResult.CompletedSynchronously)
                {
                    continueReceiving = false;
                    Dispatch(receivedMessage);
                }
                else if (receivedMessage != null)
                {
                    ThreadPool.QueueUserWorkItem(new WaitCallback(DispatchCallback), receivedMessage);
                }
            }
        }

        Message EndReceive(Socket listenSocket, IAsyncResult result)
        {
            // if we've started the shutdown process, then we've disposed
            // the socket and calls to socket.EndReceive will throw 
            if (base.State != CommunicationState.Opened)
                return null;

            byte[] buffer = ((SocketReceiveState)result.AsyncState).Buffer;
            Debug.Assert(buffer != null);
            Message message = null;

            try
            {
                int count = 0;

                lock (ThisLock)
                {
                    // if we've started the shutdown process, socket is disposed
                    // and calls to socket.EndReceive will throw 
                    if (base.State == CommunicationState.Opened)
                    {
                        EndPoint dummy = CreateDummyEndPoint(listenSocket);
                        count = listenSocket.EndReceiveFrom(result, ref dummy);
                    }
                }

                if (count > 0)
                {
                    try
                    {
                        message = MessageEncoderFactory.Encoder.ReadMessage(new ArraySegment<byte>(buffer, 0, count), bufferManager);
                    }
                    catch (XmlException xmlException)
                    {
                        throw new ProtocolException(
                            "There is a problem with the XML that was received from the network. See inner exception for more details.", 
                            xmlException);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error in completing the async receive via EndReceiveFrom method.");
                Debug.WriteLine(e.ToString());
            }
            finally
            {
                if (message == null)
                {
                    this.bufferManager.ReturnBuffer(buffer);
                    buffer = null;
                }
            }

            return message;
        }

        //Called when an ansynchronous receieve operation completes
        //on the listening socket.
        void OnReceive(IAsyncResult result)
        {
            if (result.CompletedSynchronously)
                return;

            ContinueReceiving(result, ((SocketReceiveState)result.AsyncState).Socket);
        }

        void DispatchCallback(object state)
        {
            Dispatch((Message)state);
        }

        /// <summary>
        /// Matches an incoming message to its waiting listener,
        /// using the FilterTable to dispatch the message to the correc
        /// listener. If no listener is waiting for the message, it is silently
        /// discarded.
        /// </summary>
        void Dispatch(Message message)
        {
            if (message == null)
                return;

            try
            {
                UdpInputChannel newChannel;
                bool channelCreated = CreateOrRetrieveChannel(out newChannel);

                newChannel.Dispatch(message);

                if (channelCreated)
                {
                    //Hand the channel off to whomever is waiting for AcceptChannel()
                    //to complete
                    this.channelQueue.EnqueueAndDispatch(newChannel);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error dispatching Message.");
                Debug.WriteLine(e.ToString());
            }
        }

        /// <summary>
        /// Used to get a unique uri (by CompositeDuplexChannelFactory for example).
        /// We get a unique TCP port by binding to "port 0"
        /// </summary>
        public void InitializeUniqueUri(string host)
        {
            if (host == null)
            {
                throw new ArgumentNullException("host");
            }

            int port;

            lock (this.ThisLock)
            {
                CloseListenSockets(TimeSpan.Zero);
                IPAddress ipAddress = null;
                if (IPAddress.TryParse(host, out ipAddress))
                {
                    Socket socket = CreateListenSocket(ipAddress, 0);
                    port = ((IPEndPoint)socket.LocalEndPoint).Port;
                    listenSockets.Add(socket);
                }
                else
                {
                    Socket socket = CreateListenSocket(IPAddress.Any, 0);
                    port = ((IPEndPoint)socket.LocalEndPoint).Port;
                    listenSockets.Add(socket);
                    if (Socket.OSSupportsIPv6)
                    {
                        listenSockets.Add(CreateListenSocket(IPAddress.IPv6Any, port));
                    }
                }
            }

            UriBuilder uriBuilder = new UriBuilder(Scheme, host, port);
            InitializeUri(uriBuilder.Uri, String.Empty);
        }


        void InitializeUri(Uri baseAddress, string relativeAddress, ListenUriMode mode)
        {
            switch (mode)
            {
                case ListenUriMode.Explicit:
                    this.InitializeUri(baseAddress, relativeAddress);
                    break;
                case ListenUriMode.Unique:
                    {
                        //This listener sets unique uris using the host name only.
                        this.InitializeUniqueUri(baseAddress.Host);
                        break;
                    }
            }
        }

        public void InitializeUri(Uri baseAddress, string relativeAddress)
        {
            if (baseAddress == null)
                throw new ArgumentNullException("baseAddress");

            if (relativeAddress == null)
                throw new ArgumentNullException("relativeAddress");

            if (!baseAddress.IsAbsoluteUri)
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture,
                    "Base address must be an absolute URI."), "baseAddress");

            if (baseAddress.Scheme != this.Scheme)
            {
                // URI schemes are case-insensitive, so try a case insensitive compare now
                if (string.Compare(baseAddress.Scheme, this.Scheme, true, System.Globalization.CultureInfo.InvariantCulture) != 0)
                {
                    throw new ArgumentException(string.Format(CultureInfo.CurrentCulture,
                        "Invalid URI scheme: {0}.", baseAddress.Scheme), "baseAddress");
                }
            }

            Uri fullUri = baseAddress;

            // Ensure that baseAddress Path does end with a slash if we have a relative address
            if (relativeAddress != string.Empty)
            {
                if (!baseAddress.AbsolutePath.EndsWith("/"))
                {
                    UriBuilder uriBuilder = new UriBuilder(baseAddress);
                    uriBuilder.Path = uriBuilder.Path + "/";
                    baseAddress = uriBuilder.Uri;
                }

                fullUri = new Uri(baseAddress, relativeAddress);
            }

            lock (base.ThisLock)
            {
                ThrowIfDisposedOrImmutable();
                this.uri = fullUri;
                CloseListenSockets(TimeSpan.Zero);
            }
        }

        //Synchronously returns a channel that is attached to this listener.
        protected override IInputChannel OnAcceptChannel(TimeSpan timeout)
        {
            UdpChannelHelpers.ValidateTimeout(timeout);
            if (!this.IsDisposed)
            {
                this.EnsureChannelAvailable();
            }

            IInputChannel channel;
            if (this.channelQueue.Dequeue(timeout, out channel))
            {
                return channel;
            }
            else
            {
                throw CreateAcceptTimeoutException(timeout);
            }
        }

        TimeoutException CreateAcceptTimeoutException(TimeSpan timeout)
        {
            return new TimeoutException(
                string.Format("Accept on listener at address {0} timed out after {1}.",
                this.Uri.AbsoluteUri, timeout));
        }

        protected override IAsyncResult OnBeginAcceptChannel(TimeSpan timeout, AsyncCallback callback, object state)
        {
            UdpChannelHelpers.ValidateTimeout(timeout);
            if (!this.IsDisposed)
            {
                this.EnsureChannelAvailable();
            }

            return this.channelQueue.BeginDequeue(timeout, callback, state);
        }

        protected override IInputChannel OnEndAcceptChannel(IAsyncResult result)
        {
            IInputChannel channel;
            if (this.channelQueue.EndDequeue(result, out channel))
            {
                return channel;
            }
            else
            {
                throw new TimeoutException();
            }
        }

        protected override bool OnWaitForChannel(TimeSpan timeout)
        {
            UdpChannelHelpers.ValidateTimeout(timeout);
            return this.channelQueue.WaitForItem(timeout);
        }

        protected override IAsyncResult OnBeginWaitForChannel(TimeSpan timeout, AsyncCallback callback, object state)
        {
            UdpChannelHelpers.ValidateTimeout(timeout);
            return this.channelQueue.BeginWaitForItem(timeout, callback, state);
        }

        protected override bool OnEndWaitForChannel(IAsyncResult result)
        {
            return this.channelQueue.EndWaitForItem(result);
        }

        //Guarantees that channel is attached to this listener.
        void EnsureChannelAvailable()
        {
            UdpInputChannel newChannel;
            bool channelCreated = CreateOrRetrieveChannel(out newChannel);

            if (channelCreated)
            {
                this.channelQueue.EnqueueAndDispatch(newChannel);
            }
        }

        bool CreateOrRetrieveChannel(out UdpInputChannel newChannel)
        {
            bool channelCreated = false;

            if ((newChannel = currentChannel) == null)
            {
                lock (currentChannelLock)
                {
                    if ((newChannel = currentChannel) == null)
                    {
                        newChannel = new UdpInputChannel(this);
                        newChannel.Closed += new EventHandler(this.OnChannelClosed);
                        currentChannel = newChannel;
                        channelCreated = true;
                    }
                }
            }

            return channelCreated;
        }

        void OnChannelClosed(object sender, EventArgs args)
        {
            UdpInputChannel channel = (UdpInputChannel)sender;

            lock (this.currentChannelLock)
            {
                if (channel == this.currentChannel)
                {
                    this.currentChannel = null;
                }
            }
        }

        class SocketReceiveState
        {
            Socket socket;
            byte[] buffer;
            public SocketReceiveState(Socket socket, byte[] buffer)
            {
                this.socket = socket;
                this.buffer = buffer;
            }

            public Socket Socket
            {
                get { return this.socket; }
            }

            public byte[] Buffer
            {
                get { return this.buffer; }
            }
        }
    }
}
