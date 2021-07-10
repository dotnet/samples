//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved. 
//----------------------------------------------------------------

using System;
using System.ServiceModel.Syndication;
using System.Xml;

namespace Microsoft.Samples.StronglyTypedExtensions
{
    public class ThreadedItem : SyndicationItem
    {
        private InReplyToElement inReplyTo;

        public ThreadedItem()
        {
            inReplyTo = new InReplyToElement();
        }

        public ThreadedItem(string title, string content, Uri itemAlternateLink, string id, DateTimeOffset lastUpdatedTime) :
            base(title, content, itemAlternateLink, id, lastUpdatedTime)
        {
            inReplyTo = new InReplyToElement();
        }

        public InReplyToElement InReplyTo
        {
            get { return this.inReplyTo; }
        }

        protected override bool TryParseElement(System.Xml.XmlReader reader, string version)
        {
            if (version == SyndicationVersions.Atom10 &&
                reader.NamespaceURI == InReplyToElement.NsUri &&
                reader.LocalName == InReplyToElement.ElementName)
            {
                this.inReplyTo = new InReplyToElement();

                this.InReplyTo.ReadXml(reader);

                return true;
            }
            else
            {
                return base.TryParseElement(reader, version);
            }
        }

        protected override void WriteElementExtensions(XmlWriter writer, string version)
        {
            if (this.InReplyTo != null && version == SyndicationVersions.Atom10)
            {
                writer.WriteStartElement(InReplyToElement.ElementName, InReplyToElement.NsUri);
                this.InReplyTo.WriteXml(writer);
                writer.WriteEndElement();
            }

            base.WriteElementExtensions(writer, version);
        }
    }
}
