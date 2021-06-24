//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved. 
//----------------------------------------------------------------

using System;
using System.ServiceModel;
using System.ServiceModel.Syndication;
using System.ServiceModel.Web;
using System.Xml;

namespace Microsoft.Samples.StreamingFeeds
{
    class Program
    {
        static void Main(string[] args)
        {
            ItemCounter counter = new ItemCounter();

            using (WebServiceHost host = new WebServiceHost(new StreamingFeedService(counter), new Uri("http://localhost:8000/Service")))
            {
                WebHttpBinding binding = new WebHttpBinding();

                binding.TransferMode = TransferMode.StreamedResponse;
                host.AddServiceEndpoint(typeof(IStreamingFeedService), binding, "Feeds");

                host.Open();

                XmlReader reader = XmlReader.Create("http://localhost:8000/Service/Feeds/StreamedFeed");
                StreamedAtom10FeedFormatter formatter = new StreamedAtom10FeedFormatter(counter);

                Console.WriteLine("Reading stream from server");

                formatter.ReadFrom(reader);
                SyndicationFeed feed = formatter.Feed;

                foreach (SyndicationItem item in feed.Items)
                {
                    //This sample is implemented such that the server will generate an infinite stream of items;
                    //it only stops after the client reads 10 items
                    counter.Increment();
                }

                Console.WriteLine("CLIENT: read total of {0} items", counter.GetCount());
                Console.WriteLine("Press any key to terminate");
                Console.ReadLine();
            }
        }
    }
}
