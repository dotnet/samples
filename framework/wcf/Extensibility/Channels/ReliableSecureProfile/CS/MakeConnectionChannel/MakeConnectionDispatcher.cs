//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ServiceModel.Channels;
using System.Threading;
using System.Xml;

namespace Microsoft.Samples.ReliableSecureProfile
{
    public sealed class MakeConnectionDispatcher
    {
        static WaitCallback onReplyAndEnqueueLater 
            = new WaitCallback(OnReplyAndEnqueueLater);

        Dictionary<UniqueId, MakeConnectionRequestContext> contextDictionary;
        InputQueue<MakeConnectionRequestContext> contexts;
        int refCount;
        object lockObj;

        public event EventHandler ReferencesReleased;

        public MakeConnectionDispatcher(Uri uri, object lockObj)
        {
            this.contexts = new InputQueue<MakeConnectionRequestContext>();
            this.contextDictionary = new Dictionary<UniqueId, MakeConnectionRequestContext>();
            this.Uri = uri;
            this.lockObj = lockObj;
        }

        public bool HasPendingSends
        {
            get
            {
                return contexts.PendingReaderCount > 0;
            }
        }

        public Uri Uri
        {
            get;
            private set;
        }

        bool TryGetContextForMessageId(UniqueId messageId, out MakeConnectionRequestContext context)
        {
            lock (this.contextDictionary)
            {
                return this.contextDictionary.TryGetValue(messageId, out context);
            }
        }

        public void RemoveContext(MakeConnectionRequestContext context)
        {
            if (context.RequestMesssageId != null)
            {
                lock (this.contextDictionary)
                {
                    if (this.contextDictionary.ContainsKey(context.RequestMesssageId))
                    {
                        this.contextDictionary.Remove(context.RequestMesssageId);
                    }
                }
            }
        }

        public void AddContext(MakeConnectionRequestContext context)
        {
            if (context.RequestMesssageId != null)
            {
                lock (this.contextDictionary)
                {
                    if (!this.contextDictionary.ContainsKey(context.RequestMesssageId))
                    {
                        this.contextDictionary.Add(context.RequestMesssageId, context);
                    }
                }
            }
        }

        public IAsyncResult BeginEnqueueAndSendMessage(Message message, TimeSpan timeout, AsyncCallback callback, object state)
        {
            return new EnqueueAndSendAsyncResult(message, timeout, this, callback, state);
        }

        public void EnqueueAndDispatchContext(MakeConnectionRequestContext context)
        {
            MakeConnectionRequestContext oldContext;

            if (context.IsMakeConnectionPollingMessage && TryDequeue(out oldContext))
            {
                ThreadPool.QueueUserWorkItem(onReplyAndEnqueueLater, new EnqueueData(this, oldContext, context));
            }
            else
            {
                this.contexts.EnqueueAndDispatch(context, new Action(context.OnDequeue), false);
            }
        }

        public void EnqueueAndSendMessage(Message message, TimeSpan timeout)
        {
            TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
            MakeConnectionRequestContext context;
            MessagePendingHeader.AddToMessage(message, this.HasPendingSends);
            do
            {
                if (message.Headers.RelatesTo == null || !TryGetContextForMessageId(message.Headers.RelatesTo, out context))
                {
                    context = contexts.Dequeue(timeoutHelper.RemainingTime());
                }
            } while (!context.TryReply(message, timeoutHelper.RemainingTime()));
        }


        public void EndEnqueueAndSendMessage(IAsyncResult result)
        {
            EnqueueAndSendAsyncResult.End(result);
        }

        public int AddRef()
        {
            return Interlocked.Increment(ref refCount);
        }

        public int Release()
        {
            int count = Interlocked.Decrement(ref refCount);

            if (count == 0)
            {
                lock (lockObj)
                {
                    this.Shutdown();
                    EventHandler handler = this.ReferencesReleased;
                    if (handler != null)
                    {
                        handler.Invoke(this, null);
                    }
                }
            }

            return count;
        }

        public void Shutdown()
        {
            this.contexts.Shutdown();
        }

        bool TryDequeue(out MakeConnectionRequestContext context)
        {
            return this.contexts.Dequeue(TimeSpan.Zero, out context);
        }

        static void OnReplyAndEnqueueLater(object state)
        {
            EnqueueData data = (EnqueueData)state;
            data.Dispatcher.contexts.EnqueueAndDispatch(data.NewContext, new Action(data.NewContext.OnDequeue), false);
            data.OldContext.SendAck();
        }

