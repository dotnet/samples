//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------ 

using System;
using System.Configuration;
using System.ServiceModel.Configuration;

namespace Microsoft.Samples.DocumentApprovalProcess.ApprovalManager
{
    public class GenericTrackingProviderElement : BehaviorExtensionElement
    {
        public GenericTrackingProviderElement()
        {
        }

        public override Type BehaviorType
        {
            get { return typeof(GenericTrackingBehavior); }
        }

        [ConfigurationProperty("", Options = ConfigurationPropertyOptions.IsDefaultCollection)]
        public TrackingComponentElementCollection TrackingComponents
        {
            get
            {
                return (TrackingComponentElementCollection)base[""];
            }
        }

        protected override object CreateBehavior()
        {
            GenericTrackingBehavior trackingBehavior = new GenericTrackingBehavior();

            foreach (TrackingComponentElement element in this.TrackingComponents)
            {
                if (string.IsNullOrEmpty(element.Name))
                {
                    throw new Exception("Tracking encountered component with no name");
                }
                trackingBehavior.TrackingComponentElements.Add(element);
            }
            return trackingBehavior;
        }
    }
}
