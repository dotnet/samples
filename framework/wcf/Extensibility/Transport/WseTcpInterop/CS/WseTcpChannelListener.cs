//  Copyright (c) Microsoft Corporation.  All Rights Reserved.

using System;
using System.Collections.ObjectModel;
using System.Net;
using System.Net.Sockets;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Microsoft.Samples.WseTcpTransport
{
    class WseTcpChannelListener : ChannelListenerBase<IDuplexSessionChannel>
    {
        BufferManager bufferManager;
        MessageEncoderFactory encoderFactory;
        Socket listenSocket;
        Uri uri;

        public WseTcpChannelListener(WseTcpTransportBindingElement bindingElement, BindingContext context)
            : base(context.Binding)
        {
            // populate members from binding element
            int maxBufferSize = (int)bindingElement.MaxReceivedMessageSize;
            this.bufferManager = BufferManager.CreateBufferManager(bindingElement.MaxBufferPoolSize, maxBufferSize);

            Collection<MessageEncodingBindingElement> messageEncoderBindingElements
                = context.BindingParameters.FindAll<MessageEncodingBindingElement>();

            if (messageEncoderBindingElements.Count > 1)
            {
                throw new InvalidOperationException("More than one MessageEncodingBindingElement was found in the BindingParameters of the BindingContext");
            }
            else if (messageEncoderBindingElements.Count == 1)
            {
                this.encoderFactory = messageEncoderBindingElements[0].CreateMessageEncoderFactory();
            }
            else
            {
                this.encoderFactory = new MtomMessageEncodingBindingElement().CreateMessageEncoderFactory();
            }

            this.uri = new Uri(context.ListenUriBaseAddress, context.ListenUriRelativeAddress);
        }

        public override Uri Uri
        {
            get { return this.uri; }
        }

        void OpenListenSocket()
        {
            if (uri == null)
            {
                throw new InvalidOperationException("SetUri must be called before opening this Channel Listener");
            }

            IPEndPoint localEndpoint = new IPEndPoint(IPAddress.Any, uri.Port);
            this.listenSocket = new Socket(localEndpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            this.listenSocket.Bind(localEndpoint);
            this.listenSocket.Listen(10);
        }

        void CloseListenSocket(TimeSpan timeout)
        {
            this.listenSocket.Close((int)timeout.TotalMilliseconds);
        }

        protected override void OnOpen(TimeSpan timeout)
        {
            OpenListenSocket();
        }

        protected override IAsyncResult OnBeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
        {
            OpenListenSocket();
            return new CompletedAsyncResult(callback, state);
        }

        protected override void OnEndOpen(IAsyncResult result)
        {
            CompletedAsyncResult.End(result);
        }

        protected override void OnAbort()
        {
            CloseListenSocket(TimeSpan.Zero);
        }

        protected override void OnClose(TimeSpan timeout)
        {
            CloseListenSocket(timeout);
        }

        protected override IAsyncResult OnBeginClose(TimeSpan timeout, AsyncCallback callback, object state)
        {
            CloseListenSocket(timeout);
            return new CompletedAsyncResult(callback, state);
        }

        protected override void OnEndClose(IAsyncResult result)
        {
            CompletedAsyncResult.End(result);
        }

        protected override IDuplexSessionChannel OnAcceptChannel(TimeSpan timeout)
        {
            Socket dataSocket = listenSocket.Accept();
            return new ServerTcpDuplexSessionChannel(this.encoderFactory, this.bufferManager, dataSocket, new EndpointAddress(uri), this);
        }

        protected override IAsyncResult OnBeginAcceptChannel(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return new AcceptChannelAsyncResult(timeout, this, callback, state);
        }

        protected override IDuplexSessionChannel OnEndAcceptChannel(IAsyncResult result)
        {
            return AcceptChannelAsyncResult.End(result);
        }

        protected override bool OnWaitForChannel(TimeSpan timeout)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        protected override IAsyncResult OnBeginWaitForChannel(TimeSpan timeout, AsyncCallback callback, object state)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        protected override bool OnEndWaitForChannel(IAsyncResult result)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        class ServerTcpDuplexSessionChannel : WseTcpDuplexSessionChannel
        {
            public ServerTcpDuplexSessionChannel(MessageEncoderFactory messageEncoderFactory, BufferManager bufferManager,
                Socket socket, EndpointAddress localAddress, ChannelManagerBase channelManager)
                : base(messageEncoderFactory, bufferManager, WseTcpDuplexSessionChannel.AnonymousAddress, localAddress, 
                WseTcpDuplexSessionChannel.AnonymousAddress.Uri, channelManager)
            {
                base.InitializeSocket(socket);
            }
        }

        class AcceptChannelAsyncResult : AsyncResult
        {
            TimeSpan timeout;
            WseTcpChannelListener listener;
            IDuplexSessionChannel channel;

            static AsyncCallback acceptCallback = new AsyncCallback(OnAccept);

            public AcceptChannelAsyncResult(TimeSpan timeout, WseTcpChannelListener listener, AsyncCallback callback, object state)
                : base(callback, state)
            {
                this.timeout = timeout;
                this.listener = listener;

                IAsyncResult acceptResult = listener.listenSocket.BeginAccept(acceptCallback, this);
                if (!acceptResult.CompletedSynchronously)
                {
                    return;
                }

                if (CompleteAccept(acceptResult))
                {
                    base.Complete(true);
                }
            }

            bool CompleteAccept(IAsyncResult result)
            {
                Socket dataSocket = listener.listenSocket.EndAccept(result);
                channel = new ServerTcpDuplexSessionChannel(this.listener.encoderFactory, this.listener.bufferManager, dataSocket, new EndpointAddress(this.listener.uri), this.listener);
                return true;
            }

            static void OnAccept(IAsyncResult result)
            {
                if (result.CompletedSynchronously)
                {
                    return;
                }

                AcceptChannelAsyncResult thisPtr = (AcceptChannelAsyncResult)result.AsyncState;

                Exception completionException = null;
                bool completeSelf = false;
                try
                {
                    completeSelf = thisPtr.CompleteAccept(result);
                }
                catch (Exception e)
                {
                    completeSelf = true;
                    completionException = e;
                }

                if (completeSelf)
                {
                    thisPtr.Complete(false, completionException);
                }
            }

            public static IDuplexSessionChannel End(IAsyncResult result)
            {
                AcceptChannelAsyncResult thisPtr = AsyncResult.End<AcceptChannelAsyncResult>(result);
                return thisPtr.channel;
            }
        }
    }
}
