//------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All Rights Reserved.
//------------------------------------------------------------

using System.Collections.Generic;
using System.ServiceModel.Syndication;

namespace Microsoft.Samples.WebRoutingIntegration
{

    class Helpers
    {

        public static List<SyndicationItem> ReadTVChannelConfigurationFromDb()
        {

            List<SyndicationItem> details = new List<SyndicationItem>();
            details.Add(new SyndicationItem("TV Channel One", "Category Adventure",null));
            details.Add(new SyndicationItem("TV Channel Two", "Category Drama", null));
            return details;
        }

        public static List<SyndicationItem> ReadMovieChannelConfigurationFromDb()
        {

            List<SyndicationItem> details = new List<SyndicationItem>();
            details.Add(new SyndicationItem("Movie One ", "Category Adventure", null));
            details.Add(new SyndicationItem("Movie Two", "Category Romance", null));
            return details;
        }
    }
}