        sealed class EnqueueData
        {
            public EnqueueData(MakeConnectionDispatcher dispatcher, MakeConnectionRequestContext oldContext, MakeConnectionRequestContext newContext)
            {
                this.Dispatcher = dispatcher;
                this.OldContext = oldContext;
                this.NewContext = newContext;
            }

            public MakeConnectionDispatcher Dispatcher { get; private set; }
            public MakeConnectionRequestContext OldContext { get; private set; }
            public MakeConnectionRequestContext NewContext { get; private set; }
        }


        sealed class EnqueueAndSendAsyncResult : AsyncResult
        {
            static AsyncCallback onContextDequeueCompleted = new AsyncCallback(OnContextDequeueCompleted);
            static AsyncCallback onReplyCompleted = new AsyncCallback(OnReplyCompleted);

            MakeConnectionRequestContext context;
            MakeConnectionDispatcher dispatcher;
            Message message;
            TimeoutHelper timeoutHelper;

            public EnqueueAndSendAsyncResult(Message message, TimeSpan timeout, MakeConnectionDispatcher dispatcher, AsyncCallback callback, object state)
                : base(callback, state)
            {
                this.dispatcher = dispatcher;
                this.message = message;
                this.timeoutHelper = new TimeoutHelper(timeout);

                MessagePendingHeader.AddToMessage(message, this.dispatcher.HasPendingSends);

                if (message.Headers.RelatesTo == null || !this.dispatcher.TryGetContextForMessageId(message.Headers.RelatesTo, out this.context))
                {
                    IAsyncResult result = dispatcher.contexts.BeginDequeue(timeoutHelper.RemainingTime(), onContextDequeueCompleted, this);
                    if (result.CompletedSynchronously)
                    {
                        if (OnContextDequeueCompletedCore(result))
                        {
                            base.Complete(true);
                        }
                    }
                }
                else
                {
                    IAsyncResult result = this.context.BeginTryReply(this.message, this.timeoutHelper.RemainingTime(), onReplyCompleted, this);
                    if (result.CompletedSynchronously)
                    {
                        if (OnReplyCompletedCore(result))
                        {
                            base.Complete(true);
                        }
                    }
                }
            }

            static void OnContextDequeueCompleted(IAsyncResult result)
            {
                if (result.CompletedSynchronously)
                {
                    return;
                }

                EnqueueAndSendAsyncResult thisPtr = (EnqueueAndSendAsyncResult)result.AsyncState;

                Exception completionException = null;
                bool completeSelf;
                try
                {
                    completeSelf = thisPtr.OnContextDequeueCompletedCore(result);
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

            bool OnContextDequeueCompletedCore(IAsyncResult result)
            {
                this.context = this.dispatcher.contexts.EndDequeue(result);

                IAsyncResult replyResult = context.BeginTryReply(this.message, this.timeoutHelper.RemainingTime(), onReplyCompleted, this);
                if (replyResult.CompletedSynchronously)
                {
                    return OnReplyCompletedCore(replyResult);
                }

                return false;
            }

            static void OnReplyCompleted(IAsyncResult result)
            {
                if (result.CompletedSynchronously)
                {
                    return;
                }

                EnqueueAndSendAsyncResult thisPtr = (EnqueueAndSendAsyncResult)result.AsyncState;

                Exception completionException = null;
                bool completeSelf;
                try
                {
                    completeSelf = thisPtr.OnReplyCompletedCore(result);
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

            bool OnReplyCompletedCore(IAsyncResult result)
            {
                bool replied = this.context.EndTryReply(result);

                if (!replied)
                {
                    if (message.Headers.RelatesTo == null || !this.dispatcher.TryGetContextForMessageId(message.Headers.RelatesTo, out this.context))
                    {
                        IAsyncResult dequeueResult = dispatcher.contexts.BeginDequeue(timeoutHelper.RemainingTime(), onContextDequeueCompleted, this);
                        if (dequeueResult.CompletedSynchronously)
                        {
                            return OnContextDequeueCompletedCore(dequeueResult);
                        }
                    }
                    else
                    {
                        IAsyncResult replyResult = this.context.BeginTryReply(this.message, this.timeoutHelper.RemainingTime(), onReplyCompleted, this);
                        if (replyResult.CompletedSynchronously)
                        {
                            return OnReplyCompletedCore(replyResult);
                        }
                    }
                }

                return replied;
            }

            public static void End(IAsyncResult result)
            {
                AsyncResult.End<EnqueueAndSendAsyncResult>(result);
            }
        }
    }
}
