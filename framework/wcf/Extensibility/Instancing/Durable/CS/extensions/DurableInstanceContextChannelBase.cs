using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Diagnostics;

namespace Microsoft.ServiceModel.Samples
{
    //This is the abstract base class for all the channel 
    //implementations in DurableInstanceExtension. This class
    //provides the common functionality required for enabling the 
    //durable instancing protocol.
    abstract class DurableInstanceContextChannelBase : ChannelBase
    {
        protected IChannel innerChannel;

        // Context type used by the derived channel.
        protected ContextType contextType;

        // If the derived channel is a client side channel,
        // this member holds a reference to the remote endpoint.
        protected EndpointAddress endpointAddress;

        // If the derived channel is a client side channel,
        // this member holds the location of the context store.
        protected string contextStoreLocation;

        public DurableInstanceContextChannelBase(ChannelManagerBase channelManager, IChannel innerChannel)
            : base(channelManager)
        {
            this.innerChannel = innerChannel;
        }

        public override T GetProperty<T>()
        {
            T baseProperty = base.GetProperty<T>();
            if (baseProperty != null)
            {
                return baseProperty;
            }

            return this.innerChannel.GetProperty<T>();
        }

        #region State machine

        protected override void OnOpen(TimeSpan timeout)
        {
            innerChannel.Open(timeout);
        }

        protected override void OnClose(TimeSpan timeout)
        {
            innerChannel.Close();
        }

        protected override void OnAbort()
        {
            innerChannel.Abort();
        }

        protected override IAsyncResult OnBeginClose(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return this.innerChannel.BeginClose(timeout, callback, state);
        }

        protected override IAsyncResult OnBeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return this.innerChannel.BeginOpen(timeout, callback, state);
        }

        protected override void OnEndClose(IAsyncResult result)
        {
            this.innerChannel.EndClose(result);
        }

        protected override void OnEndOpen(IAsyncResult result)
        {
            this.innerChannel.EndOpen(result);
        }

        #endregion

        #region Durable instance context helpers.

        //Adds the durable instance context information to the 
        //outgoing messages.
        protected void ApplyContext(Message message)
        {
            IContextManager contextManager =
                ContextManagerFactory.CreateContextManager(contextType,
                this.contextStoreLocation,
                this.endpointAddress);

            contextManager.WriteContext(message);
        }

        //Reads the durable instance context id from the incoming 
        //messages.
        protected string ReadContextId(Message message)
        {
            IContextManager contextManager =
                ContextManagerFactory.CreateContextManager(contextType,
                this.contextStoreLocation,
                this.endpointAddress);

            string contextId = contextManager.ReadContext(message);

            if (contextId == null)
            {
                throw new CommunicationException(
                    ResourceHelper.GetString("ExContextNotFound"));
            }

            // Add the context id to the properties collection of the Message. 
            // This way we can pass the context id to the service model layer in a 
            // consistent manner regardless of the context type. 
            message.Properties.Add(DurableInstanceContextUtility.ContextIdProperty, contextId);

            return contextId;
        }

        #endregion
    }
}

