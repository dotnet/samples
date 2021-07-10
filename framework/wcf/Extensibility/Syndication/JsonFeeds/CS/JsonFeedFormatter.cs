//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved. 
//----------------------------------------------------------------

using System;
using System.Runtime.Serialization;
using System.ServiceModel.Syndication;
using System.Xml;

namespace Microsoft.Samples.JsonFeeds
{
    [DataContract(Namespace = "")]
    public class JsonFeedFormatter : SyndicationFeedFormatter
    {
        [DataMember]
        JsonSyndicationFeed feed;
        DataContractSerializer serializer;

        public JsonFeedFormatter()
            : base()
        {
            this.serializer = new DataContractSerializer(typeof(JsonSyndicationFeed));
            this.feed = null;
        }

        public JsonFeedFormatter(SyndicationFeed feedToWrite)
            : base(feedToWrite)
        {
            this.serializer = new DataContractSerializer(typeof(JsonSyndicationFeed));
            this.feed = new JsonSyndicationFeed(feedToWrite);
        }

        public override string Version
        {
            get { return "Json"; }
        }

        public override bool CanRead(XmlReader reader)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }
            return this.serializer.IsStartObject(reader);
        }

        public override void ReadFrom(XmlReader reader)
        {
            if (!CanRead(reader))
            {
                throw new InvalidOperationException("Trying to deserialize an feed from unknown type");
            }
            this.feed = (JsonSyndicationFeed) this.serializer.ReadObject(reader);
            base.SetFeed(this.feed.ToSyndicationFeed());
        }

        public override void WriteTo(XmlWriter writer)
        {
            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }
            if (this.feed == null)
            {
                throw new InvalidOperationException("Trying to serialize a null feed");
            }
            this.serializer.WriteObject(writer, this.feed);
        }

        protected override SyndicationFeed CreateFeedInstance()
        {
            return new SyndicationFeed();
        }

        protected override void SetFeed(SyndicationFeed feed)
        {
            base.SetFeed(feed);
            this.feed = new JsonSyndicationFeed(feed);
        }

        [OnDeserialized]
        private void PostSerialize( StreamingContext context )
        {
            base.SetFeed(this.feed.ToSyndicationFeed());
        }
    }
}
