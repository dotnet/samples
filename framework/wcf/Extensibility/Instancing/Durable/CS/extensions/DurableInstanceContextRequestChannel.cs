using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Microsoft.ServiceModel.Samples
{
    class DurableInstanceContextRequestChannel
        : DurableInstanceContextChannelBase, IRequestChannel
    {
        IRequestChannel innerRequestChannel;

        public DurableInstanceContextRequestChannel(ChannelManagerBase channelManager,
            ContextType contextType,
            IRequestChannel innerChannel,
            string contextStoreLocation)
            : base(channelManager, innerChannel)
        {
            this.contextType = contextType;
            this.innerRequestChannel = innerChannel;
            this.contextStoreLocation = contextStoreLocation;
            this.endpointAddress = innerChannel.RemoteAddress;
        }

        public IAsyncResult BeginRequest(Message message, TimeSpan timeout,
            AsyncCallback callback, object state)
        {
            // Apply the context before sending the request.
            ApplyContext(message);
            return innerRequestChannel.BeginRequest(message, timeout,
                callback, state);
        }

        public IAsyncResult BeginRequest(Message message, AsyncCallback callback,
            object state)
        {
            return BeginRequest(message, DefaultSendTimeout, callback, state);
        }

        public Message EndRequest(IAsyncResult result)
        {
            return innerRequestChannel.EndRequest(result);
        }

        public EndpointAddress RemoteAddress
        {
            get
            {
                return innerRequestChannel.RemoteAddress;
            }
        }

        public Message Request(Message message, TimeSpan timeout)
        {
            // Apply the context before sending the request.
            ApplyContext(message);
            return innerRequestChannel.Request(message);
        }

        public Message Request(Message message)
        {
            return Request(message, DefaultSendTimeout);
        }

        public Uri Via
        {
            get
            {
                return innerRequestChannel.Via;
            }
        }
    }
}

