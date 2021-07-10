using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Microsoft.ServiceModel.Samples
{
    class DurableInstanceContextReplySessionChannel : DurableInstanceContextChannelBase,
        IReplySessionChannel
    {
        IReplySessionChannel innerReplySessionChannel;

        // Indicates whether the current message is the first
        // message in a session or not. 
        bool isFirstMessage;

        // Lock should be acquired on this object before changing the 
        // state of this channel.
        object stateLock;

        // Once the context id is read out from the first incoming message,
        // it will be cached here.
        string contextId;

        public DurableInstanceContextReplySessionChannel(ChannelManagerBase channelManager,
            ContextType contextType,
            IReplySessionChannel innerChannel)
            : base(channelManager, innerChannel)
        {
            this.contextType = contextType;
            this.innerReplySessionChannel = innerChannel;
            this.isFirstMessage = true;
            this.stateLock = new object();
        }

        public IInputSession Session
        {
            get
            {
                return this.innerReplySessionChannel.Session;
            }
        }

        public IAsyncResult BeginReceiveRequest(TimeSpan timeout,
            AsyncCallback callback, object state)
        {
            return innerReplySessionChannel.BeginReceiveRequest(timeout,
               callback, state);
        }

        public IAsyncResult BeginReceiveRequest(AsyncCallback callback,
            object state)
        {
            return BeginReceiveRequest(DefaultReceiveTimeout,
                callback, state);
        }

        public IAsyncResult BeginTryReceiveRequest(TimeSpan timeout,
            AsyncCallback callback, object state)
        {
            return innerReplySessionChannel.BeginTryReceiveRequest(timeout,
                callback, state);
        }

        public IAsyncResult BeginWaitForRequest(TimeSpan timeout,
           AsyncCallback callback, object state)
        {
            return innerReplySessionChannel.BeginWaitForRequest(
                timeout, callback, state);
        }

        public RequestContext EndReceiveRequest(IAsyncResult result)
        {
            RequestContext requestContext =
                innerReplySessionChannel.EndReceiveRequest(result);

            ReadAndAddContextIdToMessage(requestContext.RequestMessage);
            return requestContext;
        }

        public bool EndTryReceiveRequest(IAsyncResult result,
           out RequestContext context)
        {
            bool requestAvailable =
               innerReplySessionChannel.EndTryReceiveRequest(result, out context);

            if (requestAvailable && context != null)
            {
                ReadAndAddContextIdToMessage(context.RequestMessage);
            }

            return requestAvailable;
        }

        public bool EndWaitForRequest(IAsyncResult result)
        {
            return innerReplySessionChannel.EndWaitForRequest(result);
        }

        public EndpointAddress LocalAddress
        {
            get { return innerReplySessionChannel.LocalAddress; }
        }

        public RequestContext ReceiveRequest(TimeSpan timeout)
        {
            RequestContext requestContext =
                innerReplySessionChannel.ReceiveRequest(timeout);

            ReadAndAddContextIdToMessage(requestContext.RequestMessage);
            return requestContext;
        }

        public RequestContext ReceiveRequest()
        {
            return ReceiveRequest(DefaultReceiveTimeout);
        }

        public bool TryReceiveRequest(TimeSpan timeout, out RequestContext context)
        {
            bool requestAvailable =
               innerReplySessionChannel.TryReceiveRequest(timeout, out context);

            if (requestAvailable)
            {
                ReadAndAddContextIdToMessage(context.RequestMessage);
            }

            return requestAvailable;
        }

        public bool WaitForRequest(TimeSpan timeout)
        {
            return innerReplySessionChannel.WaitForRequest(timeout);
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

