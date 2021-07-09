using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Microsoft.ServiceModel.Samples
{
    class DurableInstanceContextOutputSessionChannel : DurableInstanceContextChannelBase,
        IOutputSessionChannel
    {
        IOutputSessionChannel innerOutputSessionChannel;

        // Indicates whether the current message is the first
        // message in a session or not. 
        bool isFirstMessage;

        // Lock should be acquired on this object before changing the 
        // state of this channel.
        object stateLock;

        public DurableInstanceContextOutputSessionChannel(
            ChannelManagerBase channelManager,
            ContextType contextType,
            IOutputSessionChannel innerChannel,
            string contextStoreLocation)
            : base(channelManager, innerChannel)
        {
            this.innerOutputSessionChannel = innerChannel;
            this.isFirstMessage = true;
            this.contextStoreLocation = contextStoreLocation;
            this.endpointAddress = innerChannel.RemoteAddress;
            this.contextType = contextType;
            stateLock = new object();
        }

        public IOutputSession Session
        {
            get
            {
                return this.innerOutputSessionChannel.Session;
            }
        }

        public IAsyncResult BeginSend(Message message, TimeSpan timeout,
            AsyncCallback callback, object state)
        {
            lock (stateLock)
            {
                // Apply the context if the message is the first message.
                if (isFirstMessage)
                {
                    this.ApplyContext(message);
                    isFirstMessage = false;
                }
            }

            return innerOutputSessionChannel.BeginSend(message, timeout,
                callback, state);
        }

        public IAsyncResult BeginSend(Message message,
           AsyncCallback callback, object state)
        {
            return BeginSend(message, DefaultSendTimeout, callback, state);
        }

        public void EndSend(IAsyncResult result)
        {
            innerOutputSessionChannel.EndSend(result);
        }

        public EndpointAddress RemoteAddress
        {
            get
            {
                return innerOutputSessionChannel.RemoteAddress;
            }
        }

        public void Send(Message message, TimeSpan timeout)
        {
            lock (stateLock)
            {
                // Apply the context if the message is the first message.
                if (isFirstMessage)
                {
                    this.ApplyContext(message);
                    isFirstMessage = false;
                }
            }

            innerOutputSessionChannel.Send(message, timeout);
        }

        public void Send(Message message)
        {
            Send(message, DefaultSendTimeout);
        }

        public Uri Via
        {
            get
            {
                return innerOutputSessionChannel.Via;
            }
        }
    }
}

