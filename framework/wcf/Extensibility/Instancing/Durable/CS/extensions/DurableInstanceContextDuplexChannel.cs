using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Microsoft.ServiceModel.Samples
{
    class DurableInstanceContextDuplexChannel
        : DurableInstanceContextChannelBase, IDuplexChannel
    {
        DurableInstanceContextOutputChannel outputChannel;

        DurableInstanceContextInputChannel inputChannel;

        public DurableInstanceContextDuplexChannel(ChannelManagerBase channelManger,
            ContextType contextType,
            IDuplexChannel innerChannel,
            string contextStoreLocation)
            : base(channelManger, innerChannel)
        {
            this.contextType = contextType;

            this.outputChannel = new DurableInstanceContextOutputChannel(channelManger,
                contextType, (IOutputChannel)innerChannel, contextStoreLocation);

            this.inputChannel = new DurableInstanceContextInputChannel(channelManger,
                contextType, (IInputChannel)innerChannel);
        }

        public IAsyncResult BeginReceive(TimeSpan timeout,
            AsyncCallback callback, object state)
        {
            return inputChannel.BeginReceive(timeout, callback, state);
        }

        public IAsyncResult BeginReceive(AsyncCallback callback, object state)
        {
            return inputChannel.BeginReceive(callback, state);
        }

        public IAsyncResult BeginTryReceive(TimeSpan timeout,
            AsyncCallback callback, object state)
        {
            return inputChannel.BeginTryReceive(timeout, callback, state);
        }

        public IAsyncResult BeginWaitForMessage(TimeSpan timeout,
            AsyncCallback callback, object state)
        {
            return inputChannel.BeginWaitForMessage(timeout, callback, state);
        }

        public Message EndReceive(IAsyncResult result)
        {
            return inputChannel.EndReceive(result);
        }

        public bool EndTryReceive(IAsyncResult result, out Message message)
        {
            return inputChannel.EndTryReceive(result, out message);
        }

        public bool EndWaitForMessage(IAsyncResult result)
        {
            return inputChannel.EndWaitForMessage(result);
        }

        public EndpointAddress LocalAddress
        {
            get
            {
                return inputChannel.LocalAddress;
            }
        }

        public Message Receive(TimeSpan timeout)
        {
            return inputChannel.Receive(timeout);
        }

        public Message Receive()
        {
            return inputChannel.Receive();
        }

        public bool TryReceive(TimeSpan timeout, out Message message)
        {
            return inputChannel.TryReceive(timeout, out message);
        }

        public bool WaitForMessage(TimeSpan timeout)
        {
            return inputChannel.WaitForMessage(timeout);
        }


        public IAsyncResult BeginSend(Message message, TimeSpan timeout,
            AsyncCallback callback, object state)
        {
            return outputChannel.BeginSend(message, timeout, callback, state);
        }

        public IAsyncResult BeginSend(Message message,
            AsyncCallback callback, object state)
        {
            return outputChannel.BeginSend(message, callback, state);
        }

        public void EndSend(IAsyncResult result)
        {
            outputChannel.EndSend(result);
        }

        public EndpointAddress RemoteAddress
        {
            get
            {
                return outputChannel.RemoteAddress;
            }
        }

        public void Send(Message message, TimeSpan timeout)
        {
            outputChannel.Send(message, timeout);
        }
        public void Send(Message message)
        {
            outputChannel.Send(message);
        }

        public Uri Via
        {
            get
            {
                return outputChannel.Via;
            }
        }

    }
}

