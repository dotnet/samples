using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Microsoft.ServiceModel.Samples
{
    class DurableInstanceContextOutputChannel
        : DurableInstanceContextChannelBase, IOutputChannel
    {

        private IOutputChannel innerOutputChannel;

        public DurableInstanceContextOutputChannel(
            ChannelManagerBase channelManager,
            ContextType contextType,
            IOutputChannel innerChannel,
            string contextStoreLocation)
            : base(channelManager, innerChannel)
        {
            this.contextType = contextType;
            this.innerOutputChannel = innerChannel;
            this.contextStoreLocation = contextStoreLocation;
            this.endpointAddress = innerChannel.RemoteAddress;
        }

        public IAsyncResult BeginSend(Message message, TimeSpan timeout,
            AsyncCallback callback, object state)
        {
            // Apply the context information before sending the message.
            this.ApplyContext(message);

            return innerOutputChannel.BeginSend(message, timeout,
                callback, state);
        }

        public IAsyncResult BeginSend(Message message, AsyncCallback callback,
            object state)
        {
            return BeginSend(message, DefaultSendTimeout, callback,
                state);
        }

        public void EndSend(IAsyncResult result)
        {
            innerOutputChannel.EndSend(result);
        }

        public EndpointAddress RemoteAddress
        {
            get
            {
                return innerOutputChannel.RemoteAddress;
            }
        }

        public void Send(Message message, TimeSpan timeout)
        {
            // Apply the context information before sending the message.
            this.ApplyContext(message);
            innerOutputChannel.Send(message, timeout);
        }

        public void Send(Message message)
        {
            Send(message, DefaultSendTimeout);
        }

        public Uri Via
        {
            get
            {
                return innerOutputChannel.Via;
            }
        }
    }
}

