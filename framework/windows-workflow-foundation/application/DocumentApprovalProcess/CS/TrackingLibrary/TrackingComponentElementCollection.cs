//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------ 

using System.Configuration;
using System.ServiceModel.Activities.Tracking.Configuration;

namespace Microsoft.Samples.DocumentApprovalProcess.ApprovalManager
{
    [ConfigurationCollection(typeof(TrackingComponentElement))]
    public sealed class TrackingComponentElementCollection : TrackingConfigurationCollection<TrackingComponentElement>
    {
        public override ConfigurationElementCollectionType CollectionType
        {
            get { return ConfigurationElementCollectionType.AddRemoveClearMap; }
        }
    }
}
