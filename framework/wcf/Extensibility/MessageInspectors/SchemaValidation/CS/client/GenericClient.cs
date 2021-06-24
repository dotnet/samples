
//  Copyright (c) Microsoft Corporation.  All Rights Reserved.

using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Microsoft.ServiceModel.Samples
{
    /// <summary>
    /// This is a generic request/reply service contract.
    /// </summary>
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName = "IGenericClient")]
    interface IGenericClient
    {
        [OperationContract(Action = "*", ReplyAction="*")]
        Message Request(Message msg);
    }

    /// <summary>
    /// Generic client proxy for request/reply message exchange.
    /// </summary>
    class GenericClient : ClientBase<IGenericClient>, IGenericClient
    {
        public GenericClient()
        {
        }

        public GenericClient(string endpointConfigurationName)
            :
                base(endpointConfigurationName)
        {
        }

        public GenericClient(string endpointConfigurationName, string remoteAddress)
            :
                base(endpointConfigurationName, remoteAddress)
        {
        }

        public GenericClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress)
            :
                base(endpointConfigurationName, remoteAddress)
        {
        }

        public GenericClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress)
            :
                base(binding, remoteAddress)
        {
        }

        #region IGenericContract Members

        public Message Request(Message msg)
        {
            return base.Channel.Request(msg);
        }

        #endregion
    }

}
