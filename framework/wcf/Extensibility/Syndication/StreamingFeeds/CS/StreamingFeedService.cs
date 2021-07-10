//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved. 
//----------------------------------------------------------------

using System.ServiceModel;
using System.ServiceModel.Syndication;

namespace Microsoft.Samples.StreamingFeeds
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    class StreamingFeedService : IStreamingFeedService
    {
        ItemCounter counter;

        public StreamingFeedService(ItemCounter counter)
        {
            this.counter = counter;
        }

        public Atom10FeedFormatter StreamedFeed()
        {
            SyndicationFeed feed = new SyndicationFeed("Streamed feed", "Feed to test streaming", null);

            //Generate an infinite stream of items. Both the client and the service share
            //a reference to the ItemCounter, which allows the sample to terminate
            //execution after the client has read 10 items from the stream
            ItemGenerator itemGenerator = new ItemGenerator(this.counter, 10);

            feed.Items = itemGenerator.GenerateItems();
            return feed.GetAtom10Formatter();
        }
    }

}
