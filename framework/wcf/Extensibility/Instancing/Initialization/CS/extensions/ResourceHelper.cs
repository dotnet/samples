//  Copyright (c) Microsoft Corporation.  All Rights Reserved.

#region using

using System;
using System.Globalization;
using System.Resources;
using System.Reflection;
using System.Threading;

#endregion
 
namespace Microsoft.ServiceModel.Samples
{
    /// <summary>
    /// This is a helper class for accessing the 
    /// resource file.
    /// </summary>
    class ResourceHelper
    {

        #region Public Static Members

        /// <summary>
        /// Gets the string associated with the specified key
        /// from the resource file.
        /// </summary>        
        public static string GetString(string key)
        {
            ResourceManager resourceManager =
                new ResourceManager(
                "Microsoft.ServiceModel.Samples.Properties.Resources",
                Assembly.GetExecutingAssembly());

            return resourceManager.GetString(key);
        }

        #endregion
    }
}
