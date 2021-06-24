//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System.ComponentModel;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace Microsoft.Samples.BasicHttpService
{
    [ServiceContract]
    public interface IService
    {
        [Description("Simple echo operation over HTTP GET")]
        [WebGet]
        string EchoWithGet(string s);

        [Description("Simple echo operation over HTTP POST")]
        [WebInvoke]
        string EchoWithPost(string s);
    }
}
