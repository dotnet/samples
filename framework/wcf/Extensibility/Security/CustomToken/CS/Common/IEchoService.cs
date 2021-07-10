//  Copyright (c) Microsoft Corporation.  All Rights Reserved.

using System;
using System.ServiceModel;

namespace Microsoft.Samples.CustomToken
{
    [ServiceContract]
    public interface IEchoService : IDisposable
    {
        [OperationContract]
        string Echo();
    }
}
