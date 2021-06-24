
//  Copyright (c) Microsoft Corporation.  All Rights Reserved.

using System;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Security;

namespace Microsoft.Samples.SupportingTokens
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
        public EchoServiceHost(params Uri[] addresses)
            : base(typeof(EchoService), addresses)
	{
        }

        override protected void InitializeRuntime()
        {
            // Extract the ServiceCredentials behavior or create one.
            ServiceCredentials serviceCredentials = this.Description.Behaviors.Find<ServiceCredentials>();
            if (serviceCredentials == null)
            {
                serviceCredentials = new ServiceCredentials();
                this.Description.Behaviors.Add(serviceCredentials);
            }

            // Set the service certificate.
            serviceCredentials.ServiceCertificate.SetCertificate("CN=localhost");

            /*
            Setting the CertificateValidationMode to PeerOrChainTrust means that if the certificate 
            is in the Trusted People store, then it is trusted without performing a
            validation of the certificate's issuer chain. This setting is used here for convenience so that the 
            sample can be run without having to have certificates issued by a certificate authority (CA).
            This setting is less secure than the default, ChainTrust. The security implications of this 
            setting should be carefully considered before using PeerOrChainTrust in production code. 
            */
            serviceCredentials.ClientCertificate.Authentication.CertificateValidationMode = X509CertificateValidationMode.PeerOrChainTrust;

            // Create the custom binding and add an endpoint to the service.
            Binding multipleTokensBinding = BindingHelper.CreateMultiFactorAuthenticationBinding();
            this.AddServiceEndpoint(typeof(IEchoService), multipleTokensBinding, string.Empty);
			
            base.InitializeRuntime();
        }
    }
}

