using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Microsoft.ServiceModel.Samples
{
    class DurableInstanceContextInputChannel
        : DurableInstanceContextChannelBase, IInputChannel
    {
        IInputChannel innerInputChannel;

        public DurableInstanceContextInputChannel(
            ChannelManagerBase channelManager,
            ContextType contextType,
            IInputChannel innerChannel)
            : base(channelManager, innerChannel)
        {
            this.contextType = contextType;
            this.innerInputChannel = innerChannel;
        }

        public IAsyncResult BeginReceive(TimeSpan timeout,
            AsyncCallback callback, object state)
        {
            return this.innerInputChannel.BeginReceive(
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
            return this.innerInputChannel.BeginTryReceive(
                timeout, callback, state);
        }

        public IAsyncResult BeginWaitForMessage(TimeSpan timeout,
            AsyncCallback callback, object state)
        {
            return this.innerInputChannel.BeginWaitForMessage(
                timeout, callback, state);
        }

        public Message EndReceive(IAsyncResult result)
        {
            Message message =
                this.innerInputChannel.EndReceive(result);

            // Read the context id from the incoming message.
            ReadContextId(message);
            return message;
        }

        public bool EndTryReceive(IAsyncResult result, out Message message)
        {
            if (this.innerInputChannel.EndTryReceive(
                result, out message))
            {
                // Read the context id from the incoming message.
                ReadContextId(message);
                return true;
            }

            return false;
        }

        public bool EndWaitForMessage(IAsyncResult result)
        {
            return this.innerInputChannel.EndWaitForMessage(result);
        }

        public EndpointAddress LocalAddress
        {
            get
            {
                return this.innerInputChannel.LocalAddress;
            }
        }

        public Message Receive(TimeSpan timeout)
        {
            Message message =
                this.innerInputChannel.Receive(timeout);

            // Read the context id from the incoming message.
            ReadContextId(message);
            return message;
        }

        public Message Receive()
        {
            return Receive(DefaultReceiveTimeout);
        }

        public bool TryReceive(TimeSpan timeout, out Message message)
        {
            if (this.innerInputChannel.TryReceive(timeout, out message))
            {
                // Read the context id from the incoming message.
                ReadContextId(message);
                return true;
            }

            return false;
        }

        public bool WaitForMessage(TimeSpan timeout)
        {
            return this.innerInputChannel.WaitForMessage(timeout);
        }
    }
}

