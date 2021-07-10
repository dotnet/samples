//  Copyright (c) Microsoft Corporation.  All Rights Reserved.
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Activation;
using System.ServiceModel.Description;

namespace Microsoft.Samples.CustomServiceHost
{
    //This class is a custom derivative of ServiceHost
    //that can automatically enabled metadata generation
    //for any service it hosts.
    class SelfDescribingServiceHost : ServiceHost
    {
        public SelfDescribingServiceHost(Type serviceType, params Uri[] baseAddresses)
            : base(serviceType, baseAddresses)
        {
        }

        //Overriding ApplyConfiguration() allows us to 
        //alter the ServiceDescription prior to opening
        //the service host. 
        protected override void ApplyConfiguration()
        {
            //First, we call base.ApplyConfiguration()
            //to read any configuration that was provided for
            //the service we're hosting. After this call,
            //this.ServiceDescription describes the service
            //as it was configured.
            base.ApplyConfiguration();

            //Now that we've populated the ServiceDescription, we can reach into it
            //and do interesting things (in this case, we'll add an instance of
            //ServiceMetadataBehavior if it's not already there.
            ServiceMetadataBehavior mexBehavior = this.Description.Behaviors.Find<ServiceMetadataBehavior>();
            if (mexBehavior == null)
            {
                mexBehavior = new ServiceMetadataBehavior();
                this.Description.Behaviors.Add(mexBehavior);
            }
            else
            {
                //Metadata behavior has already been configured, 
                //so we don't have any work to do.
                return;
            }

            //Add a metadata endpoint at each base address
            //using the "/mex" addressing convention
            foreach (Uri baseAddress in this.BaseAddresses)
            {
                if (baseAddress.Scheme == Uri.UriSchemeHttp)
                {
                    mexBehavior.HttpGetEnabled = true;
                    this.AddServiceEndpoint(ServiceMetadataBehavior.MexContractName,
                                            MetadataExchangeBindings.CreateMexHttpBinding(),
                                            "mex");
                }
                else if (baseAddress.Scheme == Uri.UriSchemeHttps)
                {
                    mexBehavior.HttpsGetEnabled = true;
                    this.AddServiceEndpoint(ServiceMetadataBehavior.MexContractName,
                                            MetadataExchangeBindings.CreateMexHttpsBinding(),
                                            "mex");
                }
                else if (baseAddress.Scheme == Uri.UriSchemeNetPipe)
                {
                    this.AddServiceEndpoint(ServiceMetadataBehavior.MexContractName,
                                            MetadataExchangeBindings.CreateMexNamedPipeBinding(),
                                            "mex");
                }
                else if (baseAddress.Scheme == Uri.UriSchemeNetTcp)
                {
                    this.AddServiceEndpoint(ServiceMetadataBehavior.MexContractName,
                                            MetadataExchangeBindings.CreateMexTcpBinding(),
                                            "mex");
                }
            }

        }
    }
}
