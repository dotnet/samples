//  Copyright (c) Microsoft Corporation.  All Rights Reserved.

using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading;
using Microsoft.ServiceModel.Samples.Activation;

namespace Microsoft.ServiceModel.Samples.Hosting
{
    [CallbackBehavior(ConcurrencyMode=ConcurrencyMode.Multiple)]
    class HostedUdpTransportListener : IUdpControlCallback
    {
        const int MaxControlReceivedMessageSize = 1 << 20;
        ChannelFactory<IUdpControlRegistration> channelFactory;
        IDuplexSessionChannel controlChannel;
        int instanceId;
        Uri uri;
        DataReceivedCallback dataReceivedCallback;

        public HostedUdpTransportListener(int instanceId, Uri uri, DataReceivedCallback dataReceivedCallback)
        {
            this.instanceId = instanceId;
            this.uri = uri;
            this.dataReceivedCallback = dataReceivedCallback;
        }

        public void Open()
        {
            // FUTURE: For security purpose, we need to set the right ACL to the Named-Pipe. Unfortunately
            // this is not supported in current WCF API.
            NetNamedPipeBinding binding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.None);
            binding.MaxReceivedMessageSize = MaxControlReceivedMessageSize;
            binding.ReaderQuotas.MaxArrayLength = MaxControlReceivedMessageSize;
            channelFactory = new DuplexChannelFactory<IUdpControlRegistration>(
                new InstanceContext(null, this),
                binding,
                new EndpointAddress(HostedUdpConstants.ControlServiceAddress));

            IUdpControlRegistration controlRegistration = channelFactory.CreateChannel();
            controlChannel = controlRegistration as IDuplexSessionChannel;
            
            ControlRegistrationData data = new ControlRegistrationData();
            data.Uri = uri;
            data.InstanceId = instanceId;

            controlRegistration.Register(data);
        }

        public void Dispatch(FramingData data)
        {
            // Dispatch the message on a new thread
            ThreadPool.QueueUserWorkItem(new WaitCallback(OnDispatchMessage), data);
        }

        public void OnDispatchMessage(object state)
        {
            if (dataReceivedCallback != null)
            {
                dataReceivedCallback((FramingData)state);
            }
        }

        public void Close()
        {
            if (controlChannel != null)
            {
                controlChannel.Close();
                controlChannel = null;
            }

            if (channelFactory != null)
            {
                channelFactory.Close();
                channelFactory = null;
            }
        }
    }
}

