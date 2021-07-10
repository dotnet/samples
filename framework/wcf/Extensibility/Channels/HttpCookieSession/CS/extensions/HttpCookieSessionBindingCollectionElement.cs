//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System.ServiceModel.Configuration;

namespace Microsoft.Samples.HttpCookieSession
{
    // configuration for HttpCookieSessionBinding.
    public class HttpCookieSessionBindingCollectionElement  : 
        StandardBindingCollectionElement<HttpCookieSessionBinding, 
        HttpCookieSessionBindingConfigurationElement>
    {     
    }
}
