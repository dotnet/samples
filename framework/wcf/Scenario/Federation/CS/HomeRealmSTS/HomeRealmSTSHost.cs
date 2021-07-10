//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------
using System;

using System.ServiceModel;
using System.ServiceModel.Activation;

namespace Microsoft.Samples.Federation
{
	public class HomeRealmSTSHostFactory : ServiceHostFactoryBase
	{
	    public override ServiceHostBase CreateServiceHost(string constructorString, Uri[] baseAddresses)
	    {
	        return new HomeRealmSTSHost(baseAddresses);
	    }
	}
	
	class HomeRealmSTSHost : ServiceHost
	{
        public HomeRealmSTSHost(params Uri[] addresses) : base(typeof(HomeRealmSTS), addresses)
		{
            ServiceConstants.LoadAppSettings();
		}
	}
}

