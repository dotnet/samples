//------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All Rights Reserved.
//------------------------------------------------------------

using System.ServiceModel;
using System.ServiceModel.Syndication;
using System.ServiceModel.Web;

namespace Microsoft.Samples.WebRoutingIntegration
{

    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IFeed1" in both code and config file together.
    [ServiceContract]
    [ServiceKnownType(typeof(Atom10FeedFormatter))]
    [ServiceKnownType(typeof(Rss20FeedFormatter))]
    public interface IFeed
    {

        [OperationContract]
        [WebGet(UriTemplate = "*", BodyStyle = WebMessageBodyStyle.Bare)]
        SyndicationFeedFormatter CreateFeed();

        // Add your service operations here
    }
}
