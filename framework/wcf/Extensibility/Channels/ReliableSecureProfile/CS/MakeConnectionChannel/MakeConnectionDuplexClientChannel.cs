//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Microsoft.Samples.ReliableSecureProfile
{
    class MakeConnectionDuplexClientChannel<TChannel> : MakeConnectionDuplexChannel<TChannel>, IMakeConnectionFactorySettings
        where TChannel : class, IRequestChannel
    {
        IMakeConnectionFactorySettings settings;
        EndpointAddress localAddress;
        MakeConnectionPoller<TChannel> poller;
        MessageVersion messageVersion;

        public MakeConnectionDuplexClientChannel(ChannelManagerBase channelManager, TChannel innerChannel)
            : base(channelManager, innerChannel)
        {
            this.localAddress = new EndpointAddress(string.Concat(MakeConnectionConstants.AnonymousUriTemplate, Guid.NewGuid().ToString()));
            this.poller = MakeConnectionPoller<TChannel>.AddChannelToPoller(this);
            this.settings = (IMakeConnectionFactorySettings)channelManager;
        }

        public TimeSpan ClientPollTimeout
        {
            get
            {
                return this.settings.ClientPollTimeout;
            }
        }

        public override EndpointAddress LocalAddress
        {
            get
            {
                return this.localAddress;
            }
        }

        public MessageVersion MessageVersion
        {
            get
            {
                if (this.messageVersion == null)
                {
                    this.messageVersion = this.GetProperty<MessageVersion>();
                }

                return this.messageVersion;
            }
        }


        public override EndpointAddress RemoteAddress
        {
            get
            {
                return this.InnerChannel.RemoteAddress;
            }
        }

        public override Uri Via
        {
            get
            {
                return this.InnerChannel.Via;
            }
        }

        public TChannel GetChannelForPoll()
        {
            return this.InnerChannel;
        }

        public override void EnqueueAndDispatch(Message message, Action dequeuedCallback, bool canDispatchOnThisThread)
        {
            // FindHeader will mark as understood if present, 
            // but we do not process this header since we are always polling
            MessagePendingHeader.FindHeader(message);
            base.EnqueueAndDispatch(message, dequeuedCallback, canDispatchOnThisThread);
        }

        protected override void OnBeforeReceive()
        {
            // Do nothing
        }

        protected override IAsyncResult OnBeginSend(Message message, TimeSpan timeout, AsyncCallback callback, object state)
        {
            PrepareForSend(message);
            return InnerChannel.BeginRequest(message, callback, state);
        }

        protected override void OnEndSend(IAsyncResult result)
        {
            Message responseMessage = InnerChannel.EndRequest(result);
            if (responseMessage != null)
            {
                this.EnqueueAndDispatch(responseMessage, null, false);
            }
            this.poller.EnsurePolling(this);
        }

        protected override void OnSend(Message message, TimeSpan timeout)
        {
            PrepareForSend(message);
            Message responseMessage = InnerChannel.Request(message, timeout);
            if (responseMessage != null)
            {
                this.EnqueueAndDispatch(responseMessage, null, false);
            }
            this.poller.EnsurePolling(this);
        }

        protected override void OnClose(TimeSpan timeout)
        {
            TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
            this.poller.Close(timeoutHelper.RemainingTime());
            base.OnClose(timeoutHelper.RemainingTime());
        }

        protected override IAsyncResult OnBeginClose(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return new CloseAsyncResult(timeout, this, callback, state);
        }

        protected override void OnEndClose(IAsyncResult result)
        {
            CloseAsyncResult.End(result);
        }

        IAsyncResult BeginCloseInner(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return base.OnBeginClose(timeout, callback, state);
        }

        void EndCloseInner(IAsyncResult result)
        {
            base.OnEndClose(result);
        }

        protected override void OnAbort()
        {
            this.poller.Abort();
            base.OnAbort();
        }

        void PrepareForSend(Message message)
        {
            message.Headers.ReplyTo = this.localAddress;
        }

        sealed class CloseAsyncResult : AsyncResult
        {
            static AsyncCallback onClosePollerComplete = new AsyncCallback(OnClosePollerComplete);
            static AsyncCallback onCloseChannelComplete = new AsyncCallback(OnCloseChannelComplete);

            MakeConnectionDuplexClientChannel<TChannel> channel;
            TimeoutHelper timeoutHelper;
            
            public CloseAsyncResult(TimeSpan timeout, MakeConnectionDuplexClientChannel<TChannel> channel, AsyncCallback callback, object state)
                : base(callback, state)
            {
                this.channel = channel;
                this.timeoutHelper = new TimeoutHelper(timeout);

                IAsyncResult result = channel.poller.BeginClose(timeoutHelper.RemainingTime(), onClosePollerComplete, this);
                if (result.CompletedSynchronously)
                {
                    if (CompleteClosePoller(result))
                    {
                        base.Complete(true);
                    }
                }
            }

            static void OnClosePollerComplete(IAsyncResult result)
            {
                if (result.CompletedSynchronously)
                {
                    return;
                }

                CloseAsyncResult thisPtr = (CloseAsyncResult)result.AsyncState;

                Exception completionException = null;
                bool completeSelf = false;
                try
                {
                    completeSelf = thisPtr.CompleteClosePoller(result);
                }
                catch (Exception e)
                {
                    completionException = e;
                    completeSelf = true;
                }

                if (completeSelf)
                {
                    thisPtr.Complete(false, completionException);
                }
            }

            bool CompleteClosePoller(IAsyncResult result)
            {
                this.channel.EndClose(result);

                IAsyncResult closeChannelResult = this.channel.BeginCloseInner(this.timeoutHelper.RemainingTime(), onCloseChannelComplete, this);
                if (closeChannelResult.CompletedSynchronously)
                {
                    CompleteCloseChannel(closeChannelResult);
                    return true;
                }

                return false;
            }

            static void OnCloseChannelComplete(IAsyncResult result)
            {
                if (result.CompletedSynchronously)
                {
                    return;
                }

                CloseAsyncResult thisPtr = (CloseAsyncResult)result.AsyncState;

                Exception completionException = null;
                try
                {
                    thisPtr.CompleteCloseChannel(result);
                }
                catch (Exception e)
                {
                    completionException = e;
                }

                thisPtr.Complete(false, completionException);
            }


            void CompleteCloseChannel(IAsyncResult result)
            {
                this.channel.EndCloseInner(result);
            }

            public static void End(IAsyncResult result)
            {
                AsyncResult.End<CloseAsyncResult>(result);
            }
        }
    }
}
