//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading;
using System.Xml;

namespace Microsoft.Samples.ReliableSecureProfile
{
    public sealed class MakeConnectionRequestContext : RequestContext
    {
        static AsyncCallback onFaultCompleted = new AsyncCallback(OnFaultCompleted);
        static TimerCallback onPollTimeout = new TimerCallback(OnPollTimeout);

        bool addressIsAnonymousUri;
        MakeConnectionDispatcher dispatcher;
        RequestContext innerContext;
        bool isFaulting;
        TimeSpan pollTimeout;
        Timer pollTimer;
        bool responded;
        object thisLock;
        IDefaultCommunicationTimeouts timeouts;
        
        public MakeConnectionRequestContext(MakeConnectionDispatcherManager dispatcherManager, RequestContext innerContext, TimeSpan pollTimeout, IDefaultCommunicationTimeouts timeouts)
        {
            this.innerContext = innerContext;
            this.pollTimeout = pollTimeout;
            this.timeouts = timeouts;
            this.thisLock = new object();

            if (this.RequestMessage != null)
            {
                this.IsMakeConnectionPollingMessage = (this.RequestMessage.Headers.Action == MakeConnectionConstants.MakeConnectionMessage.Action);
                this.RequestMesssageId = this.RequestMessage.Headers.MessageId;

                EndpointAddress address = GetAddress();

                if (address != null && this.addressIsAnonymousUri)
                {
                    this.dispatcher = dispatcherManager.Get(address.Uri);
                    if (this.RequestMesssageId != null)
                    {
                        this.dispatcher.AddContext(this);
                    }
                }
            }
        }


        public bool IsMakeConnectionPollingMessage
        {
            get;
            private set;
        }

        object ThisLock
        {
            get { return this.thisLock; }
        }

        public UniqueId RequestMesssageId
        {
            get;
            private set;
        }

        public void DispatchOrAcknowledge()
        {
            bool dispatched = false;

            if (this.dispatcher != null)
            {
                if (this.IsMakeConnectionPollingMessage || this.dispatcher.HasPendingSends)
                {
                    if (this.IsMakeConnectionPollingMessage)
                    {
                        this.pollTimer = new Timer(onPollTimeout, this, pollTimeout, TimeSpan.FromMilliseconds(-1));
                    }
                    else
                    {
                        this.pollTimer = new Timer(onPollTimeout, this, MakeConnectionConstants.Defaults.ApplicationMessagePollTimeout, TimeSpan.FromMilliseconds(-1));
                    }
                    this.dispatcher.EnqueueAndDispatchContext(this);
                    dispatched = true;
                }
            }

            if (!dispatched && !this.isFaulting)
            {
                this.SendAck();
            }
        }

        static void OnPollTimeout(object state)
        {
            MakeConnectionRequestContext thisPtr = (MakeConnectionRequestContext)state;

            thisPtr.pollTimer = null; 
            thisPtr.SendAck();
        }

        public override void Abort()
        {
            this.innerContext.Abort();
            Cleanup();
        }

        public override IAsyncResult BeginReply(Message message, TimeSpan timeout, AsyncCallback callback, object state)
        {
            return this.innerContext.BeginReply(message, timeout, callback, state);
        }

        public override IAsyncResult BeginReply(Message message, AsyncCallback callback, object state)
        {
            return this.BeginReply(message, this.timeouts.SendTimeout, callback, state);
        }

        public IAsyncResult BeginSendAck(AsyncCallback callback, object state)
        {
            return new SendAckAsyncResult(this, callback, state);
        }

        public IAsyncResult BeginTryReply(Message message, TimeSpan timeout, AsyncCallback callback, object state)
        {
            return new TryReplyAsyncResult(message, timeout, this, callback, state);
        }

        void Cleanup()
        {
            if (this.dispatcher != null)
            {
                lock (ThisLock)
                {
                    if (this.dispatcher != null)
                    {
                        this.dispatcher.Release();
                        this.dispatcher = null;
                    }
                }
            }
        }

        public override void Close(TimeSpan timeout)
        {
            this.innerContext.Close(timeout);
            Cleanup();
        }

        public override void Close()
        {
            this.Close(this.timeouts.CloseTimeout);
        }

        public override void EndReply(IAsyncResult result)
        {
            this.innerContext.EndReply(result);
        }

        public void EndSendAck(IAsyncResult result)
        {
            bool succeeded = false;
            try
            {
                SendAckAsyncResult.End(result);
                succeeded = true;
            }
            finally
            {
                if (!succeeded)
                {
                    this.Abort();
                }
            }
        }

