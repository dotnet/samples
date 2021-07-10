//  Copyright (c) Microsoft Corporation.  All Rights Reserved.
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Text;

namespace Microsoft.Samples.CustomServiceHost
{
	public class SelfDescribingServiceHostFactory : ServiceHostFactory
    {
        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            //All the custom factory does is return a new instance
            //of our custom host class. The bulk of the custom logic should
            //live in the custom host (as opposed to the factory) for maximum
            //reuse value.
            return new SelfDescribingServiceHost(serviceType, baseAddresses);
        }
    }
}
