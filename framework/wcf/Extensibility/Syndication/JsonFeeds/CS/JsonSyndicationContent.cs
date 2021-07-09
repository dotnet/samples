//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved. 
//----------------------------------------------------------------

using System.Runtime.Serialization;
using System.ServiceModel.Syndication;
using System.Text;
using System.Xml;

namespace Microsoft.Samples.JsonFeeds
{
    [DataContract(Namespace = "")]
    public class JsonSyndicationContent
    {
        [DataMember]
        public readonly string Content;

        [DataMember]
        public readonly string Type;

        public JsonSyndicationContent(string type, string content)
        {
            this.Type = type;
            this.Content = content;
        }
        public JsonSyndicationContent(SyndicationContent content)
        {
            if (content != null)
            {
                this.Type = content.Type;
                if (content is TextSyndicationContent)
                {
                    this.Content = (content as TextSyndicationContent).Text;
                }
                else if (content is UrlSyndicationContent)
                {
                    this.Content = (content as UrlSyndicationContent).Url.ToString();
                }
                else if (content is XmlSyndicationContent)
                {
                    this.Content = ExtensionToString((content as XmlSyndicationContent).Extension);
                }
                else
                {
                    this.Content = null;
                }
            }
        }

        public TextSyndicationContent ToSyndicationContent()
        {
            return new TextSyndicationContent(this.Content);
        }
        static string ExtensionToString(SyndicationElementExtension extension)
        {
            StringBuilder sb = new StringBuilder();
            XmlWriterSettings xws = new XmlWriterSettings();
            xws.ConformanceLevel = ConformanceLevel.Fragment;
            xws.Indent = true;
            xws.Encoding = Encoding.ASCII;
            XmlWriter xw = XmlWriter.Create(sb, xws);
            XmlDictionaryWriter xdw = XmlDictionaryWriter.CreateDictionaryWriter(xw);
            extension.WriteTo(xdw);
            xdw.Close();
            return sb.ToString();
        }
    }
}
