//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Microsoft.Samples.LocalChannel
{
    class LocalChannelListener : ChannelListenerBase<IDuplexSessionChannel>
    {
        InputQueue<IDuplexSessionChannel>channelQueue;
        bool channelRegistered;
        Action onAcceptChannelCallback;
        Uri uri;

        internal LocalChannelListener(BindingContext context)
            : base()
        {

            this.channelQueue = new InputQueue<IDuplexSessionChannel>();
            this.uri = new Uri(context.ListenUriBaseAddress, context.ListenUriRelativeAddress);
            this.onAcceptChannelCallback = new Action(OnAcceptChannelCallback);
        }

        public override Uri Uri
        {
            get { return this.uri; }
        }

        public override T GetProperty<T>()
        {
            if (typeof(T).TypeHandle.Equals(typeof(MessageVersion).TypeHandle))
            {
                return (T)(object)LocalTransportDefaults.MessageVersionLocal;
            }

            return base.GetProperty<T>();
        }

        protected override void OnAbort()
        {
            DisposeChannelQueue();
        }

        protected override void OnFaulted()
        {
            base.OnFaulted();
        }

        protected override void OnOpen(TimeSpan timeout)
        {
            RegisterListener();
        }

        protected override IAsyncResult OnBeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
        {
            RegisterListener();
            return new CompletedAsyncResult(callback, state);
        }

        protected override void OnEndOpen(IAsyncResult result)
        {
            CompletedAsyncResult.End(result);
        }

        protected override void OnClose(TimeSpan timeout)
        {
            DisposeChannelQueue();
        }

        protected override IAsyncResult OnBeginClose(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return new ChannelCloseAsyncResult(this, timeout, callback, state);
        }

        protected override void OnEndClose(IAsyncResult result)
        {
            ChannelCloseAsyncResult.End(result);
        }

        protected override IDuplexSessionChannel OnAcceptChannel(TimeSpan timeout)
        {
            return this.channelQueue.Dequeue(timeout);
        }

        protected override IAsyncResult OnBeginAcceptChannel(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return this.channelQueue.BeginDequeue(timeout, callback, state);
        }

        protected override IDuplexSessionChannel OnEndAcceptChannel(IAsyncResult result)
        {
            return this.channelQueue.EndDequeue(result);
        }

        protected override bool OnWaitForChannel(TimeSpan timeout)
        {
            return this.channelQueue.WaitForItem(timeout);
        }

        protected override IAsyncResult OnBeginWaitForChannel(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return this.channelQueue.BeginWaitForItem(timeout, callback, state);
        }

        protected override bool OnEndWaitForChannel(IAsyncResult result)
        {
            return this.channelQueue.EndWaitForItem(result);
        }

        internal void EnqueueNewChannel(IDuplexSessionChannel channel, TimeSpan timeout)
        {
            // We do not want to dispath on the same client thread.  It would violate the principle for establishing boundaries 
            // between the client and the service.
            this.channelQueue.EnqueueAndDispatch(channel, this.onAcceptChannelCallback, false); //  dispatach on a seperate thread 
        }

        internal IAsyncResult BeginEnqueueNewChannel(IDuplexSessionChannel channel, TimeSpan timeout, AsyncCallback callback, object state)
        {
            return new ChannelEnqueuedAsyncResult(this, channel, timeout, callback, state);
        }

        internal void EndEnqueueNewChannel(IAsyncResult result)
        {
            ChannelEnqueuedAsyncResult.End(result);
        }

        void OnAcceptChannelCallback()
        {
        }

        void RegisterListener()
        {
            lock (ThisLock)
            {
                if (this.State == CommunicationState.Opening)
                {
                    LocalTransportManager.RegisterListener(this);
                    this.channelRegistered = true;
                }
            }
        }

        void DisposeChannelQueue()
        {
            this.channelQueue.Close();
            lock (ThisLock)
            {
                if (this.channelRegistered)
                {
                    LocalTransportManager.UnregisterListener(this);
                    this.channelRegistered = false;
                }
            }
        }

        class ChannelCloseAsyncResult : AsyncResult
        {
            static FastAsyncCallback onQueueAvailable = new FastAsyncCallback(OnQueueAvailable);
            LocalChannelListener listener;

            public ChannelCloseAsyncResult(LocalChannelListener listener, TimeSpan timeout, AsyncCallback callback, object state)
                : base(callback, state)
            {
                this.listener = listener;
                this.listener.DisposeChannelQueue();
                this.Complete(true);
            }

            public static void End(IAsyncResult result)
            {
                AsyncResult.End<ChannelCloseAsyncResult>(result);
            }

            static void OnQueueAvailable(object state, Exception asyncException)
            {
                ChannelCloseAsyncResult enqueuedAsyncResult = (ChannelCloseAsyncResult)state;

                if (asyncException != null)
                {
                    enqueuedAsyncResult.Complete(false, asyncException);
                }
                else
                {
                    Exception completionException = null;
                    try
                    {
                        enqueuedAsyncResult.listener.DisposeChannelQueue();
                    }
                    catch (Exception e)
                    {
                        completionException = e;
                    }

                    enqueuedAsyncResult.Complete(false, completionException);
                }
            }
        }

        class ChannelEnqueuedAsyncResult : AsyncResult
        {
            static FastAsyncCallback onQueueAvailable = new FastAsyncCallback(OnQueueAvailable);
            IDuplexSessionChannel channel;
            LocalChannelListener listener;

            public ChannelEnqueuedAsyncResult(LocalChannelListener listener, IDuplexSessionChannel channel,
                TimeSpan timeout, AsyncCallback callback, object state)
                : base(callback, state)
            {
                this.listener = listener;
                this.channel = channel;
                {
                    this.listener.channelQueue.EnqueueAndDispatch(this.channel, this.listener.onAcceptChannelCallback, false);
                    this.Complete(true);
                }
            }

            public static void End(IAsyncResult result)
            {
                AsyncResult.End<ChannelEnqueuedAsyncResult>(result);
            }

            static void OnQueueAvailable(object state, Exception asyncException)
            {
                ChannelEnqueuedAsyncResult enqueuedAsyncResult = (ChannelEnqueuedAsyncResult)state;

                if (asyncException != null)
                {
                    enqueuedAsyncResult.Complete(false, asyncException);
                }
                else
                {
                    Exception completionException = null;
                    try
                    {
                        enqueuedAsyncResult.listener.channelQueue.EnqueueAndDispatch(enqueuedAsyncResult.channel, enqueuedAsyncResult.listener.onAcceptChannelCallback, false);
                    }
                    catch (Exception e)
                    {
                        completionException = e;
                    }

                    enqueuedAsyncResult.Complete(false, completionException);
                }
            }
        }
    }
}
