//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Microsoft.Samples.ReliableSecureProfile
{
    abstract class LayeredChannelFactory<TChannel, TInnerChannel> : ChannelFactoryBase<TChannel>
    {
        IChannelFactory<TInnerChannel> innerChannelFactory;
        EventHandler onInnerFactoryFaulted;

        protected LayeredChannelFactory(BindingContext context)
            : base(context.Binding)
        {
            this.innerChannelFactory = context.BuildInnerChannelFactory<TInnerChannel>();
            if (this.innerChannelFactory == null)
            {
                throw new ArgumentNullException("innerChannelFactory");
            }
            this.onInnerFactoryFaulted = new EventHandler(OnInnerFactoryFaulted);
            this.innerChannelFactory.Faulted += this.onInnerFactoryFaulted;
        }

        protected override TChannel OnCreateChannel(EndpointAddress address, Uri via)
        {
            TInnerChannel innerChannel = this.innerChannelFactory.CreateChannel(address, via);
            return WrapChannel(innerChannel);
        }

        public override T GetProperty<T>()
        {
            T baseProperty = base.GetProperty<T>();
            if (baseProperty != null)
            {
                return baseProperty;
            }

            return this.innerChannelFactory.GetProperty<T>();
        }

        protected override void OnClosing()
        {
            this.innerChannelFactory.Faulted -= this.onInnerFactoryFaulted;
            base.OnClosing();
        }

        protected override IAsyncResult OnBeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return this.innerChannelFactory.BeginOpen(timeout, callback, state);
        }

        protected override IAsyncResult OnBeginClose(TimeSpan timeout, AsyncCallback callback, object state)
        {
            TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
            base.OnClose(timeoutHelper.RemainingTime());
            return this.innerChannelFactory.BeginClose(timeoutHelper.RemainingTime(), callback, state);
        }

        protected override void OnEndOpen(IAsyncResult result)
        {
            this.innerChannelFactory.EndOpen(result);
        }

        protected override void OnEndClose(IAsyncResult result)
        {
            this.innerChannelFactory.EndClose(result);
        }

        protected override void OnOpen(TimeSpan timeout)
        {
            this.innerChannelFactory.Open(timeout);
        }

        protected override void OnAbort()
        {
            base.OnAbort();
            this.innerChannelFactory.Abort();
        }

        protected override void OnClose(TimeSpan timeout)
        {
            TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
            base.OnClose(timeoutHelper.RemainingTime());
            this.innerChannelFactory.Close(timeoutHelper.RemainingTime());
        }

        protected abstract TChannel WrapChannel(TInnerChannel innerChannel);

        void OnInnerFactoryFaulted(object sender, EventArgs e)
        {
            Fault();
        }
    }
}
