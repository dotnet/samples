//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System;
using System.ServiceModel.Configuration;

namespace Microsoft.Samples.ServiceModel
{
    class EndpointValidateElement : BehaviorExtensionElement
    {
        protected override object CreateBehavior()
        {
            return new EndpointValidateBehavior();
        }

        public override Type BehaviorType
        {
            get { return typeof(EndpointValidateBehavior); }
        }
    }
}
