using System;
using System.Globalization;
using System.Resources;
using System.Reflection;
using System.Threading;

namespace Microsoft.ServiceModel.Samples
{
    static class ResourceHelper
    {
        public static string GetString(string key)
        {
            ResourceManager resourceManager =
                new ResourceManager(
                DurableInstanceContextUtility.ResourceFile,
                Assembly.GetExecutingAssembly());

            return resourceManager.GetString(key);
        }
    }
}

