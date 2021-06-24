//------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------------------------

using System.ServiceModel;

namespace Microsoft.Samples.ReliableSecureProfile
{
    [ServiceContract(CallbackContract = typeof(IProcessDataDuplexCallBack))]
    public interface IProcessDataDuplex
    {
        [OperationContract(IsOneWay=true)]
        void ProcessData(string rawData);
    }

    public interface IProcessDataDuplexCallBack
    {
        [OperationContract(IsOneWay = true)]
        void SendProcessedData(string processedData);
    }
}
