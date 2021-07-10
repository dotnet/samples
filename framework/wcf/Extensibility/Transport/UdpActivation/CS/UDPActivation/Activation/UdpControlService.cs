
//  Copyright (c) Microsoft Corporation.  All Rights Reserved.


using System.ServiceModel;

namespace Microsoft.ServiceModel.Samples.Activation
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession,
        ConcurrencyMode=ConcurrencyMode.Multiple)]
    class UdpControlService : IUdpControlRegistration
    {
        App app;
        IUdpControlCallback controlCallback;
        ControlRegistrationData data;
        public void Register(ControlRegistrationData data)
        {
            this.data = data;
            UdpListenerAdapter.Register(data, this);
            controlCallback = OperationContext.Current.GetCallbackChannel<IUdpControlCallback>();
        }

        internal void SetApp(App app)
        {
            this.app = app;
        }

        internal void Dispatch()
        {
            FramingData framingData = app.EndDequeue(app.BeginDequeue());
            controlCallback.Dispatch(framingData);

            UdpListenerAdapter.Dispatch(data, this);
        }
    }
}

