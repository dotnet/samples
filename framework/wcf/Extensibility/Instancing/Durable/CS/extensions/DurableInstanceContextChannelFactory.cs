using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Microsoft.ServiceModel.Samples
{
    class DurableInstanceChannelFactory<TChannel> : ChannelFactoryBase<TChannel>
    {
        string contextStoreLocation;
        ContextType contextType;
        IChannelFactory<TChannel> innerChannelFactory;

        public DurableInstanceChannelFactory(string contextStoreLocation,
            ContextType contextType, 
            IChannelFactory<TChannel> innerChannelFactory)
        {
            this.contextStoreLocation = contextStoreLocation;
            this.contextType = contextType;
            this.innerChannelFactory = innerChannelFactory;
        }

        protected override TChannel OnCreateChannel(EndpointAddress address, Uri via)
        {
            TChannel innerChannel =
                innerChannelFactory.CreateChannel(address, via);

            if (typeof(TChannel) ==
                typeof(IOutputChannel))
            {
                return (TChannel)(object)new DurableInstanceContextOutputChannel(
                    this,
                    contextType,
                    (IOutputChannel)innerChannel,
                    contextStoreLocation);
            }
            else if (typeof(TChannel) ==
                typeof(IOutputSessionChannel))
            {
                return (TChannel)(object)new DurableInstanceContextOutputSessionChannel(
                    this,
                    contextType,
                    (IOutputSessionChannel)innerChannel,
                    contextStoreLocation);
            }
            else if (typeof(TChannel) ==
                typeof(IRequestChannel))
            {
                return (TChannel)(object)new DurableInstanceContextRequestChannel(
                    this,
                    contextType,
                    (IRequestChannel)innerChannel,
                    contextStoreLocation);
            }
            else if (typeof(TChannel) ==
                typeof(IRequestSessionChannel))
            {
                return (TChannel)(object)new DurableInstanceContextRequestSessionChannel(
                    this,
                    contextType,
                    (IRequestSessionChannel)innerChannel,
                    contextStoreLocation);
            }
            else if (typeof(TChannel) ==
                typeof(IDuplexChannel))
            {
                return (TChannel)(object)new DurableInstanceContextDuplexChannel(
                    this,
                    contextType,
                    (IDuplexChannel)innerChannel,
                    contextStoreLocation);
            }
            else if (typeof(TChannel) ==
                typeof(IDuplexSessionChannel))
            {
                return (TChannel)(object)new DurableInstanceContextDuplexSessionChannel(
                    this,
                    contextType,
                    (IDuplexSessionChannel)innerChannel,
                    contextStoreLocation);
            }
            else
            {
                throw new NotSupportedException(
                    ResourceHelper.GetString("ExChannelNotSupported"));
            }
        }

        protected override void OnOpen(TimeSpan timeout)
        {
            innerChannelFactory.Open(timeout);
        }

        protected override IAsyncResult OnBeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return innerChannelFactory.BeginOpen(timeout, callback, state);
        }

        protected override void OnEndOpen(IAsyncResult result)
        {
            innerChannelFactory.EndOpen(result);
        }
    }
}

