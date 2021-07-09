#region using

using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Channels;

#endregion

namespace Microsoft.ServiceModel.Samples
{
    class DurableInstanceChannelListener<TChannel> :
        ChannelListenerBase<TChannel>
        where TChannel : class, IChannel
    {

        ContextType contextType;
        string contextStoreLocation = null;
        IChannelListener<TChannel> innerChannelListener;

        public DurableInstanceChannelListener(
            ContextType contextType, BindingContext context)
        {
            this.contextType = contextType;
            this.innerChannelListener = context.BuildInnerChannelListener<TChannel>();
            if (this.innerChannelListener == null)
            {
                throw new InvalidOperationException(
                    "DurableInstanceChannelListener requires an inner IChannelListener.");
            }
        }

        //Creates an instance of DurableInstanceChannelListener class.
        //This overloaded constructor accepts a parameter which specifies 
        //the context store location. This is necessary for duplex message
        //exchanges.
        public DurableInstanceChannelListener(
            ContextType contextType,
            string contextStoreLocation, BindingContext context)
            : this(contextType, context)
        {
            this.contextStoreLocation = contextStoreLocation;
        }

        public override T GetProperty<T>()
        {
            T baseProperty = base.GetProperty<T>();
            if (baseProperty != null)
            {
                return baseProperty;
            }

            return this.innerChannelListener.GetProperty<T>();
        }

        protected override void OnOpen(TimeSpan timeout)
        {
            this.innerChannelListener.Open(timeout);
        }

        protected override IAsyncResult OnBeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return this.innerChannelListener.BeginOpen(timeout, callback, state);
        }

        protected override void OnEndOpen(IAsyncResult result)
        {
            this.innerChannelListener.EndOpen(result);
        }

        protected override void OnClose(TimeSpan timeout)
        {
            this.innerChannelListener.Close(timeout);
        }

        protected override IAsyncResult OnBeginClose(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return this.innerChannelListener.BeginClose(timeout, callback, state);
        }

        protected override void OnEndClose(IAsyncResult result)
        {
            this.innerChannelListener.EndClose(result);
        }

        protected override void OnAbort()
        {
            this.innerChannelListener.Abort();
        }

        protected override TChannel OnAcceptChannel(TimeSpan timeout)
        {
            TChannel innerChannel = this.innerChannelListener.AcceptChannel(timeout);
            return GetChannelWrapper(innerChannel);
        }

        protected override IAsyncResult OnBeginAcceptChannel(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return this.innerChannelListener.BeginAcceptChannel(timeout, callback, state);
        }

        protected override TChannel OnEndAcceptChannel(IAsyncResult result)
        {
            TChannel innerChannel = this.innerChannelListener.EndAcceptChannel(result);
            return GetChannelWrapper(innerChannel);
        }

        protected override bool OnWaitForChannel(TimeSpan timeout)
        {
            return this.innerChannelListener.WaitForChannel(timeout);
        }

        protected override IAsyncResult OnBeginWaitForChannel(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return this.innerChannelListener.BeginWaitForChannel(timeout, callback, state);
        }

        protected override bool OnEndWaitForChannel(IAsyncResult result)
        {
            return this.innerChannelListener.EndWaitForChannel(result);
        }

        public override Uri Uri
        {
            get
            {
                return innerChannelListener.Uri;
            }
        }

        TChannel GetChannelWrapper(TChannel innerChannel)
        {
            if (innerChannel == null)
            {
                return null;
            }

            if (typeof(TChannel) == typeof(IInputChannel))
            {
                return (TChannel)(object)new DurableInstanceContextInputChannel(this,
                    contextType,
                    (IInputChannel)innerChannel);
            }
            else if (typeof(TChannel) == typeof(IInputSessionChannel))
            {
                return (TChannel)(object)new DurableInstanceContextInputSessionChannel(this,
                    contextType,
                    (IInputSessionChannel)innerChannel);
            }
            else if (typeof(TChannel) == typeof(IReplyChannel))
            {
                return (TChannel)(object)new DurableInstanceContextReplyChannel(this,
                    contextType,
                    (IReplyChannel)innerChannel);
            }
            else if (typeof(TChannel) == typeof(IReplySessionChannel))
            {
                return (TChannel)(object)new DurableInstanceContextReplySessionChannel(
                    this,
                    contextType,
                    (IReplySessionChannel)innerChannel);
            }
            else if (typeof(TChannel) == typeof(IDuplexChannel))
            {
                return (TChannel)(object)new DurableInstanceContextDuplexChannel(this,
                    contextType,
                    (IDuplexChannel)innerChannel,
                    contextStoreLocation);
            }
            else if (typeof(TChannel) == typeof(IDuplexSessionChannel))
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

    }
}

