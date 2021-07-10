
//  Copyright (c) Microsoft Corporation.  All Rights Reserved.

using System;
using System.ServiceModel.Configuration;

namespace Microsoft.Samples.ClientValidation
{
    public class InternetClientValidatorElement : BehaviorExtensionElement
    {
        public override Type BehaviorType
        {
            get { return typeof(InternetClientValidatorBehavior); }
        }

        protected override object CreateBehavior()
        {
            return new InternetClientValidatorBehavior();
        }
    }
}

