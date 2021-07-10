//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System;
using System.Globalization;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Microsoft.Samples.LocalChannel
{

    abstract class LocalDuplexSessionChannel : ChannelBase, IDuplexSessionChannel
    {
        protected InputQueue<Message>receiveQueue;
        protected InputQueue<Message>sendQueue;
        EndpointAddress address;
        bool isInputSessionClosed;
        bool isOutputSessionClosed;
        Action onItemDequeued;
        LocalDuplexSession session;
        Uri via;

        protected LocalDuplexSessionChannel(ChannelManagerBase channelManager, 
            InputQueue<Message>receiveQueue, InputQueue<Message>sendQueue,
            EndpointAddress address, Uri via)
            : base(channelManager)
        {
            this.receiveQueue = receiveQueue;
            this.sendQueue = sendQueue;
            this.address = address;
            this.via = via;
            this.session = new LocalDuplexSession(this);
            this.onItemDequeued = new Action(OnItemDequeued);
        }

        public EndpointAddress LocalAddress
        {
            get
            {
                return this.address;
            }
        }

        public EndpointAddress RemoteAddress
        {
            get
            {
                return this.address;
            }
        }

        public IDuplexSession Session
        {
            get { return this.session; }
        }

        public Uri Via
        {
            get
            {
                return this.via;
            }
        }

        public override T GetProperty<T>()
        {
            if (typeof(T) == typeof(IDuplexSessionChannel))
            {
                return (T)(object)this;
            }

            return base.GetProperty<T>();
        }

        public Message Receive()
        {
            return this.Receive(this.DefaultReceiveTimeout);
        }

        public Message Receive(TimeSpan timeout)
        {
            Message message;
            if (this.TryReceive(timeout, out message))
            {
                return message;
            }
            else
            {
                throw new TimeoutException(String.Format(ExceptionMessages.ReceiveTimedOut, 
                    this.address.Uri.AbsoluteUri, timeout));
            }
        }

        public IAsyncResult BeginReceive(AsyncCallback callback, object state)
        {
            return this.BeginReceive(this.DefaultReceiveTimeout, callback, state);
        }

        public IAsyncResult BeginReceive(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return new HelpReceiveAsyncResult(this, timeout, callback, state);
        }

        public Message EndReceive(IAsyncResult result)
        {
            return HelpReceiveAsyncResult.End(result);
        }

        public bool TryReceive(TimeSpan timeout, out Message message)
        {
            bool success = this.receiveQueue.Dequeue(timeout, out message);
            if (success)
            {
                ProcessReceivedMessage(message);
            }

            return success;
        }

        public IAsyncResult BeginTryReceive(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return this.receiveQueue.BeginDequeue(timeout, callback, state);
        }

        public bool EndTryReceive(IAsyncResult result, out Message message)
        {
            DoneReceivingAsyncResult doneReceivingResult = result as DoneReceivingAsyncResult;
            if (doneReceivingResult != null)
            {
                DoneReceivingAsyncResult.End(doneReceivingResult, out message);
                return true;
            }

            bool success = this.receiveQueue.EndDequeue(result, out message);
            if (success)
            {
                ProcessReceivedMessage(message);
            }

            return success;
        }

        public bool WaitForMessage(TimeSpan timeout)
        {
            return this.receiveQueue.WaitForItem(timeout);
        }

        public IAsyncResult BeginWaitForMessage(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return this.receiveQueue.BeginWaitForItem(timeout, callback, state);
        }

        public bool EndWaitForMessage(IAsyncResult result)
        {
            DoneReceivingAsyncResult doneRecevingResult = result as DoneReceivingAsyncResult;
            if (doneRecevingResult != null)
            {
                DoneReceivingAsyncResult.End(doneRecevingResult);
                return true;
            }

            return this.receiveQueue.EndWaitForItem(result);
        }

        public void Send(Message message)
        {
            this.Send(message, this.DefaultSendTimeout);
        }

        public void Send(Message message, TimeSpan timeout)
        {
            ThrowIfDisposedOrNotOpen();

            Message messageToSend = PrepareMessageForSend(message);
            this.sendQueue.EnqueueAndDispatch(messageToSend, this.onItemDequeued, false);
        }

        public IAsyncResult BeginSend(Message message, AsyncCallback callback, object state)
        {
            return this.BeginSend(message, TimeSpan.MaxValue, callback, state);
        }

        public IAsyncResult BeginSend(Message message, TimeSpan timeout, AsyncCallback callback, object state)
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }

            ThrowIfDisposedOrNotOpen();

            return new SendAsyncResult(this, message, timeout, callback, state);
        }

        public void EndSend(IAsyncResult result)
        {
            SendAsyncResult.End(result);
        }

        //Closes the channel ungracefully during error conditions.
        protected override void OnAbort()
        {
            this.receiveQueue.Close();
            this.sendQueue.Close();
        }

        protected override void OnFaulted()
        {
            base.OnFaulted();
        }

        protected override void OnClose(TimeSpan timeout)
        {
            // close input session if necessary
            if (!isInputSessionClosed)
            {
                using (Message message = this.receiveQueue.Dequeue(timeout))
                {
                    if (message != null)
                    {
                        throw CreateReceiveShutdownReturnedNonNull(message);
                    }
                }
                OnInputSessionClosed();
            }
        }

        protected override IAsyncResult OnBeginClose(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return new CloseAsyncResult(this, timeout, callback, state);
        }

        protected override void OnEndClose(IAsyncResult result)
        {
            CloseAsyncResult.End(result);
        }

        void CloseOutputSession(TimeSpan timeout)
        {
            if (this.State == CommunicationState.Created 
                || this.State == CommunicationState.Opening 
                    || this.State == CommunicationState.Faulted)
            {
                throw new InvalidOperationException(ExceptionMessages.CommunicationObjectInInvalidState);
            }

            // check again in case the previous send faulted while we were waiting for the lock
            if (this.State == CommunicationState.Faulted)
            {
                throw new InvalidOperationException(ExceptionMessages.CommunicationObjectInInvalidState);
            }

            if (isOutputSessionClosed)
            {
                return;
            }
            isOutputSessionClosed = true;

            bool shouldFault = true;
            try
            {
                OnOutputSessionClosed();
                shouldFault = false;
            }
            finally
            {
                if (shouldFault)
                {
                    this.Fault();
                }
            }
        }

        IAsyncResult BeginCloseOutputSession(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return new CloseOutputSessionAsyncResult(this, timeout, callback, state);
        }

        void EndCloseOutputSession(IAsyncResult result)
        {
            CloseOutputSessionAsyncResult.End(result);
        }

        Message PrepareMessageForSend(Message message)
        {
            Message messageToSend;
            ThrowIfDisposedOrNotOpen();

            if (message == null)
            {
                throw new ArgumentNullException("message");
            }

            if (message.Version != LocalTransportDefaults.MessageVersionLocal)
            {
                throw new ProtocolException(String.Format(ExceptionMessages.SentMessageVersionMismatch, LocalTransportDefaults.MessageVersionLocal));
            }

            if (this.RemoteAddress != null)
            {
                this.RemoteAddress.ApplyTo(message);
            }

            LocalByRefMessage localMessage = message as LocalByRefMessage;
            if (localMessage != null)
            {
                return localMessage.PrepareForSend();
            }
            else
            {
                using (MessageBuffer messageCopy = message.CreateBufferedCopy(int.MaxValue))
                {
                    messageToSend = messageCopy.CreateMessage();
                    messageToSend.Properties.Clear();
                }
            }

            return messageToSend;
        }

        void OnInputSessionClosed()
        {
            lock (ThisLock)
            {
                if (isInputSessionClosed)
                {
                    return;
                }

                isInputSessionClosed = true;
            }
        }

        void OnOutputSessionClosed()
        {
            this.sendQueue.Shutdown();
            this.sendQueue.Close();
        }

        void OnItemDequeued()
        {
        }

        void ProcessReceivedMessage(Message message)
        {
            if (message == null)
            {
                OnInputSessionClosed();
            }
            else if (message.Version != LocalTransportDefaults.MessageVersionLocal)
            {
                throw new ProtocolException(String.Format(ExceptionMessages.SentMessageVersionMismatch, LocalTransportDefaults.MessageVersionLocal));
            }
        }

        static ProtocolException CreateReceiveShutdownReturnedNonNull(Message message)
        {
            if (message.IsFault)
            {
                try
                {
                    MessageFault fault = MessageFault.CreateFault(message, 64 * 1024);
                    FaultReasonText reason = fault.Reason.GetMatchingTranslation(CultureInfo.CurrentCulture);
                    string text = String.Format(ExceptionMessages.ReceiveShutdownReturnedFault, reason.Text);
                    return new ProtocolException(text);
                }
                catch (QuotaExceededException)
                {
                    string text = String.Format(ExceptionMessages.ReceiveShutdownReturnedLargeFault,
                        message.Headers.Action);
                    return new ProtocolException(text);
                }
            }
            else
            {
                string text = String.Format(ExceptionMessages.ReceiveShutdownReturnedMessage,
                        message.Headers.Action);
                return new ProtocolException(text);
            }
        }

        static Exception CreateReceiveTimedOutException(IDuplexSessionChannel channel, TimeSpan timeout)
        {
            if (channel.LocalAddress != null)
            {
                return new TimeoutException(String.Format
                    (ExceptionMessages.ReceiveTimedOut,channel.LocalAddress.Uri.AbsoluteUri, timeout));
            }
            else
            {
                return new TimeoutException(String.Format
                    (ExceptionMessages.ReceiveTimedOutNoLocalAddress, timeout));
            }
        }

        class CloseAsyncResult : AsyncResult
        {
            static AsyncCompletion closeInputSessionCallback;
            static AsyncCompletion closeOutputSessionCallback;
            TimeoutHelper timeouthelper;
            LocalDuplexSessionChannel channel;
            
            public CloseAsyncResult(LocalDuplexSessionChannel channel, TimeSpan timeout, AsyncCallback callback, object state)
                : base(callback, state)
            {
                if (channel.State == CommunicationState.Created
                    || channel.State == CommunicationState.Opening
                        || channel.State == CommunicationState.Faulted)
                {
                    throw new InvalidOperationException(ExceptionMessages.CommunicationObjectInInvalidState);
                }
                this.timeouthelper = new TimeoutHelper(timeout);
                this.channel = channel;
                bool completeSelf = false;
                IAsyncResult result = this.channel.BeginCloseOutputSession(timeout, 
                        PrepareAsyncCompletion(CloseOutputSessionCallback), this);
                if (result.CompletedSynchronously)
                {
                    completeSelf = OnOutputSessionClosed(result);
                }
                if (completeSelf)
                {
                    base.Complete(true);
                }
            }

            static AsyncCompletion CloseInputSessionCallback
            {
                get
                {
                    if (closeInputSessionCallback == null)
                    {
                        closeInputSessionCallback = new AsyncCompletion(OnInputSessionClosed);
                    }
                    return closeInputSessionCallback;
                }
            }

            static AsyncCompletion CloseOutputSessionCallback
            {
                get
                {
                    if (closeOutputSessionCallback == null)
                    {
                        closeOutputSessionCallback = new AsyncCompletion(OnOutputSessionClosed);
                    }
                    return closeOutputSessionCallback;
                }
            }

            public static void End(IAsyncResult result)
            {
                AsyncResult.End<CloseAsyncResult>(result);
            }

            static bool OnInputSessionClosed(IAsyncResult result)
            {
                CloseAsyncResult thisPtr = (CloseAsyncResult)result.AsyncState;
                using (Message message = thisPtr.channel.receiveQueue.EndDequeue(result))
                {
                    if (message != null)
                    {
                        throw CreateReceiveShutdownReturnedNonNull(message);
                    }
                }
                thisPtr.channel.OnInputSessionClosed();
                return true;
            }

            static bool OnOutputSessionClosed(IAsyncResult result)
            {
                CloseAsyncResult thisPtr = (CloseAsyncResult)result.AsyncState;
                thisPtr.channel.EndCloseOutputSession(result);
                if (thisPtr.channel.isInputSessionClosed)
                {
                    return true;
                }
                else
                {
                    IAsyncResult closeInputSessionResult =
                        thisPtr.channel.receiveQueue.BeginDequeue(thisPtr.timeouthelper.RemainingTime(), 
                            thisPtr.PrepareAsyncCompletion(CloseInputSessionCallback), thisPtr);
                    if (!closeInputSessionResult.CompletedSynchronously)
                    {
                        return false;
                    }
                    return OnInputSessionClosed(closeInputSessionResult);
                }
            }
        }

        class CloseOutputSessionAsyncResult : AsyncResult
        {
            static FastAsyncCallback onEnterComplete = new FastAsyncCallback(OnEnterComplete);
            LocalDuplexSessionChannel channel;

            public CloseOutputSessionAsyncResult(LocalDuplexSessionChannel channel, TimeSpan timeout, AsyncCallback callback, object state)
                : base(callback, state)
            {
                if (channel.State == CommunicationState.Created
                    || channel.State == CommunicationState.Opening
                        || channel.State == CommunicationState.Faulted)
                {
                    throw new InvalidOperationException(ExceptionMessages.CommunicationObjectInInvalidState);
                }

                this.channel = channel;

                bool shutDownSuccess = false;
                try
                {
                    this.channel.OnOutputSessionClosed();
                    shutDownSuccess = true;
                }
                finally
                {
                    if (!shutDownSuccess)
                    {
                        this.channel.Fault();
                    }
                }

                base.Complete(true);
            }

            public static void End(IAsyncResult result)
            {
                AsyncResult.End<CloseOutputSessionAsyncResult>(result);
            }

            static void OnEnterComplete(object state, Exception asyncException)
            {
                CloseOutputSessionAsyncResult closeAsyncResult = (CloseOutputSessionAsyncResult)state;

                if (asyncException != null)
                {
                    closeAsyncResult.Complete(false, asyncException);
                }
                else
                {
                    Exception completionException = null;
                    try
                    {
                        closeAsyncResult.channel.sendQueue.Shutdown();
                    }
                    catch (Exception e)
                    {
                        completionException = e;
                        closeAsyncResult.channel.Fault();
                    }
                    closeAsyncResult.Complete(false, completionException);
                }
            }
        }

        class HelpReceiveAsyncResult : AsyncResult
        {
            static AsyncCallback onReceive = new AsyncCallback(OnReceive);

            LocalDuplexSessionChannel channel;
            Message message;
            TimeSpan timeout;

            public HelpReceiveAsyncResult(LocalDuplexSessionChannel channel, TimeSpan timeout, 
                AsyncCallback callback, object state)
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
                    throw CreateReceiveTimedOutException(this.channel, timeout);
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

        class LocalDuplexSession : IDuplexSession
        {
            LocalDuplexSessionChannel channel;
            string id;

            public LocalDuplexSession(LocalDuplexSessionChannel channel)
                : base()
            {
                this.channel = channel;
                this.id = Guid.NewGuid().ToString();
            }

            public string Id
            {
                get { return this.id; }
            }

            public void CloseOutputSession()
            {
                channel.CloseOutputSession(channel.DefaultCloseTimeout);
            }

            public void CloseOutputSession(TimeSpan timeout)
            {
                channel.CloseOutputSession(timeout);
            }

            public IAsyncResult BeginCloseOutputSession(AsyncCallback callback, object state)
            {
                return channel.BeginCloseOutputSession(channel.DefaultCloseTimeout, callback, state);
            }

            public IAsyncResult BeginCloseOutputSession(TimeSpan timeout, AsyncCallback callback, object state)
            {
                return channel.BeginCloseOutputSession(timeout, callback, state);
            }

            public void EndCloseOutputSession(IAsyncResult result)
            {
                channel.EndCloseOutputSession(result);
            }
        }

        class SendAsyncResult : AsyncResult
        {
            static FastAsyncCallback onEnterComplete = new FastAsyncCallback(OnEnterComplete);

            LocalDuplexSessionChannel channel;
            Message message;

            public SendAsyncResult(LocalDuplexSessionChannel channel, Message message, TimeSpan timeout, AsyncCallback callback, object state)
                : base(callback, state)
            {
                this.channel = channel;
                this.message = channel.PrepareMessageForSend(message);
                this.channel.sendQueue.EnqueueAndDispatch(this.message, this.channel.onItemDequeued, false);
                this.Complete(true);
            }

            public static void End(IAsyncResult result)
            {
                AsyncResult.End<SendAsyncResult>(result);
            }

            static void OnEnterComplete(object state, Exception asyncException)
            {
                SendAsyncResult sendAsyncResult = (SendAsyncResult)state;

                if (asyncException != null)
                {
                    sendAsyncResult.Complete(false, asyncException);
                }
                else
                {
                    Exception completionException = null;
                    try
                    {
                        sendAsyncResult.channel.sendQueue.EnqueueAndDispatch(sendAsyncResult.message, sendAsyncResult.channel.onItemDequeued, false);
                    }
                    catch (Exception e)
                    {
                        completionException = e;
                    }

                    sendAsyncResult.Complete(false, completionException);
                }
            }
        }

        class DoneReceivingAsyncResult : CompletedAsyncResult
        {
            public DoneReceivingAsyncResult(AsyncCallback callback, object state)
                : base(callback, state)
            {
            }

            public static void End(DoneReceivingAsyncResult result, out Message message)
            {
                message = null;
                AsyncResult.End<DoneReceivingAsyncResult>(result);
            }

            public static void End(DoneReceivingAsyncResult result)
            {
                AsyncResult.End<DoneReceivingAsyncResult>(result);
            }
        }
    }
}
