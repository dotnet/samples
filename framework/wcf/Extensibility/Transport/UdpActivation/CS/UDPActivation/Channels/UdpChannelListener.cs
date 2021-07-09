
//  Copyright (c) Microsoft Corporation.  All Rights Reserved.


namespace Microsoft.ServiceModel.Samples
{
    #region using
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.Net;
    using System.Net.Sockets;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Description;
    #endregion

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

        Uri uri;

        InputQueue<IInputChannel> channelQueue;

        //The channel associated with this listener.
        UdpInputChannel currentChannel;

        object currentChannelLock;

        long maxBufferPoolSize;
        string virtualPath;

        internal UdpChannelListener(UdpTransportBindingElement bindingElement, BindingContext context)
            : base(context.Binding)
        {
            this.maxMessageSize = (int)bindingElement.MaxReceivedMessageSize;
            this.multicast = bindingElement.Multicast;
            this.bufferManager = BufferManager.CreateBufferManager(bindingElement.MaxBufferPoolSize, this.maxMessageSize);
            this.maxBufferPoolSize = bindingElement.MaxBufferPoolSize;

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
                // FUTURE: Need to unregister the channel listener from the TransportManager
            }
        }

        /// <summary>
        /// Shutdown gracefully
        /// </summary>
        protected override void OnClose(TimeSpan timeout)
        {
            lock (this.ThisLock)
            {
                // FUTURE: Need to unregister the channel listener from the TransportManager
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

            UdpTransportManager transportManager = UdpTransportManager.LookUp(this);
            if (transportManager == null)
            {
                lock (UdpTransportManager.SyncRoot)
                {
                    transportManager = UdpTransportManager.LookUp(this);
                    if (transportManager == null)
                    {
                        // This is non-activation case. We create a new TransportManager
                        if (this.listenSockets != null)
                        {
                            transportManager = new ExclusiveUdpTransportManager(
                                this.Uri,
                                this.listenSockets,
                                (int)this.maxBufferPoolSize,
                                this.maxMessageSize);
                        }
                        else
                        {
                            transportManager = new ExclusiveUdpTransportManager(
                                this.Uri,
                                this.multicast,
                                (int)this.maxBufferPoolSize,
                                this.maxMessageSize);
                        }

                        ((ExclusiveUdpTransportManager)transportManager).Open();
                        UdpTransportManager.Add(transportManager);
                    }
                }
            }

            // FUTURE: Should check whether the settings of the found transport manager matches
            // the channel listener.

            transportManager.Register(this);
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
        }
        #endregion

        /// <summary>
        /// Matches an incoming message to its waiting listener,
        /// using the FilterTable to dispatch the message to the correc
        /// listener. If no listener is waiting for the message, it is silently
        /// discarded.
        /// </summary>
        public void OnRawMessageReceived(byte[] rawMessage)
        {
            if (rawMessage == null)
                return;

            Message message = DecodeMessage(rawMessage);

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

            this.listenSockets = new List<Socket>();
            int port;

            lock (this.ThisLock)
            {
                IPAddress ipAddress = null;
                if (IPAddress.TryParse(host, out ipAddress))
                {
                    Socket socket = UdpSocketListener.CreateListenSocket(
                        ipAddress, 0, this.multicast);

                    port = ((IPEndPoint)socket.LocalEndPoint).Port;
                    listenSockets.Add(socket);
                }
                else
                {
                    Socket socket = UdpSocketListener.CreateListenSocket(
                        IPAddress.Any, 0, this.multicast);
                    port = ((IPEndPoint)socket.LocalEndPoint).Port;
                    listenSockets.Add(socket);

                    if (Socket.OSSupportsIPv6)
                    {
                        listenSockets.Add(UdpSocketListener.CreateListenSocket(
                            IPAddress.IPv6Any, port, this.multicast));
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

        internal void SetVirtualPath(string virtualPath)
        {
            // We can use virtual path to check settings for hosted environment.
            this.virtualPath = virtualPath;
        }

        Message DecodeMessage(byte[] rawMessage)
        {
            byte[] buffer = this.bufferManager.TakeBuffer(rawMessage.Length);
            Buffer.BlockCopy(rawMessage, 0, buffer, 0, rawMessage.Length);

            return MessageEncoderFactory.Encoder.ReadMessage(new ArraySegment<byte>(buffer, 0, rawMessage.Length), this.bufferManager);
        }
    }
}

