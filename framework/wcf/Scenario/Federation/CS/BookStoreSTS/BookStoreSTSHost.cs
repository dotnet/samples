//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------
using System;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Security;

namespace Microsoft.Samples.Federation
{
	public class BookStoreSTSHostFactory : ServiceHostFactoryBase
	{
	    public override ServiceHostBase CreateServiceHost(string constructorString, Uri[] baseAddresses)
	    {
	        return new BookStoreSTSHost(baseAddresses);
	    }
	}
	
	class BookStoreSTSHost : ServiceHost
	{
		public BookStoreSTSHost(params Uri[] addresses) : base(typeof(BookStoreSTS), addresses)
		{
            ServiceConstants.LoadAppSettings();
            // Setting the certificateValidationMode to PeerOrChainTrust means that if the certificate 
            // is in the Trusted People store, then it will be trusted without performing a
            // validation of the certificate's issuer chain. This setting is used here for convenience 
            // so that the sample can be run without having to have certificates issued by a certificate 
            // authority (CA). This setting is less secure than the default, ChainTrust. The security 
            // implications of this setting should be carefully considered before using PeerOrChainTrust 
            // in production code. 
            this.Credentials.IssuedTokenAuthentication.CertificateValidationMode = X509CertificateValidationMode.PeerOrChainTrust;
		}
	}
}

