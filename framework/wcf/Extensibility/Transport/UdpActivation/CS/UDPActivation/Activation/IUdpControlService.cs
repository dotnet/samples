
//  Copyright (c) Microsoft Corporation.  All Rights Reserved.


using System;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace Microsoft.ServiceModel.Samples.Activation
{
    [DataContract]
    class ControlRegistrationData
    {
        [DataMember]
        Uri uri;

        [DataMember]
        int instanceId;

        public ControlRegistrationData() { }

        public Uri Uri
        {
            get
            {
                return this.uri;
            }

            set
            {
                this.uri = value;
            }
        }

        public int InstanceId
        {
            get
            {
                return this.instanceId;
            }

            set
            {
                this.instanceId = value;
            }
        }
    }

    [ServiceContract(SessionMode=SessionMode.Required)]
    interface IUdpControlCallback
    {
        [OperationContract]
        void Dispatch(FramingData data);
    }

    [ServiceContract(SessionMode = SessionMode.Required, CallbackContract = typeof(IUdpControlCallback))]
    interface IUdpControlRegistration
    {
        [OperationContract]
        void Register(ControlRegistrationData data);
    }
}

