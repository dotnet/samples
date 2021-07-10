using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Microsoft.ServiceModel.Samples
{
    class DurableInstanceContextInputSessionChannel : DurableInstanceContextChannelBase,
        IInputSessionChannel
    {
        IInputSessionChannel innerInputSessionChannel;

        // Indicates whether the current message is the first
        // message in a session or not.
        bool isFirstMessage;

        // Lock should be acquired on this object before changing the 
        // state of this channel.
        object stateLock;

        // Once the context id is read out from the first incoming message,
        // it will be cached here.
        string contextId;

        public DurableInstanceContextInputSessionChannel(
            ChannelManagerBase channelManager,
            ContextType contextType,
            IInputSessionChannel innerChannel)
            : base(channelManager, innerChannel)
        {
            this.isFirstMessage = true;
            this.innerInputSessionChannel = innerChannel;
            this.contextType = contextType;
            this.stateLock = new object();
        }

        public IInputSession Session
        {
            get
            {
                return this.innerInputSessionChannel.Session;
            }
        }
        public IAsyncResult BeginReceive(TimeSpan timeout,
            AsyncCallback callback, object state)
        {
            return this.innerInputSessionChannel.BeginReceive(
               timeout, callback, state);
        }

        public IAsyncResult BeginReceive(AsyncCallback callback, object state)
        {
            return BeginReceive(
                DefaultReceiveTimeout, callback, state);
        }

        public IAsyncResult BeginTryReceive(TimeSpan timeout,
            AsyncCallback callback, object state)
        {
            return this.innerInputSessionChannel.BeginTryReceive(
                timeout, callback, state);
        }

        public IAsyncResult BeginWaitForMessage(TimeSpan timeout,
            AsyncCallback callback, object state)
        {
            return this.innerInputSessionChannel.BeginWaitForMessage(
                timeout, callback, state);
        }

        public Message EndReceive(IAsyncResult result)
        {
            Message message =
                this.innerInputSessionChannel.EndReceive(result);
            ReadAndAddContextIdToMessage(message);
            return message;
        }

        public bool EndTryReceive(IAsyncResult result, out Message message)
        {
            if (this.innerInputSessionChannel.EndTryReceive(
                result, out message))
            {
                ReadAndAddContextIdToMessage(message);
                return true;
            }

            return false;
        }

        public bool EndWaitForMessage(IAsyncResult result)
        {
            return this.innerInputSessionChannel.EndWaitForMessage(result);
        }

        public EndpointAddress LocalAddress
        {
            get { return this.innerInputSessionChannel.LocalAddress; }
        }

        public Message Receive(TimeSpan timeout)
        {
            Message message =
                this.innerInputSessionChannel.Receive(timeout);

            ReadAndAddContextIdToMessage(message);

            return message;
        }

        public Message Receive()
        {
            return Receive(DefaultReceiveTimeout);
        }

        public bool TryReceive(TimeSpan timeout, out Message message)
        {
            if (this.innerInputSessionChannel.TryReceive(timeout, out message))
            {
                ReadAndAddContextIdToMessage(message);
                return true;
            }

            return false;
        }

        public bool WaitForMessage(TimeSpan timeout)
        {
            return this.innerInputSessionChannel.WaitForMessage(timeout);
        }


        //Reads the context id from an incoming message and adds
        //it to the properties collection of the message.
        void ReadAndAddContextIdToMessage(Message message)
        {
            // Check the session terminating null message.
            if (message != null)
            {
                lock (stateLock)
                {
                    // Read the context if this is the first message.
                    // Otherwise add the context id in the cache to 
                    // the message properties.
                    if (isFirstMessage)
                    {
                        this.contextId = ReadContextId(message);
                        isFirstMessage = false;
                    }
                    else
                    {
                        message.Properties.Add(DurableInstanceContextUtility.ContextIdProperty,
                            this.contextId);
                    }
                }
            }
        }
    }
}

