//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Microsoft.Samples.ReliableSecureProfile
{
    public sealed class MakeConnectionDuplexSessionServiceChannel : MakeConnectionDuplexServiceChannel<IReplySessionChannel>, IDuplexSessionChannel
    {
        IDuplexSession session;
        EndpointAddress remoteAddress;

        public MakeConnectionDuplexSessionServiceChannel(ChannelManagerBase channelManager, IReplySessionChannel innerChannel)
            : base(channelManager, innerChannel)
        {
            this.session = MakeConnectionDuplexSession.CreateServerSession
                (this, innerChannel.Session);
        }

        public override EndpointAddress RemoteAddress
        {
            get
            {
                return this.remoteAddress;
            }
        }

        public IDuplexSession Session
        {
            get
            {
                return this.session;
            }
        }

        public override bool IsDatagram
        {
            get
            {
                return false;
            }
        }

        protected override void OnRemoteAddressAcquired(EndpointAddress remoteAddress)
        {
            this.remoteAddress = remoteAddress;
        }
    }
}
