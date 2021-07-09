//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved. 
//----------------------------------------------------------------

using System;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using System.ServiceModel.Syndication;

namespace Microsoft.Samples.JsonFeeds
{
    [DataContract( Namespace="" )]
    public class JsonSyndicationItem
    {
        [DataMember]
        public readonly Collection<JsonSyndicationPerson> Authors;
        [DataMember]
        public readonly Uri BaseUri;
        [DataMember]
        public readonly Collection<JsonSyndicationCategory> Categories;
        [DataMember]
        public readonly JsonSyndicationContent Content;
        [DataMember]
        public readonly Collection<JsonSyndicationPerson> Contributors;
        [DataMember]
        public readonly JsonSyndicationContent Copyright;
        [DataMember]
        public readonly string Id;
        [DataMember]
        public readonly DateTime LastUpdatedTime;
        [DataMember]
        public readonly Collection<JsonSyndicationLink> Links;
        [DataMember]
        public readonly DateTime PublishDate;
        [DataMember]
        public readonly JsonSyndicationContent Summary;
        [DataMember]
        public readonly JsonSyndicationContent Title;

        public JsonSyndicationItem(Collection<JsonSyndicationPerson> authors, Uri baseUri,
            Collection<JsonSyndicationCategory> categories, JsonSyndicationContent content,
            Collection<JsonSyndicationPerson> contributors, JsonSyndicationContent copyright,
            string id, DateTime lastUpdatedTime, Collection<JsonSyndicationLink> links,
            DateTime publishDate, JsonSyndicationContent summary, JsonSyndicationContent title)
        {
            this.Authors = authors;
            this.BaseUri = baseUri;
            this.Categories = categories;
            this.Content = content;
            this.Contributors = contributors;
            this.Copyright = copyright;
            this.Id = id;
            this.LastUpdatedTime = lastUpdatedTime;
            this.Links = links;
            this.PublishDate = publishDate;
            this.Summary = summary;
            this.Title = title;
        }
        public JsonSyndicationItem(SyndicationItem item)
        {
            if (item != null)
            {
                if ((item.Authors != null) && (item.Authors.Count > 0))
                {
                    this.Authors = new Collection<JsonSyndicationPerson>();
                    foreach (SyndicationPerson author in item.Authors)
                    {
                        this.Authors.Add(new JsonSyndicationPerson(author));
                    }
                }
                this.BaseUri = item.BaseUri;
                if ((item.Categories != null) && (item.Categories.Count > 0))
                {
                    this.Categories = new Collection<JsonSyndicationCategory>();
                    foreach (SyndicationCategory category in item.Categories)
                    {
                        this.Categories.Add(new JsonSyndicationCategory(category));
                    }
                }
                this.Content = new JsonSyndicationContent(item.Content);
                if ((item.Contributors != null) && (item.Contributors.Count > 0))
                {
                    this.Contributors = new Collection<JsonSyndicationPerson>();
                    foreach (SyndicationPerson contributor in item.Contributors)
                    {
                        this.Contributors.Add(new JsonSyndicationPerson(contributor));
                    }
                }
                this.Copyright = new JsonSyndicationContent(item.Copyright);
                this.Id = item.Id;
                this.LastUpdatedTime = item.LastUpdatedTime.DateTime;
                if ((item.Links != null) && (item.Links.Count > 0))
                {
                    this.Links = new Collection<JsonSyndicationLink>();
                    foreach (SyndicationLink link in item.Links)
                    {
                        this.Links.Add(new JsonSyndicationLink(link));
                    }
                }
                this.PublishDate = item.PublishDate.DateTime;
                this.Summary = new JsonSyndicationContent(item.Summary);
                this.Title = new JsonSyndicationContent(item.Title);
            }
        }

        public SyndicationItem ToSyndicationItem()
        {
            SyndicationItem item = new SyndicationItem();
            if ((this.Authors != null) && (this.Authors.Count > 0))
            {
                foreach (JsonSyndicationPerson author in this.Authors)
                {
                    item.Authors.Add(author.ToSyndicationPerson());
                }
            }
            item.BaseUri = this.BaseUri;
            if ((this.Categories != null) && (this.Categories.Count > 0))
            {
                foreach (JsonSyndicationCategory category in this.Categories)
                {
                    item.Categories.Add(category.ToSyndicationCategory());
                }
            }
            item.Content = this.Content.ToSyndicationContent();
            if ((this.Contributors != null) && (this.Contributors.Count > 0))
            {
                foreach (JsonSyndicationPerson contributor in this.Contributors)
                {
                    item.Contributors.Add(contributor.ToSyndicationPerson());
                }
            }
            item.Copyright = this.Copyright.ToSyndicationContent();
            item.Id = this.Id;
            item.LastUpdatedTime = this.LastUpdatedTime;
            if ((this.Links != null) && (this.Links.Count > 0))
            {
                foreach (JsonSyndicationLink link in this.Links)
                {
                    item.Links.Add(link.ToSyndicationLink());
                }
            }
            item.PublishDate = this.PublishDate;
            item.Summary = this.Summary.ToSyndicationContent();
            item.Title = this.Title.ToSyndicationContent();

            return item;
        }
    }
}
