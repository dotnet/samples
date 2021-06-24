//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Discovery;

namespace Microsoft.Samples.Discovery
{

    class CompactSignatureSecurityDuplexChannel : ChannelBase, IDuplexChannel
    {
        EventHandler onInnerChannelFaulted;

        public CompactSignatureSecurityDuplexChannel(
            ChannelManagerBase channelManager,
            IDuplexChannel innerChannel,
            DiscoveryVersion discoveryVersion,
            X509Certificate2 certificate,
            ReceivedCertificatesStoreSettings receivedCertificatesStoreSettings)
            : base(channelManager)
        {
            Utility.IfNullThrowNullArgumentException(innerChannel, "innerChannel");

            this.DiscoveryInfo = new ProtocolSettings(discoveryVersion);
            this.InnerChannel = innerChannel;
            this.ReceivedCertificatesStoreSettings = receivedCertificatesStoreSettings;
            this.SigningCertificate = certificate;

            this.onInnerChannelFaulted = new EventHandler(OnInnerChannelFaulted);
            this.InnerChannel.Faulted += this.onInnerChannelFaulted;
        }

        public EndpointAddress RemoteAddress
        {
            get
            {
                return this.InnerChannel.RemoteAddress;
            }
        }

        public Uri Via
        {
            get
            {
                return this.InnerChannel.Via;
            }
        }

        public EndpointAddress LocalAddress
        {
            get
            {
                return this.InnerChannel.LocalAddress;
            }
        }

        protected X509Certificate2 SigningCertificate { get; set; }

        protected ReceivedCertificatesStoreSettings ReceivedCertificatesStoreSettings { get; set; }

        protected ProtocolSettings DiscoveryInfo { get; set; }

        protected IDuplexChannel InnerChannel { get; private set; }

        public override T GetProperty<T>()
        {
            T baseProperty = base.GetProperty<T>();
            if (baseProperty != null)
            {
                return baseProperty;
            }

            return this.InnerChannel.GetProperty<T>();
        }

        protected override IAsyncResult OnBeginClose(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return this.InnerChannel.BeginClose(timeout, callback, state);
        }

        protected override IAsyncResult OnBeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return this.InnerChannel.BeginOpen(timeout, callback, state);
        }

        protected override void OnClose(TimeSpan timeout)
        {
            this.InnerChannel.Close(timeout);
        }

        protected override void OnClosing()
        {
            this.InnerChannel.Faulted -= this.onInnerChannelFaulted;
            base.OnClosing();
        }

        protected override void OnEndClose(IAsyncResult result)
        {
            this.InnerChannel.EndClose(result);
        }

        protected override void OnEndOpen(IAsyncResult result)
        {
            this.InnerChannel.EndOpen(result);
        }

        protected override void OnOpen(TimeSpan timeout)
        {
            this.InnerChannel.Open(timeout);
        }

        protected override void OnAbort()
        {
            this.InnerChannel.Abort();
        }

        void OnInnerChannelFaulted(object sender, EventArgs e)
        {
            this.Fault();
        }

        public IAsyncResult BeginSend(Message message, TimeSpan timeout, AsyncCallback callback, object state)
        {
            return this.InnerChannel.BeginSend(this.CreateSignedMessage(message), callback, state);
        }

        public IAsyncResult BeginSend(Message message, AsyncCallback callback, object state)
        {
            return this.BeginSend(message, DefaultSendTimeout, callback, state);
        }

        public void EndSend(IAsyncResult result)
        {
            this.InnerChannel.EndSend(result);
        }

        public void Send(Message message, TimeSpan timeout)
        {
            this.InnerChannel.Send(this.CreateSignedMessage(message), timeout);
        }

        public void Send(Message message)
        {
            this.Send(message, DefaultSendTimeout);
        }

        Message CreateSignedMessage(Message message)
        {
            Utility.IfNullThrowNullArgumentException(message, "message");
            SignedMessage signedMessage = new SignedMessage(message, this.SigningCertificate, this.DiscoveryInfo);
            Console.WriteLine("Sign {0}", message.Headers.Action);
            return signedMessage;
        }

        public IAsyncResult BeginReceive(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return new ReceiveAsyncResult(this, timeout, callback, state);
        }

        public IAsyncResult BeginReceive(AsyncCallback callback, object state)
        {
            return this.BeginReceive(DefaultReceiveTimeout, callback, state);
        }

        public IAsyncResult BeginTryReceive(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return new TryReceiveAsyncResult(this, timeout, callback, state);
        }

        public IAsyncResult BeginWaitForMessage(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return this.InnerChannel.BeginWaitForMessage(timeout, callback, state);
        }

        public Message EndReceive(IAsyncResult result)
        {
            return ReceiveAsyncResult.End(result);
        }

        public bool EndTryReceive(IAsyncResult result, out Message message)
        {
            return TryReceiveAsyncResult.End(result, out message);
        }