        public bool EndTryReply(IAsyncResult result)
        {
            bool succeeded = false;
            try
            {
                bool retval = TryReplyAsyncResult.End(result);
                succeeded = true;
                return retval;
            }
            finally
            {
                if (!succeeded)
                {
                    this.Abort();
                }
            }
        }

        public void OnDequeue()
        {
            if (this.pollTimer != null)
            {
                lock (ThisLock)
                {
                    if (this.pollTimer != null)
                    {
                        this.pollTimer.Change(-1, -1);
                        this.pollTimer = null;
                    }
                }
            }
        }

        public override void Reply(Message message, TimeSpan timeout)
        {
            this.innerContext.Reply(message, timeout);
        }

        public override void Reply(Message message)
        {
            this.Reply(message, this.timeouts.SendTimeout);
        }

        public override Message RequestMessage
        {
            get
            {
                return this.innerContext.RequestMessage;
            }
        }

        public void SendAck()
        {
            bool succeeded = false;

            try
            {
                if (this.TrySetResponded())
                {
                    this.Reply(null, this.timeouts.SendTimeout);
                    this.Close();
                }
                succeeded = true;
            }
            finally
            {
                if (!succeeded)
                {
                    this.Abort();
                }
            }
        }

        public bool TryReply(Message message, TimeSpan timeout)
        {
            bool retval = false;
            
            // only respond on the application message backchannel if the reply relates to the message sent
            if (this.IsMakeConnectionPollingMessage || message.Headers.RelatesTo == this.RequestMesssageId)
            {
                bool succeeded = false;
                try
                {
                    retval = TrySetResponded();
                    if (retval)
                    {
                        this.Reply(message, timeout);
                        this.Close();
                    }
                    succeeded = true;
                }
                finally
                {
                    if (!succeeded)
                    {
                        this.Abort();
                        retval = false;
                    }
                }
            }
            else
            {
                // This context's request message does not correspond with the message we are sending
                // so we will just return a 202 now, and let the next WS-MC call return the response for this
                // message and the potential response for this context's message
                this.SendAck();
            }

            return retval;
        }

        public bool TrySetResponded()
        {
            if (!this.responded)
            {
                lock (ThisLock)
                {
                    Timer timer = this.pollTimer;
                    if (timer != null)
                    {
                        this.pollTimer = null;
                        timer.Change(-1, -1);
                    }
                    
                    if (!this.responded)
                    {
                        if (this.dispatcher != null)
                        {
                            this.dispatcher.RemoveContext(this);
                        }
                        this.responded = true;
                        return true;
                    }
                }
            }

            return false;
        }

        MakeConnectionMessageFault VerifyProtocolElements(MakeConnectionMessageInfo info)
        {
            if (!string.IsNullOrEmpty(info.UnknownSelection) || info.Identifier != null)
            {
                return MakeConnectionMessageFault.CreateUnsupportedSelectionFault(this.RequestMessage, info.UnknownSelection);
            }
            else if (info.Address == null)
            {
                return MakeConnectionMessageFault.CreateMissingSelectionFault(this.RequestMessage);
            }

            return null;
        }

        EndpointAddress GetAddress()
        {
            EndpointAddress address = null;

            if (this.IsMakeConnectionPollingMessage)
            {
                MakeConnectionMessageInfo info = MakeConnectionMessageInfo.ReadMessage(this.RequestMessage);
                MakeConnectionMessageFault fault = VerifyProtocolElements(info);

                if (fault == null)
                {
                    address = new EndpointAddress(info.Address);
                }
                else
                {
                    this.isFaulting = true;
                    IAsyncResult result = BeginReply(fault.CreateMessage(RequestMessage.Version), onFaultCompleted, this);
                    if (result.CompletedSynchronously)
                    {
                        OnFaultCompletedCore(result);
                    }
                }
            }
            else
            {
                // normal message, grab the reply-to
                address = this.RequestMessage.Headers.ReplyTo;
            }

            if (address != null)
            {
                this.addressIsAnonymousUri = MakeConnectionUtility.IsAnonymousUri(address.Uri);
            }

            return address;
        }

        static void OnFaultCompleted(IAsyncResult result)
        {
            if (result.CompletedSynchronously)
            {
                return;
            }

            MakeConnectionRequestContext thisPtr = (MakeConnectionRequestContext)result.AsyncState;
            thisPtr.OnFaultCompletedCore(result);
        }

