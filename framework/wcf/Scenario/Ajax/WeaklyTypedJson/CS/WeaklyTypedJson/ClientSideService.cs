//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Web;

namespace Microsoft.Samples.WeaklyTypedJson
{
    [ServiceContract]
    interface IClientSideProfileService
    {
        // There is no need to write a DataContract for the complex type returned by the service.
        // The client will use a JsonObject to browse the JSON in the received message.

        [WebGet(ResponseFormat = WebMessageFormat.Json)]
        Message GetMemberProfile();
    }
}
