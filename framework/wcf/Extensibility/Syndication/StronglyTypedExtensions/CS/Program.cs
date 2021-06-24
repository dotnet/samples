//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved. 
//----------------------------------------------------------------

using System;
using System.IO;
using System.ServiceModel.Syndication;
using System.Xml;

namespace Microsoft.Samples.StronglyTypedExtensions
{
    class Program
    {
        const string InstanceDocument =
            @"<feed xmlns=""http://www.w3.org/2005/Atom""
         xmlns:thr=""http://purl.org/syndication/thread/1.0"">
     <id>http://www.example.org/myfeed</id>
     <title>My Example Feed</title>
     <updated>2005-07-28T12:00:00Z</updated>
     <link href=""http://www.example.org/myfeed"" />
     <author><name>James</name></author>
     <entry>
       <id>tag:example.org,2005:1</id>
       <title>My original entry</title>
       <updated>2006-03-01T12:12:12Z</updated>
       <link
         type=""application/xhtml+xml""
         href=""http://www.example.org/entries/1"" />
       <summary>This is my original entry</summary>
     </entry>
     <entry>
       <id>tag:example.org,2005:1,1</id>
       <title>A response to the original</title>
       <updated>2006-03-01T12:12:12Z</updated>
       <link href=""http://www.example.org/entries/1/1"" />
       <thr:in-reply-to
         ref=""tag:example.org,2005:1""
         type=""application/xhtml+xml""
         href=""http://www.example.org/entries/1""/>
       <summary>This is a response to the original entry</summary>
     </entry>
     <entry>
       <id>tag:example.org,2005:1,2</id>
       <title>Another response to the original</title>
       <updated>2006-03-01T12:12:13Z</updated>
       <link href=""http://www.example.org/entries/1/2"" />
       <thr:in-reply-to
         ref=""tag:example.org,2005:1""
         type=""application/xhtml+xml""
         href=""http://www.example.org/entries/1"">
             <anotherElement>Some more data</anotherElement>
             <aDifferentElement>Even more data</aDifferentElement>
       </thr:in-reply-to>
       <summary>This is a response to the original entry</summary>
     </entry>
   </feed>";

        static void Main(string[] args)
        {
            XmlReader reader = XmlTextReader.Create(new StringReader(InstanceDocument));
            ThreadedFeed feed = SyndicationFeed.Load<ThreadedFeed>(reader);

            foreach (ThreadedItem i in feed.ThreadedItems)
            {
                //ThreadedItem exposes a strongly-typed member called InReplyTo
                //holding the extension data.
                Console.WriteLine(i.InReplyTo.ToString());
            }

            Console.WriteLine("");

            Atom10FeedFormatter formatter = feed.GetAtom10Formatter();
            XmlWriter writer = XmlTextWriter.Create(Console.Out, new XmlWriterSettings() { Indent = true });
            formatter.WriteTo(writer);
            writer.Flush();

            Console.ReadLine();
        }
    }
}
