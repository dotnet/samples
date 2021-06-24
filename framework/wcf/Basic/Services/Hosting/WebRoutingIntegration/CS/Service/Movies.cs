//------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All Rights Reserved.
//------------------------------------------------------------

using System.ServiceModel.Activation;
using System.ServiceModel.Syndication;
using System.ServiceModel.Web;

namespace Microsoft.Samples.WebRoutingIntegration
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Movies" in code, svc and config file together.
    [AspNetCompatibilityRequirements(RequirementsMode= AspNetCompatibilityRequirementsMode.Required)]
    public class Movies : IFeed
    {

        public SyndicationFeedFormatter CreateFeed()
        {
            // Create a new Syndication Feed.
            SyndicationFeed feed = new SyndicationFeed("Movies Feed", "A WCF Syndication Feed", null);

            feed.Items = Global.Movies;
            // Create a new Syndication Item.
            // Return ATOM or RSS based on query string
            // rss -> http://localhost:8732/Design_Time_Addresses/AdventureWorksSample/Feed1/
            // atom -> http://localhost:8732/Design_Time_Addresses/AdventureWorksSample/Feed1/?format=atom
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
