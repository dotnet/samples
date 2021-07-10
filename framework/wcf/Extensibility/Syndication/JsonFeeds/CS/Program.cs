//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved. 
//----------------------------------------------------------------

using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.ServiceModel.Syndication;
using System.Text;

namespace Microsoft.Samples.JsonFeeds
{
    class Program
    {
        static void Main(string[] args)
        {
            MemoryStream stream = new MemoryStream();

            //Simple feed with sample data
            SyndicationFeed feed = new SyndicationFeed("Custom JSON feed", "A Syndication extensibility sample", null);
            feed.LastUpdatedTime = DateTime.Now;
            feed.Items = from s in new string[] { "hello", "world" }
                         select new SyndicationItem()
                         {
                             Summary = SyndicationContent.CreatePlaintextContent(s)
                         };

            //Write the feed out to a MemoryStream as JSON
            DataContractJsonSerializer writeSerializer = new DataContractJsonSerializer(typeof(JsonFeedFormatter));
            writeSerializer.WriteObject(stream, new JsonFeedFormatter(feed));
            
            stream.Position = 0;

            //Read in the feed using the DataContractJsonSerializer
            DataContractJsonSerializer readSerializer = new DataContractJsonSerializer(typeof(JsonFeedFormatter));
            JsonFeedFormatter formatter = readSerializer.ReadObject(stream) as JsonFeedFormatter;
            SyndicationFeed feedRead = formatter.Feed;


            //Dump the JSON stream to the console
            string output = Encoding.UTF8.GetString(stream.GetBuffer());

            Console.WriteLine(output);
            Console.ReadLine();
        }
    }
}
