//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved. 
//----------------------------------------------------------------

using System;
using System.Runtime.Serialization;
using System.ServiceModel.Syndication;

namespace Microsoft.Samples.JsonFeeds
{
    [DataContract(Namespace = "")]
    public class JsonSyndicationLink
    {
        [DataMember]
        public readonly Uri BaseUri;
        [DataMember]
        public readonly long Length;
        [DataMember]
        public readonly string MediaType;
        [DataMember]
        public readonly string RelationshipType;
        [DataMember]
        public readonly string Title;
        [DataMember]
        public readonly Uri Uri;

        public JsonSyndicationLink(Uri baseUri, Uri uri, string relationshipType, string title, string mediaType,
            long length)
        {
            this.Length = length;
            this.MediaType = mediaType;
            this.RelationshipType = relationshipType;
            this.Title = title;
            this.Uri = baseUri;
            this.Uri = uri;
        }
        public JsonSyndicationLink(SyndicationLink link)
        {
            if (link != null)
            {
                this.Length = link.Length;
                this.MediaType = link.MediaType;
                this.RelationshipType = link.RelationshipType;
                this.Title = link.Title;
                this.Uri = link.BaseUri;
                this.Uri = link.Uri;
            }
        }

        public SyndicationLink ToSyndicationLink()
        {
            SyndicationLink result = new SyndicationLink(this.Uri, this.RelationshipType, this.Title, this.MediaType, this.Length);
            result.BaseUri = this.BaseUri;
            return result;
        }
    }
}
