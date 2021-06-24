//  Copyright (c) Microsoft Corporation.  All Rights Reserved.

using System;
using System.Configuration;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;

namespace Microsoft.Samples.CustomToken
{
    public class EchoServiceHostFactory : ServiceHostFactoryBase
    {
        public override ServiceHostBase CreateServiceHost(string constructorString, Uri[] baseAddresses)
        {
            return new EchoServiceHost(baseAddresses);
        }
    }
	
    class EchoServiceHost : ServiceHost
    {
        string creditCardFile;

        public EchoServiceHost(params Uri[] addresses)
            : base(typeof(EchoService), addresses)
	{
            creditCardFile = ConfigurationManager.AppSettings["creditCardFile"];
            if (string.IsNullOrEmpty(creditCardFile))
            {
                throw new ConfigurationErrorsException("creditCardFile not specified in service config");
            }
            
            creditCardFile = String.Format("{0}\\{1}", System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath, creditCardFile);

	}

        override protected void InitializeRuntime()
        {
            // Create a credit card service credentials and add it to the behaviors.
            CreditCardServiceCredentials serviceCredentials = new CreditCardServiceCredentials(this.creditCardFile);
            serviceCredentials.ServiceCertificate.SetCertificate("CN=localhost", StoreLocation.LocalMachine, StoreName.My);
            this.Description.Behaviors.Remove((typeof(ServiceCredentials)));
            this.Description.Behaviors.Add(serviceCredentials);

            // Register a credit card binding for the endpoint.
            Binding creditCardBinding = BindingHelper.CreateCreditCardBinding();
            this.AddServiceEndpoint(typeof(IEchoService), creditCardBinding, string.Empty);

            base.InitializeRuntime();
        }
    }
}

