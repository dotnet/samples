//------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All Rights Reserved.
//------------------------------------------------------------

using System.ServiceModel.Activation;
using System.ServiceModel.Syndication;
using System.ServiceModel.Web;

namespace Microsoft.Samples.WebRoutingIntegration
{
    
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class Channels : IFeed
    {

        public SyndicationFeedFormatter CreateFeed()
        {
            // Create a new Syndication Feed.
            SyndicationFeed feed = new SyndicationFeed("Feed Title", "A WCF Syndication Feed", null);
            feed.Items = Global.Channels;
            string query = WebOperationContext.Current.IncomingRequest.UriTemplateMatch.QueryParameters["format"];
            SyndicationFeedFormatter formatter = null;
            if (query == "atom")
            {
                formatter = new Atom10FeedFormatter(feed);
            }
            else
            {
                formatter = new Rss20FeedFormatter(feed);
            }
            return formatter;
        }
    }
}