        public bool EndWaitForMessage(IAsyncResult result)
        {
            return this.InnerChannel.EndWaitForMessage(result);
        }

        public Message Receive(TimeSpan timeout)
        {
            return this.VerifyMessage(this.InnerChannel.Receive(timeout));
        }

        public Message Receive()
        {
            return this.Receive(DefaultReceiveTimeout);
        }

        public bool TryReceive(TimeSpan timeout, out Message message)
        {
            return this.InnerChannel.TryReceive(timeout, out message);
        }

        public bool WaitForMessage(TimeSpan timeout)
        {
            return this.InnerChannel.WaitForMessage(timeout);
        }

        Message VerifyMessage(Message innerMessage)
        {
            if (innerMessage == null)
            {
                return null;
            }

            MessageBuffer buffer = innerMessage.CreateBufferedCopy(int.MaxValue);
            ReceivedCompactSignatureHeader.VerifyMessage(
                    buffer.CreateMessage(),
                    this.DiscoveryInfo,
                    this.ReceivedCertificatesStoreSettings);
            Console.WriteLine("Verified message {0}.", innerMessage.Headers.Action);
            return buffer.CreateMessage();
        }

        abstract class ReceiveAsyncResultBase : AsyncResult
        {
            public ReceiveAsyncResultBase(
                CompactSignatureSecurityDuplexChannel channel,
                TimeSpan timeout,
                AsyncCallback callback,
                object state)
                : base(callback, state)
            {
                this.Timeout = timeout;
                this.Channel = channel;

                IAsyncResult result = this.BeginReceive();
                if (result.CompletedSynchronously)
                {
                    this.HandleReceiveComplete(result);
                }
            }

            protected CompactSignatureSecurityDuplexChannel Channel { get; private set; }

            protected Message Message { get; set; }

            protected TimeSpan Timeout { get; private set; }

            protected abstract void GetMessageAndVerify(IAsyncResult result, out Message receivedMessage);

            protected abstract IAsyncResult BeginReceive();

            protected void OnReceive(IAsyncResult result)
            {
                if (!result.CompletedSynchronously)
                {
                    this.HandleReceiveComplete(result);
                }
            }

            void HandleReceiveComplete(IAsyncResult result)
            {
                Message messageReceived = null;
                Exception exception = null;
                try
                {
                    this.GetMessageAndVerify(result, out messageReceived);
                    this.Complete(result.CompletedSynchronously);
                }
                catch (CompactSignatureSecurityException ex)
                {
                    exception = ex;
                    if (messageReceived != null && messageReceived.Headers != null && !String.IsNullOrEmpty(messageReceived.Headers.Action))
                    {
                        Console.WriteLine("Compact signature verification failed for {0}: {1}", messageReceived.Headers.Action, ex.Message);
                    }
                }
                catch (Exception ex)
                {
                    exception = ex;
                    Console.WriteLine("Unexpected exception while verifying message: {0}", ex.Message);
                }

                if (exception != null)
                {
                    this.Complete(result.CompletedSynchronously, exception);
                }
            }
        }

        class TryReceiveAsyncResult : ReceiveAsyncResultBase
        {
            bool returnValue;

            public TryReceiveAsyncResult(
                CompactSignatureSecurityDuplexChannel channel,
                TimeSpan timeout,
                AsyncCallback callback,
                object state)
                : base(channel, timeout, callback, state)
            {
            }

            public static bool End(IAsyncResult result, out Message message)
            {
                TryReceiveAsyncResult asyncResult = AsyncResult.End<TryReceiveAsyncResult>(result);
                message = asyncResult.Message;
                return asyncResult.returnValue;
            }

            protected override IAsyncResult BeginReceive()
            {
                return this.Channel.InnerChannel.BeginTryReceive(this.Timeout, this.OnReceive, null);
            }

            protected override void GetMessageAndVerify(IAsyncResult result, out Message messageReceived)
            {
                this.returnValue = this.Channel.InnerChannel.EndTryReceive(result, out messageReceived);
                this.Message = this.Channel.VerifyMessage(messageReceived);
            }
        }

        class ReceiveAsyncResult : ReceiveAsyncResultBase
        {
            public ReceiveAsyncResult(
                CompactSignatureSecurityDuplexChannel channel,
                TimeSpan timeout,
                AsyncCallback callback,
                object state)
                : base(channel, timeout, callback, state)
            {
            }

            public static Message End(IAsyncResult result)
            {
                ReceiveAsyncResult asyncResult = AsyncResult.End<ReceiveAsyncResult>(result);
                return asyncResult.Message;
            }

            protected override IAsyncResult BeginReceive()
            {
                return this.Channel.InnerChannel.BeginReceive(this.Timeout, this.OnReceive, null);
            }

            protected override void GetMessageAndVerify(IAsyncResult result, out Message messageReceived)
            {
                messageReceived = this.Channel.InnerChannel.EndReceive(result);
                this.Message = this.Channel.VerifyMessage(messageReceived);
            }
        }
    }
}
