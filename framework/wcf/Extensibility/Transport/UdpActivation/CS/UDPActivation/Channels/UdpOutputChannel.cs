
//  Copyright (c) Microsoft Corporation.  All Rights Reserved.


using System;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Microsoft.ServiceModel.Samples
{
    /// <summary>
    /// IOutputChannel implementation for Udp.
    /// </summary>
    class UdpOutputChannel : ChannelBase, IOutputChannel
    {
        #region member_variables
        EndpointAddress remoteAddress;
        Uri via;
        EndPoint remoteEndPoint;
        Socket socket;
        MessageEncoder encoder;
        UdpChannelFactory parent;
        #endregion

        internal UdpOutputChannel(UdpChannelFactory factory, EndpointAddress remoteAddress, Uri via, MessageEncoder encoder)
            : base(factory)
        {
            if (!string.Equals(via.Scheme, UdpConstants.Scheme, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, 
                    "The scheme {0} specified in address is not supported.", via.Scheme), "via");
            }

            if (via.IsDefaultPort)
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, 
                    "The address {0} must specify a remote port.", via), "via");
            }

            // convert the Uri host into an IP Address
            IPAddress remoteIP = null;

            switch (via.HostNameType)
            {
                default:
                    throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, 
                        "Cannot determine the remote host address from {0}.",
                            this.via.ToString()), "via");

                case UriHostNameType.IPv4:
                case UriHostNameType.IPv6:
                        remoteIP = IPAddress.Parse(via.Host);
                        break;

                case UriHostNameType.Basic:
                case UriHostNameType.Dns:
                    {
                        IPHostEntry hostEntry = Dns.GetHostEntry(via.Host);
                        if (hostEntry.AddressList.Length > 0)
                        {
                            remoteIP = hostEntry.AddressList[0];
                        }
                        else
                        {
                            throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, 
                                "Failed to resolve remote host: {0}.", via.Host), 
                                "via");
                        }
                        break;
                    }
            }

            if (factory.Multicast && !UdpChannelHelpers.IsInMulticastRange(remoteIP))
            {
                throw new ArgumentOutOfRangeException("remoteEndPoint", "Via must be in the valid multicast range.");
            }

            this.parent = factory;
            this.remoteAddress = remoteAddress;
            this.via = via;
            this.encoder = encoder;
            this.remoteEndPoint = new IPEndPoint(remoteIP, via.Port);
            this.socket = new Socket(this.remoteEndPoint.AddressFamily, SocketType.Dgram, ProtocolType.Udp);

            if (parent.Multicast)
            {
                this.socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);
#if LATER // Support outgoing interface
                if (this.remoteEndPoint.AddressFamily == AddressFamily.InterNetwork)
                {
                    this.socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastInterface, IPAddress.HostToNetworkOrder((int)interfaceIndex));
                }
                else // IPv6
                {
                    this.sendSocketV6.SetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.MulticastInterface, (int)interfaceIndex);
                }
