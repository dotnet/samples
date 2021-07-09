//  Copyright (c) Microsoft Corporation.  All Rights Reserved.

using System;
using System.ServiceModel;

namespace Microsoft.ServiceModel.Samples.Hosting
{
    class HostedUdpTransportManager : UdpTransportManager
    {
        HostedUdpTransportListener transportListener;
        public HostedUdpTransportManager(Uri listenUri)
            : base(listenUri)
        {
        }

        public void Open(int instanceId)
        {
            transportListener = new HostedUdpTransportListener(instanceId, ListenUri,
                this.OnDataReceived);
            transportListener.Open();
        }

        public void OnDataReceived(FramingData data)
        {
            ServiceHostingEnvironment.EnsureServiceAvailable(data.To.LocalPath);

            base.Dispatch(data);
        }
    }
}

