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
    public class JsonItemFormatter : SyndicationItemFormatter
    {
        [DataMember]
        JsonSyndicationItem item;
        DataContractSerializer serializer;

        public JsonItemFormatter()
            : base()
        {
            this.serializer = new DataContractSerializer(typeof(JsonSyndicationItem));
            this.item = null;
        }

        public JsonItemFormatter(SyndicationItem itemToWrite)
            : base(itemToWrite)
        {
            this.serializer = new DataContractSerializer(typeof(JsonSyndicationItem));
            this.item = new JsonSyndicationItem(itemToWrite);
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
                throw new InvalidOperationException("Trying to deserialize an item from unknown type");
            }
            this.item = (JsonSyndicationItem) this.serializer.ReadObject(reader);
            base.SetItem(this.item.ToSyndicationItem());
        }

        public override void WriteTo(XmlWriter writer)
        {
            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }
            if (this.item == null)
            {
                throw new InvalidOperationException("Trying to serialize a null item");
            }
            this.serializer.WriteObject(writer, this.item);
        }

        protected override SyndicationItem CreateItemInstance()
        {
            return new SyndicationItem();
        }

        protected override void SetItem(SyndicationItem item)
        {
            base.SetItem(item);
            this.item = new JsonSyndicationItem(item);
        }

        [OnDeserialized]
        private void PostSerialize(StreamingContext context)
        {
            base.SetItem(this.item.ToSyndicationItem());
        }
    }
}
