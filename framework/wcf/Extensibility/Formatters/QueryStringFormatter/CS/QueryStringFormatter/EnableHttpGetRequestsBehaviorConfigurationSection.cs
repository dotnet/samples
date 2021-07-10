
//  Copyright (c) Microsoft Corporation.  All Rights Reserved.

using System;
using System.Configuration;
using System.ServiceModel;
using System.ServiceModel.Configuration;

namespace Microsoft.ServiceModel.Samples
{

    public class EnableHttpGetRequestsBehaviorElement : BehaviorExtensionElement
    {
        protected override object CreateBehavior()
        {
            return new EnableHttpGetRequestsBehavior();
        }

        public override Type BehaviorType
        {
            get { return typeof(EnableHttpGetRequestsBehavior); }
        }
    }
}
