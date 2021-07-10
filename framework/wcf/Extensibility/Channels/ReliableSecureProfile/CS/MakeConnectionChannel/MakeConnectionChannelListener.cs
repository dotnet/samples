//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System;
using System.ServiceModel.Channels;

namespace Microsoft.Samples.ReliableSecureProfile
{
    sealed class MakeConnectionChannelListener<TChannel, TInnerChannel> : LayeredChannelListener<TChannel, TInnerChannel>, IMakeConnectionListenerSettings
        where TChannel : class, IDuplexChannel
        where TInnerChannel : class, IReplyChannel
    {
        IMakeConnectionListenerSettings settings;

        public MakeConnectionChannelListener(MakeConnectionBindingElement bindingElement, BindingContext context) 
            : base(context)
        {
            this.settings = (IMakeConnectionListenerSettings)bindingElement;
        }

        public TimeSpan ServerPollTimeout
        {
            get
            {
                return this.settings.ServerPollTimeout;
            }
        }
        
        public override T GetProperty<T>()
        {
            if (typeof(T) == typeof(IChannelListener<TChannel>))
            {
                return (T)(object)this;
            }

            return base.GetProperty<T>();
        }
 
        protected override TChannel WrapChannel(TInnerChannel innerChannel)
        {
            if (typeof(IDuplexChannel) == typeof(TChannel))
            {
                return (TChannel)(object)new MakeConnectionDuplexServiceChannel<IReplyChannel>(this, (IReplyChannel)innerChannel);
            }
            if (typeof(IDuplexSessionChannel) == typeof(TChannel))
            {
                return (TChannel)(object)new MakeConnectionDuplexSessionServiceChannel(this, (IReplySessionChannel)innerChannel);
            }

            return default(TChannel);
        }
    }
}
