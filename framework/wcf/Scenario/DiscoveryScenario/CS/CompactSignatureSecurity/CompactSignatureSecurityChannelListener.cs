//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel.Channels;
using System.ServiceModel.Discovery;

namespace Microsoft.Samples.Discovery
{

    class CompactSignatureSecurityChannelListener : ChannelListenerBase<IDuplexChannel>
    {
        DiscoveryVersion discoveryVersion;
        X509Certificate2 signingCertificate;
        ReceivedCertificatesStoreSettings receivedCertificatesStoreSettings;

        public CompactSignatureSecurityChannelListener(
            IChannelListener<IDuplexChannel> innerChannelListener,
            DiscoveryVersion discoveryVersion,
            X509Certificate2 signingCertificate,
            ReceivedCertificatesStoreSettings receivedCertificatesStoreSettings)
            : base()
        {
            this.discoveryVersion = discoveryVersion;
            this.InnerChannelListener = innerChannelListener;
            this.receivedCertificatesStoreSettings = receivedCertificatesStoreSettings;
            this.signingCertificate = signingCertificate;
        }

        public override Uri Uri
        {
            get
            {
                return this.InnerChannelListener.Uri;
            }
        }

        protected IChannelListener<IDuplexChannel> InnerChannelListener { get; private set; }

        public override T GetProperty<T>()
        {
            T baseProperty = base.GetProperty<T>();
            if (baseProperty != null)
            {
                return baseProperty;
            }

            if (this.InnerChannelListener != null)
            {
                return this.InnerChannelListener.GetProperty<T>();
            }
            else
            {
                return default(T);
            }
        }

        protected override IDuplexChannel OnAcceptChannel(System.TimeSpan timeout)
        {
            IDuplexChannel innerChannel = this.InnerChannelListener.AcceptChannel(timeout);
            return this.WrapChannel(innerChannel);
        }

        protected override IAsyncResult OnBeginAcceptChannel(System.TimeSpan timeout, System.AsyncCallback callback, object state)
        {
            return this.InnerChannelListener.BeginAcceptChannel(timeout, callback, state);
        }

        protected override IDuplexChannel OnEndAcceptChannel(System.IAsyncResult result)
        {
            IDuplexChannel innerChannel = this.InnerChannelListener.EndAcceptChannel(result);
            return this.WrapChannel(innerChannel);
        }

        protected override IAsyncResult OnBeginWaitForChannel(System.TimeSpan timeout, System.AsyncCallback callback, object state)
        {
            return this.InnerChannelListener.BeginWaitForChannel(timeout, callback, state);
        }

        protected override bool OnEndWaitForChannel(System.IAsyncResult result)
        {
            return this.InnerChannelListener.EndWaitForChannel(result);
        }

        protected override bool OnWaitForChannel(System.TimeSpan timeout)
        {
            return this.InnerChannelListener.WaitForChannel(timeout);
        }

        protected override void OnAbort()
        {
            this.InnerChannelListener.Abort();
        }

        protected override void OnClose(TimeSpan timeout)
        {
            this.InnerChannelListener.Close(timeout);
        }

        protected override IAsyncResult OnBeginClose(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return this.InnerChannelListener.BeginClose(timeout, callback, state);
        }

        protected override void OnEndClose(IAsyncResult result)
        {
            this.InnerChannelListener.EndClose(result);
        }

        protected override IAsyncResult OnBeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return this.InnerChannelListener.BeginOpen(timeout, callback, state);
        }

        protected override void OnEndOpen(IAsyncResult result)
        {
            this.InnerChannelListener.EndOpen(result);
        }

        protected override void OnOpen(TimeSpan timeout)
        {
            this.InnerChannelListener.Open(timeout);
        }

        IDuplexChannel WrapChannel(IDuplexChannel innerChannel)
        {
            if (innerChannel == null)
            {
                return null;
            }

            if (typeof(IDuplexChannel) == typeof(IDuplexChannel))
            {
                CompactSignatureSecurityDuplexChannel channel = new CompactSignatureSecurityDuplexChannel(
                    this,
                    (IDuplexChannel)innerChannel,
                    this.discoveryVersion,
                    this.signingCertificate,
                    this.receivedCertificatesStoreSettings);
                return (IDuplexChannel)(object)channel;
            }

            // Can't wrap this channel
            throw new ArgumentException("Unexpected channel type: only IDuplexChannel channel types are expected.");
        }
    }
}
