using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Microsoft.ServiceModel.Samples
{
    class DurableInstanceContextDuplexSessionChannel : DurableInstanceContextChannelBase,
        IDuplexSessionChannel
    {
        IDuplexSessionChannel innerDuplexSessionChannel;

        // Indicates whether the current message is the first
        // inbound message in a duplex session or not. 
        bool isFirstIncomingMessage;

        // Indicates whether the current message is the first
        // outbound message in a duplex session or not. 
        bool isFirstOutgoingMessage;

        // Lock should be acquired on this object before changing the 
        // state from any outbound action.
        object outputStateLock;

        // Lock should be acquired on this object before changing the 
        // state from any inbound action.
        // * by having two locks for outbound and inbound state synchronization
        // we can let both inbound and outbound messages pass through simultaneously.
        object inputStateLock;

        // Once the context id is read out from the first incoming message,
        // it will be cached here.
        string contextId;

        public DurableInstanceContextDuplexSessionChannel(
            ChannelManagerBase channelManager,
            ContextType contextType,
            IDuplexSessionChannel innerChannel,
            string contextStoreLocation)
            : base(channelManager, innerChannel)
        {
            this.innerDuplexSessionChannel = innerChannel;
            this.contextType = contextType;
            this.contextStoreLocation = contextStoreLocation;
            this.endpointAddress = innerDuplexSessionChannel.RemoteAddress;
            this.isFirstOutgoingMessage = true;
            this.isFirstIncomingMessage = true;
            this.outputStateLock = new object();
            this.inputStateLock = new object();
        }

        public IDuplexSession Session
        {
            get
            {
                return innerDuplexSessionChannel.Session;
            }
        }

        public IAsyncResult BeginReceive(TimeSpan timeout,
            AsyncCallback callback, object state)
        {
            return innerDuplexSessionChannel.BeginReceive(timeout,
                callback, state);
        }

        public IAsyncResult BeginReceive(AsyncCallback callback,
            object state)
        {
            return BeginReceive(callback, state);
        }

        public IAsyncResult BeginTryReceive(TimeSpan timeout,
            AsyncCallback callback, object state)
        {
            return innerDuplexSessionChannel.BeginTryReceive(timeout,
                callback, state);
        }
        public IAsyncResult BeginWaitForMessage(TimeSpan timeout,
            AsyncCallback callback, object state)
        {
            return innerDuplexSessionChannel.BeginWaitForMessage(timeout,
                callback, state);
        }

        public Message EndReceive(IAsyncResult result)
        {
            Message message = innerDuplexSessionChannel.EndReceive(result);
            ReadAndAddContextIdToMessage(message);
            return message;
        }

        public bool EndTryReceive(IAsyncResult result, out Message message)
        {
            bool messageAvailable = innerDuplexSessionChannel.EndTryReceive(result,
                out message);

            if (messageAvailable)
            {
                ReadAndAddContextIdToMessage(message);
            }

            return messageAvailable;
        }

        public bool EndWaitForMessage(IAsyncResult result)
        {
            return innerDuplexSessionChannel.EndWaitForMessage(result);
        }

        public EndpointAddress LocalAddress
        {
            get { return innerDuplexSessionChannel.LocalAddress; }
        }

        public Message Receive(TimeSpan timeout)
        {
            Message message = innerDuplexSessionChannel.Receive(timeout);
            ReadAndAddContextIdToMessage(message);
            return message;
        }

        public Message Receive()
        {
            return Receive(DefaultReceiveTimeout);
        }

        public bool TryReceive(TimeSpan timeout, out Message message)
        {
            bool messageAvailable =
                innerDuplexSessionChannel.TryReceive(timeout, out message);

            if (messageAvailable)
            {
                ReadAndAddContextIdToMessage(message);
            }

            return messageAvailable;
        }

        public bool WaitForMessage(TimeSpan timeout)
        {
            return innerDuplexSessionChannel.WaitForMessage(timeout);
        }

        public IAsyncResult BeginSend(Message message, TimeSpan timeout,
            AsyncCallback callback, object state)
        {
            AddContextToOutgoingMessage(message);

            return innerDuplexSessionChannel.BeginSend(message,
                timeout, callback, state);
        }

        public IAsyncResult BeginSend(Message message,
            AsyncCallback callback, object state)
        {
            return BeginSend(message, DefaultSendTimeout,
                callback, state);
        }

        public void EndSend(IAsyncResult result)
        {
            innerDuplexSessionChannel.EndSend(result);
        }

        public EndpointAddress RemoteAddress
        {
            get { return innerDuplexSessionChannel.RemoteAddress; }
        }

        public void Send(Message message, TimeSpan timeout)
        {
            AddContextToOutgoingMessage(message);
            innerDuplexSessionChannel.Send(message, timeout);
        }

        public void Send(Message message)
        {
            Send(message, DefaultSendTimeout);
        }

        public Uri Via
        {
            get { return innerDuplexSessionChannel.Via; }
        }

        //Reads the context id from an incoming message and adds
        //it to the properties collection of the message.
        void ReadAndAddContextIdToMessage(Message message)
        {
            // Check the session terminating null message.
            if (message != null)
            {
                lock (inputStateLock)
                {
                    // Read the context if this is the first message.
                    // Otherwise add the context id in the cache to 
                    // the message properties.
                    if (isFirstIncomingMessage)
                    {
                        this.contextId = ReadContextId(message);
                        isFirstIncomingMessage = false;
                    }
                    else
                    {
                        message.Properties.Add(DurableInstanceContextUtility.ContextIdProperty,
                            this.contextId);
                    }
                }
            }
        }

        //Adds the context to a given outgoing message.
        void AddContextToOutgoingMessage(Message message)
        {
            // Check the session terminating null message.
            if (message != null)
            {
                lock (outputStateLock)
                {
                    // Apply the context only if this is the fist outgoing 
                    // message in the current session.
                    if (isFirstOutgoingMessage)
                    {
                        ApplyContext(message);
                        isFirstOutgoingMessage = false;
                    }
                }
            }
        }

    }
}

