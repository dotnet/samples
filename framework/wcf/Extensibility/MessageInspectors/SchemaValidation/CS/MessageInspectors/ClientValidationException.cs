
//  Copyright (c) Microsoft Corporation.  All Rights Reserved.

using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;
using System.Threading;

namespace Microsoft.ServiceModel.Samples
{
    [Serializable]
    public class RequestClientValidationException : Exception
    {
        public RequestClientValidationException(string validationErrorDetail):
            base(validationErrorDetail)
        {
        }
    }

    [Serializable]
    public class ReplyClientValidationException : Exception
    {
        public ReplyClientValidationException(string validationErrorDetail):
            base(validationErrorDetail)            
        {
        }
    }
}
