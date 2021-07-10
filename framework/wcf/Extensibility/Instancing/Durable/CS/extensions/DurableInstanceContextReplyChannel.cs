using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Microsoft.ServiceModel.Samples
{
    class DurableInstanceContextReplyChannel
        : DurableInstanceContextChannelBase, IReplyChannel
    {
        IReplyChannel innerReplyChannel;

        public DurableInstanceContextReplyChannel(ChannelManagerBase channelManager,
            ContextType contextType,
            IReplyChannel innerChannel)
            : base(channelManager, innerChannel)
        {
            this.contextType = contextType;
            this.innerReplyChannel = innerChannel;
        }

        public IAsyncResult BeginReceiveRequest(TimeSpan timeout,
            AsyncCallback callback, object state)
        {
            return innerReplyChannel.BeginReceiveRequest(timeout,
                callback, state);
        }

        public IAsyncResult BeginReceiveRequest(AsyncCallback callback, object state)
        {
            return BeginReceiveRequest(DefaultReceiveTimeout,
                callback, state);
        }

        public IAsyncResult BeginTryReceiveRequest(TimeSpan timeout,
            AsyncCallback callback, object state)
        {
            return innerReplyChannel.BeginTryReceiveRequest(timeout,
                callback, state);
        }

        public IAsyncResult BeginWaitForRequest(TimeSpan timeout,
            AsyncCallback callback, object state)
        {
            return innerReplyChannel.BeginWaitForRequest(timeout, callback, state);
        }

        public RequestContext EndReceiveRequest(IAsyncResult result)
        {
            RequestContext requestContext =
                innerReplyChannel.EndReceiveRequest(result);

            // Read the context id from the incoming message.
            ReadContextId(requestContext.RequestMessage);
            return requestContext;
        }

        public bool EndTryReceiveRequest(IAsyncResult result, out RequestContext context)
        {
            bool requestAvailable =
                innerReplyChannel.EndTryReceiveRequest(result, out context);

            if (requestAvailable)
            {
                // Read the context id from the incoming message.
                ReadContextId(context.RequestMessage);
            }

            return requestAvailable;
        }

        public bool EndWaitForRequest(IAsyncResult result)
        {
            return innerReplyChannel.EndWaitForRequest(result);
        }

        public EndpointAddress LocalAddress
        {
            get
            {
                return innerReplyChannel.LocalAddress;
            }
        }

        public RequestContext ReceiveRequest(TimeSpan timeout)
        {
            RequestContext requestContext =
                innerReplyChannel.ReceiveRequest(timeout);

            // Read the context id from the incoming message.
            ReadContextId(requestContext.RequestMessage);
            return requestContext;
        }

        public RequestContext ReceiveRequest()
        {
            return ReceiveRequest(DefaultReceiveTimeout);
        }

        public bool TryReceiveRequest(TimeSpan timeout, out RequestContext context)
        {
            bool requestAvailable =
                innerReplyChannel.TryReceiveRequest(timeout, out context);

            if (requestAvailable)
            {
                // Read the context id from the incoming message.
                ReadContextId(context.RequestMessage);
            }

            return requestAvailable;
        }

        public bool WaitForRequest(TimeSpan timeout)
        {
            return innerReplyChannel.WaitForRequest(timeout);
        }
    }
}

