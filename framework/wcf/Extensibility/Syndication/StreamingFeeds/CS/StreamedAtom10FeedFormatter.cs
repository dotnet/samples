//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved. 
//----------------------------------------------------------------

using System.Collections.Generic;
using System.ServiceModel.Syndication;
using System.Xml;

namespace Microsoft.Samples.StreamingFeeds
{
    class StreamedAtom10FeedFormatter : Atom10FeedFormatter
    {
        ItemCounter counter;

        public StreamedAtom10FeedFormatter()
            : base()
        {
        }

        public StreamedAtom10FeedFormatter(SyndicationFeed feed)
            : base(feed)
        {
        }

        public StreamedAtom10FeedFormatter(ItemCounter counter)
        {
            this.counter = counter;
        }

        protected override SyndicationItem ReadItem(XmlReader reader, SyndicationFeed feed)
        {
            return base.ReadItem(reader, feed);
        }

        protected override IEnumerable<SyndicationItem> ReadItems(XmlReader reader, SyndicationFeed feed, out bool areAllItemsRead)
        {
            areAllItemsRead = false;
            return DelayReadItems(reader, feed);
        }

        private IEnumerable<SyndicationItem> DelayReadItems(XmlReader reader, SyndicationFeed feed)
        {
            while (reader.IsStartElement("entry", "http://www.w3.org/2005/Atom"))
            {
                yield return this.ReadItem(reader, feed);
            }

            reader.ReadEndElement();
        }
    }
}
