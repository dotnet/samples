﻿//  Copyright (c) Microsoft Corporation.  All Rights Reserved.

#region using

using System.Reflection;
using System.Resources;

#endregion

namespace Microsoft.ServiceModel.Samples
{
    /// <summary>
    /// This is a helper class for accessing the 
    /// resource file.
    /// </summary>
    internal class ResourceHelper
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
