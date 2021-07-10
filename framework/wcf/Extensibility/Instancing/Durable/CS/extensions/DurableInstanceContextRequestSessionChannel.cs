using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Microsoft.ServiceModel.Samples
{
    class DurableInstanceContextRequestSessionChannel : DurableInstanceContextChannelBase, 
        IRequestSessionChannel
    {
        IRequestSessionChannel innerRequestSessionChannel;

        // Indicates whether the current message is the first
        // message in a session or not. 
        bool isFirstMessage;

        // Lock should be acquired on this object before changing the 
        // state of this channel.
        object stateLock;

        public DurableInstanceContextRequestSessionChannel(ChannelManagerBase channelManager,
            ContextType contextType,
            IRequestSessionChannel innerChannel, 
            string contextStoreLocation)
            : base(channelManager, innerChannel)
        {
            this.contextType = contextType;
            this.innerRequestSessionChannel = innerChannel;
            this.contextStoreLocation = contextStoreLocation;            
            this.endpointAddress = innerChannel.RemoteAddress;
            this.stateLock = new object();
            this.isFirstMessage = true;
        }

        public IOutputSession Session
        {
            get 
            {
                return innerRequestSessionChannel.Session;
            }
        }

        public IAsyncResult BeginRequest(Message message, TimeSpan timeout, 
            AsyncCallback callback, object state)
        {
            lock (stateLock)
            {
                // Apply the context if the message is the first message.
                if (isFirstMessage)
                {
                    ApplyContext(message);
                    isFirstMessage = false;
                }
            }
            
            return innerRequestSessionChannel.BeginRequest(message, timeout,
                callback, state);
        }

        public IAsyncResult BeginRequest(Message message, 
            AsyncCallback callback, object state)
        {
            return BeginRequest(message, DefaultSendTimeout, callback, state);
        }

        public Message EndRequest(IAsyncResult result)
        {
            return innerRequestSessionChannel.EndRequest(result);
        }

        public EndpointAddress RemoteAddress
        {
            get 
            {
                return innerRequestSessionChannel.RemoteAddress;
            }
        }

        public Message Request(Message message, TimeSpan timeout)
        {
            lock (stateLock)
            {
                // Apply the context if the message is the first message.
                if (isFirstMessage)
                {
                    ApplyContext(message);
                    isFirstMessage = false;
                }
            }

            return innerRequestSessionChannel.Request(message, timeout);
        }

        public Message Request(Message message)
        {
            return Request(message, DefaultSendTimeout);
        }

        public Uri Via
        {
            get 
            {
                return innerRequestSessionChannel.Via;
            }
        }
    }
}

