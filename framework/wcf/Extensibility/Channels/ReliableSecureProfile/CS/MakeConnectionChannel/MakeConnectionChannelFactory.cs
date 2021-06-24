//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System;
using System.ServiceModel.Channels;

namespace Microsoft.Samples.ReliableSecureProfile
{
    sealed class MakeConnectionChannelFactory<TChannel, TInnerChannel> : LayeredChannelFactory<TChannel, TInnerChannel>, IMakeConnectionFactorySettings 
        where TChannel : class, IDuplexChannel
        where TInnerChannel : class, IRequestChannel
    {
        IMakeConnectionFactorySettings settings;

        public MakeConnectionChannelFactory(MakeConnectionBindingElement bindingElement, BindingContext context) 
            : base(context)
        {
            this.settings = (IMakeConnectionFactorySettings)bindingElement;
        }

        public TimeSpan ClientPollTimeout
        {
            get
            {
                return this.settings.ClientPollTimeout;
            }
        }

        protected override TChannel WrapChannel(TInnerChannel innerChannel)
        {
            if (typeof(IDuplexChannel) == typeof(TChannel))
            {
                return (TChannel)(object)new MakeConnectionDuplexClientChannel<IRequestChannel>(this, (IRequestChannel)innerChannel);
            }
            if (typeof(IDuplexSessionChannel) == typeof(TChannel))
            {
                return (TChannel)(object)new MakeConnectionDuplexSessionClientChannel(this, (IRequestSessionChannel)innerChannel);
            }

            return default(TChannel);
        }

        sealed class MakeConnectionDuplexSessionClientChannel : MakeConnectionDuplexClientChannel<IRequestSessionChannel>, IDuplexSessionChannel
        {
            IDuplexSession session;

            public MakeConnectionDuplexSessionClientChannel(ChannelManagerBase channelManager, IRequestSessionChannel innerChannel)
                : base(channelManager, innerChannel)
            {
                this.session = MakeConnectionDuplexSession.CreateClientSession
                    (this, innerChannel.Session);
            }

            public IDuplexSession Session
            {
                get 
                {
                    return this.session;
                }
            }
        }
    }
}