#endif
            }
        }

        #region IOutputChannel_Properties
        EndpointAddress IOutputChannel.RemoteAddress
        {
            get
            {
                return this.remoteAddress;
            }
        }

        Uri IOutputChannel.Via
        {
            get
            {
                return this.via;
            }
        }
        #endregion

        public override T GetProperty<T>()
        {
            if (typeof(T) == typeof(IOutputChannel))
            {
                return (T)(object)this;
            }

            T messageEncoderProperty = this.encoder.GetProperty<T>();
            if (messageEncoderProperty != null)
            {
                return messageEncoderProperty;
            }

            return base.GetProperty<T>();
        }

        /// <summary>
        /// Open the channel for use. We do not have any blocking work to perform so this is a no-op
        /// </summary>
        protected override void OnOpen(TimeSpan timeout)
        {
        }

        protected override IAsyncResult OnBeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return new CompletedAsyncResult(callback, state);
        }

        protected override void OnEndOpen(IAsyncResult result)
        {
            CompletedAsyncResult.End(result);
        }

        #region Socket_Shutdown
        /// <summary>
        /// Shutdown ungracefully
        /// </summary>
        protected override void OnAbort()
        {
            this.socket.Close(0);
        }

        /// <summary>
        /// Shutdown gracefully
        /// </summary>
        protected override void OnClose(TimeSpan timeout)
        {
            this.socket.Close();
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
        #endregion

        #region Send_Synchronous
        /// <summary>
        /// Address the Message and serialize it into a byte array.
        /// </summary>
        ArraySegment<byte> EncodeMessage(Message message)
        {
            try
            {
                this.remoteAddress.ApplyTo(message);
                return encoder.WriteMessage(message, int.MaxValue, parent.BufferManager);
            }
            finally
            {
                // Accessed the message by serializing it, so clean up
                message.Close();
            }
        }

        public void Send(Message message)
        {
            base.ThrowIfDisposedOrNotOpen();

            ArraySegment<byte> messageBuffer = EncodeMessage(message);

            // Adding the framing header
            messageBuffer = FramingCodec.Encode(this.remoteAddress.Uri,
                messageBuffer, parent.BufferManager);

            try
            {
                int bytesSent = this.socket.SendTo(messageBuffer.Array, messageBuffer.Offset, messageBuffer.Count,
                    SocketFlags.None, this.remoteEndPoint);

                if (bytesSent != messageBuffer.Count)
                {
                    throw new CommunicationException(string.Format(CultureInfo.CurrentCulture, 
                        "A Udp error occurred sending a message to {0}.", this.remoteEndPoint));
                }
            }
            catch (SocketException socketException)
            {
                throw UdpChannelHelpers.ConvertTransferException(socketException);
            }
            finally
            {
                // Must make sure buffers are always returned to the BufferManager
                parent.BufferManager.ReturnBuffer(messageBuffer.Array);
            }
        }

        public void Send(Message message, TimeSpan timeout)
        {
            // UDP does not block so we do not need timeouts.
            this.Send(message);
        }
        #endregion

        #region Send_Asynchronous
        public IAsyncResult BeginSend(Message message, AsyncCallback callback, object state)
        {
            base.ThrowIfDisposedOrNotOpen();
            return new SendAsyncResult(this, message, callback, state);
        }

        public IAsyncResult BeginSend(Message message, TimeSpan timeout, AsyncCallback callback, object state)
        {
            // UDP does not block so we do not need timeouts.
            return this.BeginSend(message, callback, state);
        }

        public void EndSend(IAsyncResult result)
        {
            SendAsyncResult.End(result);
        }

        /// <summary>
        /// Implementation of async send for Udp. 
        /// </summary>
        class SendAsyncResult : AsyncResult
        {
            ArraySegment<byte> messageBuffer;
            UdpOutputChannel channel;

            public SendAsyncResult(UdpOutputChannel channel, Message message, AsyncCallback callback, object state)
                : base(callback, state)
            {
                this.channel = channel;
                this.messageBuffer = channel.EncodeMessage(message);
                try
                {
                    IAsyncResult result = null;
                    try
                    {
                        result = channel.socket.BeginSendTo(messageBuffer.Array, messageBuffer.Offset, messageBuffer.Count,
                            SocketFlags.None, channel.remoteEndPoint, new AsyncCallback(OnSend), this);
                    }
                    catch (SocketException socketException)
                    {
                        throw UdpChannelHelpers.ConvertTransferException(socketException);
                    }

                    if (!result.CompletedSynchronously)
                        return;

                    CompleteSend(result, true);
                }
                catch
                {
                    CleanupBuffer();
                    throw;
                }
            }

            void CleanupBuffer()
            {
                if (messageBuffer.Array != null)
                {
                    this.channel.parent.BufferManager.ReturnBuffer(messageBuffer.Array);
                    messageBuffer = new ArraySegment<byte>();
                }
            }

            void CompleteSend(IAsyncResult result, bool synchronous)
            {
                try
                {
                    int bytesSent = channel.socket.EndSendTo(result);

                    if (bytesSent != messageBuffer.Count)
                    {
                        throw new CommunicationException(string.Format(CultureInfo.CurrentCulture, 
                            "A Udp error occurred sending a message to {0}.", channel.remoteEndPoint));
                    }
                }
                catch (SocketException socketException)
                {
                    throw UdpChannelHelpers.ConvertTransferException(socketException);
                }
                finally
                {
                    CleanupBuffer();
                }

                base.Complete(synchronous);
            }

            void OnSend(IAsyncResult result)
            {
                if (result.CompletedSynchronously)
                    return;

                try
                {
                    CompleteSend(result, false);
                }
                catch (Exception e)
                {
                    base.Complete(false, e);
                }
            }

            public static void End(IAsyncResult result)
            {
                AsyncResult.End<SendAsyncResult>(result);
            }
        }
        #endregion

    }
}

