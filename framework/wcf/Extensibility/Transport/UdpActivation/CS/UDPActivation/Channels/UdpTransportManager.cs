//  Copyright (c) Microsoft Corporation.  All Rights Reserved.

using System;

namespace Microsoft.ServiceModel.Samples
{
    abstract class UdpTransportManager
    {
        static object syncRoot = new object();
        static UriLookupTable<UdpTransportManager> transportManagers = new UriLookupTable<UdpTransportManager>();
        UriLookupTable<UdpChannelListener> channelListeners = new UriLookupTable<UdpChannelListener>();
        Uri listenUri;

        public UdpTransportManager(Uri listenUri)
        {
            this.listenUri = listenUri;
        }

        public static object SyncRoot
        {
            get
            {
                return syncRoot;
            }
        }

        public Uri ListenUri
        {
            get
            {
                return listenUri;
            }
        }

        public void Register(UdpChannelListener channelListener)
        {
            channelListeners.Add(channelListener.Uri, channelListener);
        }

        public static UdpTransportManager LookUp(UdpChannelListener channelListener)
        {
            return transportManagers.Lookup(channelListener.Uri);
        }

        public static void Add(UdpTransportManager transportManager)
        {
            transportManagers.Add(transportManager.ListenUri, transportManager);
        }

        public void Dispatch(FramingData data)
        {
            UdpChannelListener channelListener = channelListeners.Lookup(data.To);
            channelListener.OnRawMessageReceived(data.Payload);
        }
    }
}

