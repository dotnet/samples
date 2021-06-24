//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System;
using System.ServiceModel.Channels;
using System.Threading;

namespace Microsoft.Samples.ReliableSecureProfile
{
    sealed class MakeConnectionPoller<TChannel>
        where TChannel : class, IRequestChannel
    {
        static WaitCallback onEnsurePollingLater = new WaitCallback(OnEnsurePollingLater);
        static AsyncCallback onPollComplete = new AsyncCallback(OnPollComplete);

        bool stopPolling;
        object thisLock;
        EventHandler onChannelOpened;
        EventHandler onChannelClosedOrFaulted;
        MakeConnectionDuplexClientChannel<TChannel> channel;
        PollAsyncResult<TChannel> outstandingPoll;
        ManualResetEvent pollingComplete;
        MessageVersion messageVersion;

        bool polling;

        MakeConnectionPoller()
        {
            this.thisLock = new object();
            this.onChannelOpened = new EventHandler(OnChannelOpened);
            this.onChannelClosedOrFaulted = new EventHandler(OnChannelClosedOrFaulted);
            this.pollingComplete = new ManualResetEvent(false);
        }

        static MakeConnectionPoller<TChannel> GetForUri(Uri uri)
        {
            return new MakeConnectionPoller<TChannel>();
        }

        public static MakeConnectionPoller<TChannel> AddChannelToPoller(MakeConnectionDuplexClientChannel<TChannel> channel)
        {
            MakeConnectionPoller<TChannel> poller = GetForUri(channel.RemoteAddress.Uri);
            poller.AddChannel(channel);
            poller.messageVersion = channel.GetProperty<MessageVersion>();
            return poller;
        }

        public void AddChannel(MakeConnectionDuplexClientChannel<TChannel> channel)
        {
            channel.Opened += onChannelOpened;
            channel.Closed += onChannelClosedOrFaulted;
            channel.Faulted += onChannelClosedOrFaulted;
        }

        public void Abort()
        {
            this.AbortOutstandingPolls();
        }

        public void Close(TimeSpan timeout)
        {
            this.stopPolling = true;

            if (!this.pollingComplete.WaitOne(timeout))
            {
                throw new TimeoutException(String.Format
                    (ExceptionMessages.TimedOutWhileWaitingForPoll, timeout));
            }
        }

        public IAsyncResult BeginClose(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return new CloseAsyncResult(timeout, this, callback, state);
        }

        public void EndClose(IAsyncResult result)
        {
            CloseAsyncResult.End(result);
        }

        public void EnsurePolling(MakeConnectionDuplexClientChannel<TChannel> channel)
        {
            bool shouldSchedule = false;

            lock (ThisLock)
            {
                shouldSchedule = !polling;
            }

            if (shouldSchedule)
            {
                ThreadPool.QueueUserWorkItem(onEnsurePollingLater, this);
            }
        }

        static void OnEnsurePollingLater(object state)
        {
            MakeConnectionPoller<TChannel> thisPtr = (MakeConnectionPoller<TChannel>)state;

            try
            {
                thisPtr.EnsurePollingCore();
            }
            catch (Exception e)
            {
                thisPtr.EnqueueException(e);
            }
        }

        void EnsurePollingCore()
        {
            bool shouldPoll = false;
            MakeConnectionDuplexClientChannel<TChannel> channelToPoll = null;
            
            if (!polling)
            {
                lock (ThisLock)
                {
                    if (!polling)
                    {
                        if (stopPolling)
                        {
                            this.pollingComplete.Set();
                        }
                        else
                        {
                            channelToPoll = this.channel;

                            if (channelToPoll != null)
                            {
                                polling = shouldPoll = true;
                            }
                        }
                    }
                }
            }

            if (shouldPoll)
            {
                IAsyncResult result = BeginPoll(channelToPoll, onPollComplete, this);
                if (result.CompletedSynchronously)
                {
                    EndPoll(result);
                }
            }
        }

        IAsyncResult BeginPoll(MakeConnectionDuplexClientChannel<TChannel> channel, AsyncCallback callback, object state)
        {
            TChannel channelToUse = channel.GetChannelForPoll();

            this.outstandingPoll = new PollAsyncResult<TChannel>(channel, channelToUse, callback, state);
            return this.outstandingPoll;
        }

        void EndPoll(IAsyncResult result)
        {
            Message message = null;
            try
            {
                this.outstandingPoll = null;
                message = PollAsyncResult<TChannel>.End(result);

                if (message != null && message.IsFault)
                {
                    MessageFault fault = MessageFault.CreateFault(message, MakeConnectionConstants.Defaults.MaxFaultSize);
                    MakeConnectionMessageFault wsmcFault;

                    if (MakeConnectionMessageFault.TryCreateFault(message, fault, out wsmcFault))
                    {
                        throw MakeConnectionMessageFault.CreateException(wsmcFault);
                    }
                }
            }
            catch (TimeoutException)
            {
            }
            catch (Exception e)
            {
                EnqueueException(e);
            }

            if (message != null)
            {
                MakeConnectionDuplexClientChannel<TChannel> channelToDispatch = this.channel;
                if (channelToDispatch != null)
                {
                    channelToDispatch.EnqueueAndDispatch(message, null, false);
                }
            }

            lock (ThisLock)
            {
                polling = false;

                if (this.channel != null && !stopPolling)
                {
                    ThreadPool.QueueUserWorkItem(onEnsurePollingLater, this);
                }
                else
                {
                    this.pollingComplete.Set();
                }
            }
        }

        void EnqueueException(Exception e)
        {
            MakeConnectionDuplexClientChannel<TChannel> channelsToFault = this.channel;
            if (channelsToFault != null)
            {
                channelsToFault.EnqueueException(e);
            }
        }

        static void OnPollComplete(IAsyncResult result)
        {
            if (result.CompletedSynchronously)
            {
                return;
            }

            MakeConnectionPoller<TChannel> thisPtr = (MakeConnectionPoller<TChannel>)result.AsyncState;
            thisPtr.EndPoll(result);
        }

        void OnChannelOpened(object sender, EventArgs e)
        {
            this.channel = (MakeConnectionDuplexClientChannel<TChannel>)sender;
        }

        void OnChannelClosedOrFaulted(object sender, EventArgs e)
        {
            this.channel = null;
            AbortOutstandingPolls();
        }

        void AbortOutstandingPolls()
        {
            PollAsyncResult<TChannel> pollToAbort = this.outstandingPoll;
            if (pollToAbort != null)
            {
                pollToAbort.Abort();
            }
        }

        object ThisLock
        {
            get
            {
                return thisLock;
            }
        }

        sealed class CloseAsyncResult : AsyncResult
        {
            static WaitCallback onWaitForStopLater = new WaitCallback(OnWaitForStopLater);
            MakeConnectionPoller<TChannel> poller;
            TimeoutHelper timeoutHelper;

            public CloseAsyncResult(TimeSpan timeout, MakeConnectionPoller<TChannel> poller, AsyncCallback callback, object state)
                : base(callback, state)
            {
                this.timeoutHelper = new TimeoutHelper(timeout);
                this.poller = poller;

                this.poller.stopPolling = true;

                if (this.poller.polling)
                {
                    ThreadPool.QueueUserWorkItem(onWaitForStopLater, this);
                }
                else
                {
                    base.Complete(true);
                }
            }

            static void OnWaitForStopLater(object state)
            {
                CloseAsyncResult thisPtr = (CloseAsyncResult)state;

                Exception completionException = null;

                try
                {
                    thisPtr.CompleteWaitForStop();
                }
                catch (Exception e)
                {
                    completionException = e;
                }

                thisPtr.Complete(false, completionException);
            }

            void CompleteWaitForStop()
            {
                if (!this.poller.pollingComplete.WaitOne(this.timeoutHelper.RemainingTime()))
                {
                    throw new TimeoutException(
                        String.Format(ExceptionMessages.TimedOutWhileWaitingForPoll, 
                        (this.timeoutHelper.OriginalTimeout)));
                }
            }

            public static void End(IAsyncResult result)
            {
                AsyncResult.End<CloseAsyncResult>(result);
            }
        }

        sealed class PollAsyncResult<T> : AsyncResult
            where T : class, IRequestChannel 
        {
            TimeoutHelper timeout;
            MakeConnectionDuplexClientChannel<T> channel;
            T innerChannel;
            Message pollingMessage;
            Message returnMessage;

            static AsyncCallback onRequestComplete = new AsyncCallback(OnRequestComplete);

            public PollAsyncResult(
                MakeConnectionDuplexClientChannel<T> channel,
                T innerChannel,
                AsyncCallback callback,
                object state)
                : base(callback, state)
            {
                this.channel = channel;
                this.timeout = new TimeoutHelper(channel.ClientPollTimeout);
                this.innerChannel = innerChannel;

                this.pollingMessage = GetPollingMessage();

                IAsyncResult result = innerChannel.BeginRequest(pollingMessage, timeout.RemainingTime(), onRequestComplete, this);
                if (result.CompletedSynchronously)
                {
                    CompleteRequest(result);
                    base.Complete(true);
                }
            }

            static void OnRequestComplete(IAsyncResult result)
            {
                if (result.CompletedSynchronously)
                {
                    return;
                }

                PollAsyncResult<T> thisPtr = (PollAsyncResult<T>)result.AsyncState;

                Exception completionException = null;
                
                try
                {
                    thisPtr.CompleteRequest(result);
                }
                catch (Exception e)
                {
                    completionException = e;
                }

                thisPtr.Complete(false, completionException);
            }

            void CompleteRequest(IAsyncResult result)
            {
                this.returnMessage = innerChannel.EndRequest(result);
                this.pollingMessage.Close();
            }

            Message GetPollingMessage()
            {
                Message message = Message.CreateMessage(
                    channel.MessageVersion,
                    MakeConnectionConstants.MakeConnectionMessage.Action,
                    new MakeConnectionBodyWriter(channel.LocalAddress.Uri.ToString()));

                return message;
            }

            public static Message End(IAsyncResult result)
            {
                PollAsyncResult<TChannel> thisPtr = AsyncResult.End<PollAsyncResult<TChannel>>(result);

                if (thisPtr.pollingMessage != null)
                {
                    thisPtr.pollingMessage.Close();
                }

                return thisPtr.returnMessage;
            }

            public void Abort()
            {
                if (!this.IsCompleted)
                {
                    this.innerChannel.Abort();
                }
            }
        }
    }
}
