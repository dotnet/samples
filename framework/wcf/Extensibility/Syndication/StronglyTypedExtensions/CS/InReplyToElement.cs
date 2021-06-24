//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved. 
//----------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Microsoft.Samples.StronglyTypedExtensions
{
    [XmlRoot(ElementName = "in-reply-to", Namespace = "http://purl.org/syndication/thread/1.0")]
    public class InReplyToElement : IXmlSerializable
    {
        internal const string ElementName = "in-reply-to";
        internal const string NsUri = "http://purl.org/syndication/thread/1.0";

        private Dictionary<XmlQualifiedName, string> extensionAttributes;

        private Collection<XElement> extensionElements;


        public InReplyToElement()
        {
            this.extensionElements = new Collection<XElement>();
            this.extensionAttributes = new Dictionary<XmlQualifiedName, string>();
        }

        public Dictionary<XmlQualifiedName, string> AttributeExtensions
        {
            get { return this.extensionAttributes; }
        }

        public Collection<XElement> ElementExtensions
        {
            get { return this.extensionElements; }
        }

        public Uri Href
        { get; set; }

        public string MediaType
        { get; set; }



        public string Ref
        { get; set; }

        public Uri Source
        { get; set; }
#region IXmlSerializable Members

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            throw new NotSupportedException();
        }

        public void ReadXml(System.Xml.XmlReader reader)
        {
            bool isEmpty = reader.IsEmptyElement;

            if (reader.HasAttributes)
            {
                for (int i = 0; i < reader.AttributeCount; i++)
                {
                    reader.MoveToNextAttribute();

                    if (reader.NamespaceURI == "")
                    {
                        if (reader.LocalName == "ref")
                        {
                            this.Ref = reader.Value;
                        }
                        else if (reader.LocalName == "href")
                        {
                            this.Href = new Uri(reader.Value);
                        }
                        else if (reader.LocalName == "source")
                        {
                            this.Source = new Uri(reader.Value);
                        }
                        else if (reader.LocalName == "type")
                        {
                            this.MediaType = reader.Value;
                        }
                        else
                        {
                            this.AttributeExtensions.Add(new XmlQualifiedName(reader.LocalName, reader.NamespaceURI), reader.Value);
                        }
                    }
                }
            }

            reader.ReadStartElement();

            if (!isEmpty)
            {
                while (reader.IsStartElement())
                {
                    ElementExtensions.Add((XElement) XElement.ReadFrom(reader));
                }
                reader.ReadEndElement();
            }
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("ThreadedItem:");
            builder.AppendFormat("Ref: {0}\n", this.Ref);
            builder.AppendFormat("Href: {0}\n", this.Href);
            builder.AppendFormat("Source: {0}\n", this.Source);
            builder.AppendFormat("Type: {0}\n", this.MediaType);

            if (this.ElementExtensions.Count > 0)
            {
                foreach (XElement elt in this.ElementExtensions)
                {
                    builder.AppendLine(elt.ToString());
                }
            }


            return builder.ToString();
        }


        public void WriteXml(System.Xml.XmlWriter writer)
        {
            if (this.Ref != null)
            {
                writer.WriteAttributeString("ref", InReplyToElement.NsUri, this.Ref);
            }
            if (this.Href != null)
            {
                writer.WriteAttributeString("href", InReplyToElement.NsUri, this.Href.ToString());
            }
            if (this.Source != null)
            {
                writer.WriteAttributeString("source", InReplyToElement.NsUri, this.Source.ToString());
            }
            if (this.MediaType != null)
            {
                writer.WriteAttributeString("type", InReplyToElement.NsUri, this.MediaType);
            }

            foreach (KeyValuePair<XmlQualifiedName, string> kvp in this.AttributeExtensions)
            {
                writer.WriteAttributeString(kvp.Key.Name, kvp.Key.Namespace, kvp.Value);
            }

            foreach (XElement element in this.ElementExtensions)
            {
                element.WriteTo(writer);
            }
        }
#endregion
    }
}
