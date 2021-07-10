//------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------------------------

using System.Configuration;
using System.Globalization;
using System.ServiceModel.Configuration;

namespace Microsoft.Samples.WSStreamedHttpBinding
{
    public class WSStreamedHttpBindingCollectionElement : StandardBindingCollectionElement<WSStreamedHttpBinding, WSStreamedHttpBindingConfigurationElement>
    {
        internal static WSStreamedHttpBindingCollectionElement GetCollectionElement()
        {
            WSStreamedHttpBindingCollectionElement retVal = null;
            BindingsSection bindingsSection = (BindingsSection)ConfigurationManager.GetSection("system.serviceModel/bindings");
            if (null != bindingsSection)
            {
                retVal = bindingsSection[WSStreamedHttpBindingConstants.WSStreamedHttpBindingCollectionElementName] as WSStreamedHttpBindingCollectionElement;
            }

            return retVal;
        }

    }
}
