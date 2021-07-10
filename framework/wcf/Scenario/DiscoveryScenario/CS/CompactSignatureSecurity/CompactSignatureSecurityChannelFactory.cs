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

    class CompactSignatureSecurityChannelFactory : ChannelFactoryBase<IDuplexChannel>
    {
        X509Certificate2 signingCertificate;

        // The store details for getting the certificates used by clients.
        ReceivedCertificatesStoreSettings receivedCertificatesStoreSettings;
        
        DiscoveryVersion discoveryVersion;

        public CompactSignatureSecurityChannelFactory(
            IChannelFactory<IDuplexChannel> innerChannelFactory, 
            DiscoveryVersion discoveryVersion,
            X509Certificate2 signingCertificate,
            ReceivedCertificatesStoreSettings receivedCertificatesStoreSettings)
            : base()
        {
            this.signingCertificate = signingCertificate;
            this.receivedCertificatesStoreSettings = receivedCertificatesStoreSettings;
            this.InnerChannelFactory = innerChannelFactory;
            this.discoveryVersion = discoveryVersion;
        }

        public CompactSignatureSecurityChannelFactory(
            IDefaultCommunicationTimeouts timeouts,
            IChannelFactory<IDuplexChannel> innerChannelFactory,
            DiscoveryVersion discoveryVersion,
            X509Certificate2 signingCertificate,
            ReceivedCertificatesStoreSettings receivedCertificatesStoreSettings)
            : base(timeouts)
        {
            this.signingCertificate = signingCertificate;
            this.receivedCertificatesStoreSettings = receivedCertificatesStoreSettings;
            this.InnerChannelFactory = innerChannelFactory;
            this.discoveryVersion = discoveryVersion;
        }

        protected IChannelFactory<IDuplexChannel> InnerChannelFactory { get; private set; }

        public override T GetProperty<T>()
        {
            if (typeof(T) == typeof(IChannelFactory<IDuplexChannel>))
            {
                return (T)(object)this;
            }

            T baseProperty = base.GetProperty<T>();
            if (baseProperty != null)
            {
                return baseProperty;
            }

            return this.InnerChannelFactory.GetProperty<T>();
        }

        protected override IAsyncResult OnBeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return this.InnerChannelFactory.BeginOpen(timeout, callback, state);
        }

        protected override void OnEndOpen(IAsyncResult result)
        {
            this.InnerChannelFactory.EndOpen(result);
        }

        protected override IAsyncResult OnBeginClose(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return new ChainedAsyncResult(
                timeout, 
                callback, 
                state, 
                base.OnBeginClose, 
                base.OnEndClose, 
                this.InnerChannelFactory.BeginClose,
                this.InnerChannelFactory.EndClose);
        }

        protected override void OnEndClose(IAsyncResult result)
        {
            ChainedAsyncResult.End(result);
        }

        protected override void OnClose(TimeSpan timeout)
        {
            TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
            base.OnClose(timeoutHelper.RemainingTime());
            this.InnerChannelFactory.Close(timeoutHelper.RemainingTime());
        }

        protected override void OnOpen(TimeSpan timeout)
        {
            this.InnerChannelFactory.Open(timeout);
        }

        protected override void OnAbort()
        {
            base.OnAbort();
            this.InnerChannelFactory.Abort();
        }

        protected override IDuplexChannel OnCreateChannel(EndpointAddress address, Uri via)
        {
            IDuplexChannel innerChannel = this.InnerChannelFactory.CreateChannel(address, via);
            return (IDuplexChannel)(object)new CompactSignatureSecurityDuplexChannel(
                this,
                innerChannel,
                this.discoveryVersion,
                this.signingCertificate,
                this.receivedCertificatesStoreSettings);
        }

        class ChainedAsyncResult : AsyncResult
        {

            static AsyncCallback begin1Callback = new AsyncCallback(Begin1Callback);
            static AsyncCallback begin2Callback = new AsyncCallback(Begin2Callback);

            ChainedBeginHandler begin2;
            ChainedEndHandler end1;
            ChainedEndHandler end2;
            TimeoutHelper timeoutHelper;

            public ChainedAsyncResult(TimeSpan timeout, AsyncCallback callback, object state, ChainedBeginHandler begin1, ChainedEndHandler end1, ChainedBeginHandler begin2, ChainedEndHandler end2)
                : base(callback, state)
            {
                this.timeoutHelper = new TimeoutHelper(timeout);
                this.Begin(begin1, end1, begin2, end2);
            }

            public static void End(IAsyncResult result)
            {
                AsyncResult.End<ChainedAsyncResult>(result);
            }

            protected void Begin(ChainedBeginHandler begin1, ChainedEndHandler end1, ChainedBeginHandler begin2, ChainedEndHandler end2)
            {
                this.end1 = end1;
                this.begin2 = begin2;
                this.end2 = end2;

                IAsyncResult result = begin1(this.timeoutHelper.RemainingTime(), begin1Callback, this);
                if (!result.CompletedSynchronously)
                {
                    return;
                }

                if (this.Begin1Completed(result))
                {
                    this.Complete(true);
                }
            }

            static void Begin1Callback(IAsyncResult result)
            {
                if (result.CompletedSynchronously)
                {
                    return;
                }

                ChainedAsyncResult thisPtr = (ChainedAsyncResult)result.AsyncState;

                bool completeSelf = false;
                Exception completeException = null;

                try
                {
                    completeSelf = thisPtr.Begin1Completed(result);
                }
                catch (Exception exception)
                {
                    completeSelf = true;
                    completeException = exception;
                }

                if (completeSelf)
                {
                    thisPtr.Complete(false, completeException);
                }
            }

            static void Begin2Callback(IAsyncResult result)
            {
                if (result.CompletedSynchronously)
                {
                    return;
                }

                ChainedAsyncResult thisPtr = (ChainedAsyncResult)result.AsyncState;

                Exception completeException = null;

                try
                {
                    thisPtr.end2(result);
                }
                catch (Exception exception)
                {
                    completeException = exception;
                }

                thisPtr.Complete(false, completeException);
            }

            bool Begin1Completed(IAsyncResult result)
            {
                this.end1(result);

                result = this.begin2(this.timeoutHelper.RemainingTime(), begin2Callback, this);
                if (!result.CompletedSynchronously)
                {
                    return false;
                }

                this.end2(result);
                return true;
            }

            internal delegate IAsyncResult ChainedBeginHandler(TimeSpan timeout, AsyncCallback asyncCallback, object state);
            internal delegate void ChainedEndHandler(IAsyncResult result);
        }

        struct TimeoutHelper
        {
            DateTime deadline;

            public TimeoutHelper(TimeSpan timeout)
            {
                if (timeout < TimeSpan.Zero)
                {
                    throw new ArgumentOutOfRangeException("timeout");
                }

                if (timeout == TimeSpan.MaxValue)
                {
                    this.deadline = DateTime.MaxValue;
                }
                else
                {
                    this.deadline = DateTime.UtcNow + timeout;
                }
            }
         
            public static DateTime Add(DateTime time, TimeSpan timeout)
            {
                if (timeout >= TimeSpan.Zero && DateTime.MaxValue - time <= timeout)
                {
                    return DateTime.MaxValue;
                }

                if (timeout <= TimeSpan.Zero && DateTime.MinValue - time >= timeout)
                {
                    return DateTime.MinValue;
                }

                return time + timeout;
            }
           
            public TimeSpan RemainingTime()
            {
                if (this.deadline == DateTime.MaxValue)
                {
                    return TimeSpan.MaxValue;
                }
                else
                {
                    TimeSpan remaining = this.deadline - DateTime.UtcNow;
                    if (remaining <= TimeSpan.Zero)
                    {
                        return TimeSpan.Zero;
                    }
                    else
                    {
                        return remaining;
                    }
                }
            }
        }
    }
}
