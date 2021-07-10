//  Copyright (c) Microsoft Corporation.  All Rights Reserved.

#region using

using System;

#endregion

namespace Microsoft.ServiceModel.Samples
{
    /// <summary>
    /// This interface defines the methods that 
    /// object pool can invoke appropriately.
    /// </summary>
    public interface IObjectControl
    {
        /// <summary>
        /// Activates the object.
        /// </summary>
        void Activate();

        /// <summary>
        /// Deactivates the object.
        /// </summary>
        void Deactivate();

        /// <summary>
        /// Gets or sets a value indicating whether the object 
        /// can be pooled or not.
        /// </summary>
        bool CanBePooled
        {
            get;
            set;
        }
    }
}
