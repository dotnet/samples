//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading;

namespace Microsoft.Samples.ReliableSecureProfile
{
    public class MakeConnectionDuplexServiceChannel<TChannel> : MakeConnectionDuplexChannel<TChannel>
        where TChannel : class, IReplyChannel
    {
        static AsyncCallback onReceiveCompleted = new AsyncCallback(OnReceiveCompleted);
        static WaitCallback onReceiveCompletedLater = new WaitCallback(OnReceiveCompletedLater);

        int receiveCount;
        IMakeConnectionListenerSettings settings;

        public MakeConnectionDuplexServiceChannel(ChannelManagerBase channelManager, TChannel innerChannel)
            : base(channelManager, innerChannel)
        {
            this.settings = (IMakeConnectionListenerSettings)channelManager;
            this.DispatcherManager = new MakeConnectionDispatcherManager();
        }

        public virtual bool IsDatagram
        {
            get
            {
                return true;
            }
        }

        public override EndpointAddress LocalAddress
        {
            get
            {
                return this.InnerChannel.LocalAddress;
            }
        }

        Uri EnsureAnonymous(Message message)
        {
            Uri to;
            if (IsDatagram)
            {
                to = message.Headers.To;
            }
            else
            {
                to = this.RemoteAddress.Uri;
            }

            if (to == null)
            {
                throw new InvalidOperationException(ExceptionMessages.WsmcNoToHeader);
            }
            else if (!MakeConnectionUtility.IsAnonymousUri(to))
            {
                throw new InvalidOperationException(String.Format
                    (ExceptionMessages.WsmcNotAnonUri, to));
            }

            return to;
        }

        public MakeConnectionDispatcherManager DispatcherManager
        {
            get;
            private set;
        }

        public TimeSpan ServerPollTimeout
        {
            get
            {
                return this.settings.ServerPollTimeout;
            }
        }

        IAsyncResult BeginReceiveRequest(AsyncCallback callback, object state)
        {
            return this.InnerChannel.BeginReceiveRequest(callback, state);
        }

        MakeConnectionRequestContext EndReceiveRequest(IAsyncResult result)
        {
            RequestContext context = this.InnerChannel.EndReceiveRequest(result);
            if (context == null)
            {
                return null;
            }

            return new MakeConnectionRequestContext(this.DispatcherManager, context, this.ServerPollTimeout, this);
        }

        protected override void OnClosing()
        {
            base.OnClosing();
            this.DispatcherManager.Shutdown();
        }

        void EnsureReceiving()
        {
            bool shouldReceive = false;
            if (Interlocked.CompareExchange(ref receiveCount, 1, 0) == 0)
            {
                shouldReceive = true;
            }

            if (shouldReceive)
            {
                Exception receiveException = null;

                try
                {
                    IAsyncResult result = this.BeginReceiveRequest(onReceiveCompleted, this);

                    if (result.CompletedSynchronously)
                    {
                        if (Thread.CurrentThread.IsThreadPoolThread)
                        {
                            OnReceiveCompletedCore(result);
                        }
                        else
                        {
                            ThreadPool.QueueUserWorkItem(onReceiveCompletedLater, result);
                        }
                    }
                }
                catch (Exception e)
                {
                    receiveException = e;
                }
                finally
                {
                    if (receiveException != null)
                    {
                        Interlocked.CompareExchange(ref receiveCount, 0, 1);
                        this.EnqueueException(receiveException);
                    }
                }
            }
        }

        static void OnReceiveCompleted(IAsyncResult result)
        {
            if (result.CompletedSynchronously)
            {
                return;
            }

            MakeConnectionDuplexServiceChannel<TChannel> thisPtr = (MakeConnectionDuplexServiceChannel<TChannel>)result.AsyncState;
            Exception receiveException = null;

            try
            {
                thisPtr.OnReceiveCompletedCore(result);
            }
            catch (Exception e)
            {
                receiveException = e;
            }

            if (receiveException != null)
            {
                thisPtr.EnqueueException(receiveException);
                Interlocked.CompareExchange(ref thisPtr.receiveCount, 0, 1);
            }
        }

        static void OnReceiveCompletedLater(object state)
        {
            IAsyncResult result = (IAsyncResult)state;
            MakeConnectionDuplexServiceChannel<TChannel> thisPtr = (MakeConnectionDuplexServiceChannel<TChannel>)result.AsyncState;

            Exception receiveException = null;

            try
            {
                thisPtr.OnReceiveCompletedCore(result);
            }
            catch (Exception e)
            {
                receiveException = e;
            }

            if (receiveException != null)
            {
                thisPtr.EnqueueException(receiveException);
                Interlocked.CompareExchange(ref thisPtr.receiveCount, 0, 1);
            }
        }

        void EnsureAddress(Message message)
        {
            if (this.RemoteAddress == null && !this.IsDatagram)
            {
                OnRemoteAddressAcquired(message.Headers.ReplyTo);
            }
        }


        protected virtual void OnRemoteAddressAcquired(EndpointAddress remoteAddress)
        {
        }

        void OnReceiveCompletedCore(IAsyncResult result)
        {
            while (true)
            {
                MakeConnectionRequestContext context = this.EndReceiveRequest(result);
                if (context == null)
                {
                    this.Shutdown();
                }
                else
                {
                    Message requestMessage = context.RequestMessage;
                    bool enqueued = false;

                    if (!context.IsMakeConnectionPollingMessage)
                    {
                        // this wasn't a poll, so we can dispatch the message
                        EnsureAddress(requestMessage);
                        enqueued = this.EnqueueWithoutDispatch(requestMessage, null);
                    }

                    // this.Dispatch jumps threads, so it is ok to dispatch the message
                    // before dispatching or acknowledging the context
                    if (enqueued)
                    {
                        this.Dispatch();
                    }

                    context.DispatchOrAcknowledge();

                    result = this.BeginReceiveRequest(onReceiveCompleted, this);
                }

                if (context == null || !result.CompletedSynchronously)
                {
                    return;
                }
            }
        }

        protected override void OnBeforeReceive()
        {
            EnsureReceiving();
        }

        protected override IAsyncResult OnBeginSend(Message message, TimeSpan timeout, AsyncCallback callback, object state)
        {
            Uri to = EnsureAnonymous(message);
            return new SendAsyncResult(DispatcherManager.Get(to), message, timeout, callback, state);
        }

        protected override void OnEndSend(IAsyncResult result)
        {
            SendAsyncResult.End(result);
        }

        protected override void OnSend(Message message, TimeSpan timeout)
        {
            Uri to = EnsureAnonymous(message);
            MakeConnectionDispatcher dispatcher = this.DispatcherManager.Get(to);
            dispatcher.EnqueueAndSendMessage(message, timeout);
            dispatcher.Release();
        }

        protected override void OnOpened()
        {
            base.OnOpened();
            EnsureReceiving();
        }

        sealed class SendAsyncResult : AsyncResult
        {
            static AsyncCallback onSendCompleted = new AsyncCallback(OnSendCompleted);            MakeConnectionDispatcher dispatcher;

            public SendAsyncResult(MakeConnectionDispatcher dispatcher, Message message, TimeSpan timeout, AsyncCallback callback, object state)
                : base(callback, state)
            {
                this.dispatcher = dispatcher;
                IAsyncResult result = this.dispatcher.BeginEnqueueAndSendMessage(message, timeout, onSendCompleted, this);
                if (result.CompletedSynchronously)
                {
                    OnSendCompletedCore(result);
                    base.Complete(true);
                }
            }

            static void OnSendCompleted(IAsyncResult result)
            {
                if (result.CompletedSynchronously)
                {
                    return;
                }

                SendAsyncResult thisPtr = (SendAsyncResult)result.AsyncState;

                Exception completionException = null;

                try
                {
                    thisPtr.OnSendCompletedCore(result);
                }
                catch (Exception e)
                {
                    completionException = e;
                }

                thisPtr.Complete(false, completionException);
            }

            void OnSendCompletedCore(IAsyncResult result)
            {
                this.dispatcher.EndEnqueueAndSendMessage(result);
            }

            public static void End(IAsyncResult result)
            {
                SendAsyncResult thisPtr = AsyncResult.End<SendAsyncResult>(result);
                thisPtr.dispatcher.Release();
            }
        }
    }
}
