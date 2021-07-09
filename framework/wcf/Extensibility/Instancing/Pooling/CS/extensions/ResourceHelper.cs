//  Copyright (c) Microsoft Corporation.  All Rights Reserved.

using System;
using System.Globalization;
using System.Resources;
using System.Reflection;
using System.Threading;

namespace Microsoft.ServiceModel.Samples
{
    class ResourceHelper
    {
        // Gets the string associated with the specified key from the resource file
        public static string GetString(string key)
        {
            ResourceManager resourceManager = new ResourceManager(
                "Microsoft.ServiceModel.Samples.Properties.Resources",
                Assembly.GetExecutingAssembly());

            return resourceManager.GetString(key);
        }
    }
}
