//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System.ServiceModel;
using System.ServiceModel.Web;
using System.Net;

namespace Microsoft.Samples.SoapAndHttpEndpoints
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class Service : IService
    {
        int value = 0;

        public string GetData()
        {
            return value.ToString();
        }

        public string PutData(int value)
        {
            // this service only allows values >= 0
            if (value < 0)
            {
                // WebFaultException is a FaultException and can be represented in SOAP and HTTP
                throw new WebFaultException<string>("The value must be >= 0", HttpStatusCode.BadRequest);
            }
            this.value = value;
            return string.Format("You put: {0}", value);
        }
    }
}
