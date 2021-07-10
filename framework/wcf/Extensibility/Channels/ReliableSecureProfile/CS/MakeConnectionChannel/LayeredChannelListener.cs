//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System;
using System.ServiceModel.Channels;

namespace Microsoft.Samples.ReliableSecureProfile
{
    abstract class LayeredChannelListener<TChannel, TInnerChannel> : ChannelListenerBase<TChannel>
        where TChannel : class, IChannel
        where TInnerChannel : class, IChannel
    {
        IChannelListener<TInnerChannel> innerChannelListener;
        EventHandler onInnerListenerFaulted;

        protected LayeredChannelListener(BindingContext context)
            : base(context.Binding)
        {
            this.innerChannelListener = context.BuildInnerChannelListener<TInnerChannel>();
            if (this.innerChannelListener == null)
            {
                throw new ArgumentNullException("innerChannelListener");
            }
            this.onInnerListenerFaulted = new EventHandler(OnInnerListenerFaulted);
            this.innerChannelListener.Faulted += this.onInnerListenerFaulted;
        }

        public override Uri Uri
        {
            get { return this.innerChannelListener.Uri; }
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

        protected override void OnAbort()
        {
            this.innerChannelListener.Abort();
        }

        protected override TChannel OnAcceptChannel(TimeSpan timeout)
        {
            TInnerChannel innerChannel = this.innerChannelListener.AcceptChannel(timeout);
            if (innerChannel == null)
            {
                return null;
            }
            else
            {
                return WrapChannel(innerChannel);
            }
        }

        protected override IAsyncResult OnBeginAcceptChannel(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return this.innerChannelListener.BeginAcceptChannel(timeout, callback, state);
        }

        protected override IAsyncResult OnBeginClose(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return this.innerChannelListener.BeginClose(timeout, callback, state);
        }

        protected override IAsyncResult OnBeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return this.innerChannelListener.BeginOpen(timeout, callback, state);
        }

        protected override IAsyncResult OnBeginWaitForChannel(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return this.innerChannelListener.BeginWaitForChannel(timeout, callback, state);
        }

        protected override void OnClosing()
        {
            this.innerChannelListener.Faulted -= this.onInnerListenerFaulted;
            base.OnClosing();
        }

        protected override void OnClose(TimeSpan timeout)
        {
            this.innerChannelListener.Close(timeout);
        }

        protected override TChannel OnEndAcceptChannel(IAsyncResult result)
        {
            TInnerChannel innerChannel = this.innerChannelListener.EndAcceptChannel(result);
            if (innerChannel == null)
            {
                return null;
            }
            else
            {
                return WrapChannel(innerChannel);
            }
        }

        protected override void OnEndClose(IAsyncResult result)
        {
            this.innerChannelListener.EndClose(result);
        }

        protected override void OnEndOpen(IAsyncResult result)
        {
            this.innerChannelListener.EndOpen(result);
        }

        protected override bool OnEndWaitForChannel(IAsyncResult result)
        {
            return this.innerChannelListener.EndWaitForChannel(result);
        }

        protected override void OnOpen(TimeSpan timeout)
        {
            this.innerChannelListener.Open(timeout);
        }

        protected override bool OnWaitForChannel(TimeSpan timeout)
        {
            return this.innerChannelListener.WaitForChannel(timeout);
        }

        protected abstract TChannel WrapChannel(TInnerChannel innerChannel);

        void OnInnerListenerFaulted(object sender, EventArgs e)
        {
            // if our inner listener faulted, we should fault as well
            Fault();
        }
    }
}
