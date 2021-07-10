//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System.ServiceModel;

namespace Microsoft.Samples.ExtendedProtection
{
    [ServiceContract]
    public interface IGetKey
    {
        [OperationContract]
        string GetKeyFromPasscode(string passCode);
    }
}
