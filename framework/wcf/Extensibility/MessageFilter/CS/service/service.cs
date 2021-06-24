
//  Copyright (c) Microsoft Corporation.  All Rights Reserved.

using System;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Microsoft.ServiceModel.Samples
{
    [ServiceContract(Namespace="http://Microsoft.ServiceModel.Samples")]
    public interface IHello
    {
        [OperationContract]
        string Hello();
    }

    public class HelloService : IHello
    {
        public string Hello()
        {
            return "Hello";
        }
    }

}
