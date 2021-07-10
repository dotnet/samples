//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading;

namespace Microsoft.Samples.ReliableSecureProfile
{
    public abstract class MakeConnectionDuplexChannel<TInnerChannel> 
        : LayeredChannel<TInnerChannel>, IDuplexChannel
        where TInnerChannel : class, IChannel 
    {
        static WaitCallback onDispatchLater = new WaitCallback(OnDispatchLater);

        ExceptionQueue exceptionQueue;
        InputQueue<Message> messageQueue;
        
        internal MakeConnectionDuplexChannel(ChannelManagerBase channelManager, 
            TInnerChannel innerChannel) : base(channelManager, innerChannel)
        {
            this.exceptionQueue = new ExceptionQueue(new object());
            this.messageQueue = new InputQueue<Message>();
        }

        public virtual EndpointAddress LocalAddress
        {
            get { return null; }
        }

        public virtual EndpointAddress RemoteAddress
        {
            get { return null; }
        }

        public virtual Uri Via
        {
            get { return null; }
        }

        public IAsyncResult BeginReceive(TimeSpan timeout, AsyncCallback callback, object state)
        {
            ThrowPendingException();
            TimeoutHelper.ThrowIfNegativeArgument(timeout);
            OnBeforeReceive();
            return HelpBeginReceive(this, timeout, callback, state);
        }

        public IAsyncResult BeginReceive(AsyncCallback callback, object state)
        {
            return this.BeginReceive(this.DefaultReceiveTimeout, callback, state);
        }

        public IAsyncResult BeginSend(Message message, TimeSpan timeout, AsyncCallback callback, object state)
        {
            ThrowPendingException();
            TimeoutHelper.ThrowIfNegativeArgument(timeout);
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }

            ThrowIfDisposedOrNotOpen();
            return OnBeginSend(message, timeout, callback, state);
        }

        public IAsyncResult BeginSend(Message message, AsyncCallback callback, object state)
        {
            return this.BeginSend(message, this.DefaultSendTimeout, callback, state);
        }

        public IAsyncResult BeginTryReceive(TimeSpan timeout, AsyncCallback callback, object state)
        {
            ThrowPendingException();
            TimeoutHelper.ThrowIfNegativeArgument(timeout);
            return this.messageQueue.BeginDequeue(timeout, callback, state);
        }

        public IAsyncResult BeginWaitForMessage(TimeSpan timeout, AsyncCallback callback, object state)
        {
            ThrowPendingException();
            TimeoutHelper.ThrowIfNegativeArgument(timeout);
            return this.messageQueue.BeginWaitForItem(timeout, callback, state);
        }

        public Message EndReceive(IAsyncResult result)
        {
            return HelpEndReceive(result);
        }

        public void EndSend(IAsyncResult result)
        {
            OnEndSend(result);
        }

        public bool EndTryReceive(IAsyncResult result, out Message message)
        {
            return this.messageQueue.EndDequeue(result, out message);
        }

        public bool EndWaitForMessage(IAsyncResult result)
        {
            return this.messageQueue.EndWaitForItem(result);
        }

        public override T GetProperty<T>()
        {
            if (typeof(T) == typeof(IDuplexChannel))
            {
                return (T)(object)this;
            }

            T baseProperty = base.GetProperty<T>();
            if (baseProperty != null)
            {
                return baseProperty;
            }

            return default(T);
        }

        protected abstract void OnSend(Message message, TimeSpan timeout);
        protected abstract IAsyncResult OnBeginSend(Message message, TimeSpan timeout, AsyncCallback callback, object state);
        protected abstract void OnEndSend(IAsyncResult result);
        protected abstract void OnBeforeReceive();

        protected override void OnClose(TimeSpan timeout)
        {
            base.OnClose(timeout);
            this.messageQueue.Close();
        }

        protected override void OnAbort()
        {
            base.OnAbort();
            this.messageQueue.Close();
        }

        protected override void OnClosing()
        {
            base.OnClosing();
            this.messageQueue.Shutdown();
        }

        protected override IAsyncResult OnBeginClose(TimeSpan timeout, AsyncCallback callback, object state)
        {
            this.messageQueue.Close();
            return base.OnBeginClose(timeout, callback, state);
        }

        protected override void OnEndClose(IAsyncResult result)
        {
            base.OnEndClose(result);
        }

        public Message Receive(TimeSpan timeout)
        {
            ThrowPendingException();
            TimeoutHelper.ThrowIfNegativeArgument(timeout);
            return HelpReceive(this, timeout);
        }

        public Message Receive()
        {
            return this.Receive(this.DefaultReceiveTimeout);
        }

        public void Send(Message message, TimeSpan timeout)
        {
            ThrowPendingException();
            TimeoutHelper.ThrowIfNegativeArgument(timeout);
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }

