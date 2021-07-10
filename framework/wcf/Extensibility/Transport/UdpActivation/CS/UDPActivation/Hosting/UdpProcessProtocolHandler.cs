
//  Copyright (c) Microsoft Corporation.  All Rights Reserved.


using System.Collections.Generic;
using System.Diagnostics;
using System.Web.Hosting;
using Microsoft.ServiceModel.Samples.Activation;

namespace Microsoft.ServiceModel.Samples.Hosting
{
    class UdpProcessProtocolHandler : ProcessProtocolHandler
    {
        IAdphManager adphManager;
        Dictionary<int, AppInstance> appInstanceTable = new Dictionary<int, AppInstance>();

        public override void StartListenerChannel(IListenerChannelCallback listenerChannelCallback, IAdphManager adphManager)
        {
            int channelId = listenerChannelCallback.GetId();
            AppInstance appInstance;
            if (!this.appInstanceTable.TryGetValue(channelId, out appInstance))
            {
                lock (ThisLock)
                {
                    if (!this.appInstanceTable.TryGetValue(channelId, out appInstance))
                    {
                        int length = listenerChannelCallback.GetBlobLength();
                        byte[] blob = new byte[length];
                        listenerChannelCallback.GetBlob(blob, ref length);
                        appInstance = AppInstance.Deserialize(blob);
                        appInstanceTable.Add(channelId, appInstance);
                    }
                }
            }

            if (this.adphManager == null)
            {
                this.adphManager = adphManager;
            }

            Debug.Assert(channelId == appInstance.Id);
            this.adphManager.StartAppDomainProtocolListenerChannel(appInstance.AppKey,
                UdpConstants.Scheme, listenerChannelCallback);
        }

        public override void StopListenerChannel(int listenerChannelId, bool immediate)
        {
            AppInstance appInstance = this.appInstanceTable[listenerChannelId];
            this.adphManager.StopAppDomainProtocolListenerChannel(appInstance.AppKey,
                UdpConstants.Scheme, listenerChannelId, immediate);
        }

        public override void StopProtocol(bool immediate)
        {
            // FUTURE: we need to clean up the transport manager so that we won't receive new messages since after.
        }

        object ThisLock
        {
            get
            {
                return this.appInstanceTable;
            }
        }
    }
}