        void OnFaultCompletedCore(IAsyncResult result)
        {
            this.EndReply(result);
        }

        sealed class SendAckAsyncResult : AsyncResult
        {
            static AsyncCallback onReplyCompleted = new AsyncCallback(OnReplyCompleted);

            MakeConnectionRequestContext context;

            public SendAckAsyncResult(MakeConnectionRequestContext context, AsyncCallback callback, object state)
                : base(callback, state)
            {
                this.context = context;
                if (this.context.TrySetResponded())
                {
                    IAsyncResult result = this.context.BeginReply(null, onReplyCompleted, this);
                    if (!result.CompletedSynchronously)
                    {
                        return;
                    }

                    OnReplyCompletedCore(result);
                }

                base.Complete(true);
            }

            static void OnReplyCompleted(IAsyncResult result)
            {
                if (result.CompletedSynchronously)
                {
                    return;
                }

                SendAckAsyncResult thisPtr = (SendAckAsyncResult)result.AsyncState;

                Exception completionException = null;

                try
                {
                    thisPtr.OnReplyCompletedCore(result);
                }
                catch (Exception e)
                {
                    completionException = e;                }

                thisPtr.Complete(false, completionException);
            }

            void OnReplyCompletedCore(IAsyncResult result)
            {
                this.context.EndReply(result);
                this.context.Close();
            }

            public static void End(IAsyncResult result)
            {
                AsyncResult.End<SendAckAsyncResult>(result);
            }
        }

        sealed class TryReplyAsyncResult : AsyncResult
        {
            static AsyncCallback onReplyCompleted = new AsyncCallback(OnReplyCompleted);
            static AsyncCallback onSendResponseCompleted = new AsyncCallback(OnSendResponseCompleted);
            MakeConnectionRequestContext context;
            Message message;
            TimeoutHelper timeoutHelper;
            bool retval;

            public TryReplyAsyncResult(Message message, TimeSpan timeout, MakeConnectionRequestContext context, AsyncCallback callback, object state)
                : base(callback, state)
            {
                this.message = message;
                this.timeoutHelper = new TimeoutHelper(timeout);
                this.context = context;

                if (this.context.IsMakeConnectionPollingMessage || this.message.Headers.RelatesTo == this.context.RequestMesssageId)
                {
                    this.retval = this.context.TrySetResponded();

                    if (this.retval)
                    {
                        IAsyncResult result = this.context.BeginReply(this.message, this.timeoutHelper.RemainingTime(), onReplyCompleted, this);
                        if (!result.CompletedSynchronously)
                        {
                            return;
                        }
                        
                        OnReplyCompletedCore(result);
                    }
                }
                else
                {
                    IAsyncResult result = this.context.BeginSendAck(onSendResponseCompleted, this);
                    if (!result.CompletedSynchronously)
                    {
                        return;
                    }

                    OnSendResponseCompletedCore(result);
                }

                base.Complete(true);
            }

            static void OnReplyCompleted(IAsyncResult result)
            {
                if (result.CompletedSynchronously)
                {
                    return;
                }

                TryReplyAsyncResult thisPtr = (TryReplyAsyncResult)result.AsyncState;

                Exception completionException = null;

                try
                {
                    thisPtr.OnReplyCompletedCore(result);
                }
                catch (Exception e)
                {
                    completionException = e;
                }

                thisPtr.Complete(false, completionException);
            }

            static void OnSendResponseCompleted(IAsyncResult result)
            {
                if (result.CompletedSynchronously)
                {
                    return;
                }

                TryReplyAsyncResult thisPtr = (TryReplyAsyncResult)result.AsyncState;

                Exception completionException = null;

                try
                {
                    thisPtr.OnSendResponseCompletedCore(result);
                }
                catch (Exception e)
                {
                    completionException = e;
                }

                thisPtr.Complete(false, completionException);
            }

            void OnReplyCompletedCore(IAsyncResult result)
            {
                this.context.EndReply(result);
                this.context.Close();
            }

            void OnSendResponseCompletedCore(IAsyncResult result)
            {
                // SendAckAsyncResult will close the context
                this.context.EndSendAck(result);
            }

            public static bool End(IAsyncResult result)
            {
                TryReplyAsyncResult thisPtr = AsyncResult.End<TryReplyAsyncResult>(result);
                return thisPtr.retval;
            }
        }
    }
}