            ThrowIfDisposedOrNotOpen();
            OnSend(message, timeout);
        }

        public void Send(Message message)
        {
            this.Send(message, this.DefaultSendTimeout);
        }

        public bool TryReceive(TimeSpan timeout, out Message message)
        {
            ThrowPendingException();
            TimeoutHelper.ThrowIfNegativeArgument(timeout);
            return this.messageQueue.Dequeue(timeout, out message);
        }

        public bool WaitForMessage(TimeSpan timeout)
        {
            ThrowPendingException();
            TimeoutHelper.ThrowIfNegativeArgument(timeout);
            return this.messageQueue.WaitForItem(timeout);
        }

        public virtual bool EnqueueWithoutDispatch(Message message, Action dequeuedCallback)
        {
            return this.messageQueue.EnqueueWithoutDispatch(message, dequeuedCallback);
        }

        public virtual void Dispatch()
        {
            ThreadPool.QueueUserWorkItem(onDispatchLater, this);
        }

        public virtual void EnqueueAndDispatch(Message message, Action dequeuedCallback, bool canDispatchOnThisThread)
        {
            this.messageQueue.EnqueueAndDispatch(message, dequeuedCallback, canDispatchOnThisThread);
        }

        public void EnqueueException(Exception ex)
        {
            this.exceptionQueue.AddException(MakeConnectionUtility.WrapAsyncException(ex));
        }

        protected void Shutdown()
        {
            this.messageQueue.Shutdown();
        }

        protected void ThrowPendingException()
        {
            Exception exception = this.exceptionQueue.GetException();
            if (exception != null)
            {
                throw exception;
            }
        }

        static void OnDispatchLater(object state)
        {
            MakeConnectionDuplexChannel<TInnerChannel> thisPtr = (MakeConnectionDuplexChannel<TInnerChannel>)state;

            try
            {
                thisPtr.messageQueue.Dispatch();
            }
            catch (Exception e)
            {
                thisPtr.EnqueueException(e);
            }
        }

        internal static IAsyncResult HelpBeginReceive(IInputChannel channel, TimeSpan timeout, AsyncCallback callback, object state)
        {
            return new HelpReceiveAsyncResult(channel, timeout, callback, state);
        }

        internal static Message HelpEndReceive(IAsyncResult result)
        {
            return HelpReceiveAsyncResult.End(result);
        }

        internal static Message HelpReceive(IInputChannel channel, TimeSpan timeout)
        {
            Message message;
            if (channel.TryReceive(timeout, out message))
            {
                return message;
            }
            else
            {
                throw CreateReceiveTimedOutException(channel, timeout);
            }
        }

        static Exception CreateReceiveTimedOutException(IInputChannel channel, TimeSpan timeout)
        {
            if (channel.LocalAddress != null)
            {
                return new TimeoutException(String.Format
                    (ExceptionMessages.ReceiveTimedOut, channel.LocalAddress.Uri.AbsoluteUri, timeout));
            }
            else
            {
                return new TimeoutException(String.Format(ExceptionMessages.ReceiveTimedOutNoLocalAddress,timeout));
            }
        }

        class ExceptionQueue
        {
            Queue<Exception> exceptions = new Queue<Exception>();
            object thisLock;

            internal ExceptionQueue(object thisLock)
            {
                this.thisLock = thisLock;
            }

            object ThisLock
            {
                get { return this.thisLock; }
            }

            public void AddException(Exception exception)
            {
                if (exception == null)
                {
                    return;
                }

                lock (this.ThisLock)
                {
                    this.exceptions.Enqueue(exception);
                }
            }

            public Exception GetException()
            {
                lock (this.ThisLock)
                {
                    if (this.exceptions.Count > 0)
                    {
                        return this.exceptions.Dequeue();
                    }
                }

                return null;
            }
        }

        sealed class HelpReceiveAsyncResult : AsyncResult
        {
            static AsyncCallback onReceive = new AsyncCallback(OnReceive);

            IInputChannel channel;
            TimeSpan timeout;
            Message message;

            public HelpReceiveAsyncResult(IInputChannel channel, TimeSpan timeout, AsyncCallback callback, object state)
                : base(callback, state)
            {
                this.channel = channel;
                this.timeout = timeout;
                IAsyncResult result = channel.BeginTryReceive(timeout, onReceive, this);

                if (!result.CompletedSynchronously)
                {
                    return;
                }

                HandleReceiveComplete(result);
                base.Complete(true);
            }

            public static Message End(IAsyncResult result)
            {
                HelpReceiveAsyncResult thisPtr = AsyncResult.End<HelpReceiveAsyncResult>(result);
                return thisPtr.message;
            }

            void HandleReceiveComplete(IAsyncResult result)
            {
                if (!this.channel.EndTryReceive(result, out this.message))
                {
                    throw CreateReceiveTimedOutException(this.channel, this.timeout);
                }
            }

            static void OnReceive(IAsyncResult result)
            {
                if (result.CompletedSynchronously)
                {
                    return;
                }

                HelpReceiveAsyncResult thisPtr = (HelpReceiveAsyncResult)result.AsyncState;
                Exception completionException = null;
                try
                {
                    thisPtr.HandleReceiveComplete(result);
                }
                catch (Exception e)
                {
                    completionException = e;
                }

                thisPtr.Complete(false, completionException);
            }
        }
    }
}
