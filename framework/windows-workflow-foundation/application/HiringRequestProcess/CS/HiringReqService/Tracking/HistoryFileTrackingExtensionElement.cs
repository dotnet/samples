//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------
using System;
using System.Configuration;
using System.ServiceModel.Configuration;

namespace Microsoft.Samples.HiringService
{
    public class HistoryFileTrackingExtensionElement : BehaviorExtensionElement
    {
        [ConfigurationProperty("profileName", DefaultValue = null, IsKey = false, IsRequired = false)]
        public string ProfileName
        {
            get { return (string)this["profileName"]; }
            set { this["profileName"] = value; }
        }

        public HistoryFileTrackingExtensionElement() { }
        
        public override Type BehaviorType { get { return typeof(HistoryFileTrackingBehavior); } }

        protected override object CreateBehavior() { return new HistoryFileTrackingBehavior(ProfileName); }
    }
}