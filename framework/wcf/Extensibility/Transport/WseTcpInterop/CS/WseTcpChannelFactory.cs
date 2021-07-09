//  Copyright (c) Microsoft Corporation.  All Rights Reserved.

using System;
using System.Collections.ObjectModel;
using System.Net;
using System.Net.Sockets;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Microsoft.Samples.WseTcpTransport
{
    class WseTcpChannelFactory : ChannelFactoryBase<IDuplexSessionChannel>
    {
        BufferManager bufferManager;
        MessageEncoderFactory encoderFactory;

        public WseTcpChannelFactory(WseTcpTransportBindingElement bindingElement, BindingContext context)
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
        }

        protected override IDuplexSessionChannel OnCreateChannel(EndpointAddress remoteAddress, Uri via)
        {
            return new ClientWseTcpDuplexSessionChannel(encoderFactory, bufferManager, remoteAddress, via, this);
        }

        protected override IAsyncResult OnBeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return new CompletedAsyncResult(callback, state);
        }

        protected override void OnEndOpen(IAsyncResult result)
        {
            CompletedAsyncResult.End(result);
        }

        protected override void OnOpen(TimeSpan timeout)
        {
        }

        class ClientWseTcpDuplexSessionChannel : WseTcpDuplexSessionChannel
        {
            public ClientWseTcpDuplexSessionChannel(
                MessageEncoderFactory messageEncoderFactory, BufferManager bufferManager,
                EndpointAddress remoteAddress, Uri via, ChannelManagerBase channelManager)
                : base(messageEncoderFactory, bufferManager, remoteAddress, WseTcpDuplexSessionChannel.AnonymousAddress, via, channelManager)
            {
            }

            void Connect()
            {
                Socket socket = null;
                int port = Via.Port;
                if (port == -1)
                {
                    port = 8081; // the default port used by WSE 3.0
                }

                IPHostEntry hostEntry;

                try
                {
                    hostEntry = Dns.GetHostEntry(Via.Host);
                }
                catch (SocketException socketException)
                {
                    throw new EndpointNotFoundException("Unable to resolve host" + Via.Host, socketException);
                }

                for (int i = 0; i < hostEntry.AddressList.Length; i++)
                {
                    try
                    {
                        IPAddress address = hostEntry.AddressList[i];
                        socket = new Socket(address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                        socket.Connect(new IPEndPoint(address, port));
                        break;
                    }
                    catch (SocketException socketException)
                    {
                        if (i == hostEntry.AddressList.Length - 1)
                        {
                            throw ConvertSocketException(socketException, "Connect");
                        }
                    }
                }

                base.InitializeSocket(socket);
            }

            protected override void OnOpen(TimeSpan timeout)
            {
                Connect();
                base.OnOpen(timeout);
            }

            protected override IAsyncResult OnBeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
            {
                return new ConnectAsyncResult(timeout, this, callback, state);
            }

            protected override void OnEndOpen(IAsyncResult result)
            {
                ConnectAsyncResult.End(result);
            }

            class ConnectAsyncResult : AsyncResult
            {
                TimeSpan timeout;
                ClientWseTcpDuplexSessionChannel channel;
                IPHostEntry hostEntry;
                Socket socket;
                bool connected;
                int currentEntry;
                int port;

                static AsyncCallback dnsGetHostCallback = new AsyncCallback(OnDnsGetHost);
                static AsyncCallback socketConnectCallback = new AsyncCallback(OnSocketConnect);


                public ConnectAsyncResult(TimeSpan timeout, ClientWseTcpDuplexSessionChannel channel, AsyncCallback callback, object state)
                    : base(callback, state)
                {
                    // production code should use this timeout
                    this.timeout = timeout;
                    this.channel = channel;
                    
                    IAsyncResult dnsGetHostResult = Dns.BeginGetHostEntry(channel.Via.Host, dnsGetHostCallback, this);
                    if (!dnsGetHostResult.CompletedSynchronously)
                    {
                        return;
                    }

                    if (CompleteDnsGetHost(dnsGetHostResult))
                    {
                        base.Complete(true);
                    }
                }

                IAsyncResult BeginSocketConnect()
                {
                    IPAddress address = hostEntry.AddressList[currentEntry];
                    socket = new Socket(address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                    while (true)
                    {
                        try
                        {
                            return socket.BeginConnect(new IPEndPoint(address, port), socketConnectCallback, this);
                        }
                        catch (SocketException socketException)
                        {
                            if (currentEntry == hostEntry.AddressList.Length - 1)
                            {
                                throw ConvertSocketException(socketException, "Connect");
                            }
                            currentEntry++;
                        }
                    }
                }

                bool CompleteDnsGetHost(IAsyncResult result)
                {
                    try
                    {
                        hostEntry = Dns.EndGetHostEntry(result);
                    }
                    catch (SocketException socketException)
                    {
                        throw new EndpointNotFoundException("Unable to resolve host" + channel.Via.Host, socketException);
                    }

                    port = this.channel.Via.Port;
                    if (port == -1)
                    {
                        port = 8081; // the default port used by WSE 3.0
                    }

                    IAsyncResult socketConnectResult = BeginSocketConnect();
                    if (!socketConnectResult.CompletedSynchronously)
                    {
                        return false;
                    }
                    return CompleteSocketConnect(socketConnectResult);
                }

                bool CompleteSocketConnect(IAsyncResult result)
                {
                    while (!connected && currentEntry < hostEntry.AddressList.Length)
                    {
                        try
                        {
                            socket.EndConnect(result);
                            connected = true;
                            break;
                        }
                        catch(SocketException socketException)
                        {
                            if (currentEntry == hostEntry.AddressList.Length - 1)
                            {
                                throw ConvertSocketException(socketException, "Connect");
                            }
                            currentEntry++;
                        }

                        result = BeginSocketConnect();
                        if (!result.CompletedSynchronously)
                        {
                            return false;
                        }
                    }

                    this.channel.InitializeSocket(socket);
                    return true;
                }

                static void OnDnsGetHost(IAsyncResult result)
                {
                    if (result.CompletedSynchronously)
                    {
                        return;
                    }

                    ConnectAsyncResult thisPtr = (ConnectAsyncResult)result.AsyncState;

                    Exception completionException = null;
                    bool completeSelf = false;
                    try
                    {
                        completeSelf = thisPtr.CompleteDnsGetHost(result);
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

                static void OnSocketConnect(IAsyncResult result)
                {
                    if (result.CompletedSynchronously)
                    {
                        return;
                    }

                    ConnectAsyncResult thisPtr = (ConnectAsyncResult)result.AsyncState;

                    Exception completionException = null;
                    bool completeSelf = false;
                    try
                    {
                        completeSelf = thisPtr.CompleteSocketConnect(result);
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


                public static void End(IAsyncResult result)
                {
                    AsyncResult.End<ConnectAsyncResult>(result);
                }
            }
        }
   }
}
