//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System.ServiceModel;
using System.ServiceModel.Web;

namespace Microsoft.Samples.SoapAndHttpEndpoints
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IService
    {
        [OperationContract]
        [WebGet]
        string GetData();

        [OperationContract]
        [WebInvoke]
        string PutData(int value);
    }
}
