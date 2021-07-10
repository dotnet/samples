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
    public class JsonSyndicationFeed
    {
        [DataMember]
        public readonly Collection<JsonSyndicationPerson> Authors;
        [DataMember]
        public readonly Uri BaseUri;
        [DataMember]
        public readonly Collection<JsonSyndicationCategory> Categories;
        [DataMember]
        public readonly Collection<JsonSyndicationPerson> Contributors;
        [DataMember]
        public readonly JsonSyndicationContent Copyright;
        [DataMember]
        public readonly JsonSyndicationContent Description;
        [DataMember]
        public readonly string Generator;
        [DataMember]
        public readonly string Id;
        [DataMember]
        public readonly Uri ImageUrl;
        [DataMember]
        public readonly Collection<JsonSyndicationItem> Items;
        [DataMember]
        public readonly string Language;
        [DataMember]
        public readonly DateTime LastUpdatedTime; 
        [DataMember]
        public readonly Collection<JsonSyndicationLink> Links;
        [DataMember]
        public readonly JsonSyndicationContent Title;

        public JsonSyndicationFeed(Collection<JsonSyndicationPerson> authors, Uri baseUri,
            Collection<JsonSyndicationCategory> categories, Collection<JsonSyndicationPerson> contributors,
            JsonSyndicationContent copyright, JsonSyndicationContent description,
            string generator, string id, Uri imageUrl, Collection<JsonSyndicationItem> items,
            string language, DateTime lastUpdatedTime, Collection<JsonSyndicationLink> links,
            JsonSyndicationContent title)
        {
            this.Authors = authors;
            this.BaseUri = baseUri;
            this.Categories = categories;
            this.Contributors = contributors;
            this.Copyright = copyright;
            this.Description = description;
            this.Generator = generator;
            this.Id = id;
            this.ImageUrl = imageUrl;
            this.Items = items;
            this.Language = language;
            this.LastUpdatedTime = lastUpdatedTime;
            this.Links = links;
            this.Title = title;
        }
        public JsonSyndicationFeed(SyndicationFeed feed)
        {
            if (feed != null)
            {
                if ((feed.Authors != null) && (feed.Authors.Count > 0))
                {
                    this.Authors = new Collection<JsonSyndicationPerson>();
                    foreach (SyndicationPerson author in feed.Authors)
                    {
                        this.Authors.Add(new JsonSyndicationPerson(author));
                    }
                }
                this.BaseUri = feed.BaseUri;
                if ((feed.Categories != null) && (feed.Categories.Count > 0))
                {
                    this.Categories = new Collection<JsonSyndicationCategory>();
                    foreach (SyndicationCategory category in feed.Categories)
                    {
                        this.Categories.Add(new JsonSyndicationCategory(category));
                    }
                }
                if ((feed.Contributors != null) && (feed.Contributors.Count > 0))
                {
                    this.Contributors = new Collection<JsonSyndicationPerson>();
                    foreach (SyndicationPerson contributor in feed.Contributors)
                    {
                        this.Contributors.Add(new JsonSyndicationPerson(contributor));
                    }
                }
                this.Copyright = new JsonSyndicationContent(feed.Copyright);
                this.Description = new JsonSyndicationContent(feed.Description);
                this.Generator = feed.Generator;
                this.Id = feed.Id;
                this.ImageUrl = feed.ImageUrl;
                if (feed.Items != null)
                {
                    this.Items = new Collection<JsonSyndicationItem>();
                    foreach (SyndicationItem item in feed.Items)
                    {
                        this.Items.Add(new JsonSyndicationItem(item));
                    }
                }
                this.Language = feed.Language;
                this.LastUpdatedTime = feed.LastUpdatedTime.DateTime; 
                if ((feed.Links != null) && (feed.Links.Count > 0))
                {
                    this.Links = new Collection<JsonSyndicationLink>();
                    foreach (SyndicationLink link in feed.Links)
                    {
                        this.Links.Add(new JsonSyndicationLink(link));
                    }
                }
                this.Title = new JsonSyndicationContent(feed.Title);
            }
        }

        public SyndicationFeed ToSyndicationFeed()
        {
            SyndicationFeed feed = new SyndicationFeed();
            if ((this.Authors != null) && (this.Authors.Count > 0))
            {
                foreach (JsonSyndicationPerson author in this.Authors)
                {
                    feed.Authors.Add(author.ToSyndicationPerson());
                }
            }
            feed.BaseUri = this.BaseUri;
            if ((this.Categories != null) && (this.Categories.Count > 0))
            {
                foreach (JsonSyndicationCategory category in this.Categories)
                {
                    feed.Categories.Add(category.ToSyndicationCategory());
                }
            }
            if ((this.Contributors != null) && (this.Contributors.Count > 0))
            {
                foreach (JsonSyndicationPerson contributor in this.Contributors)
                {
                    feed.Contributors.Add(contributor.ToSyndicationPerson());
                }
            }
            feed.Copyright = this.Copyright.ToSyndicationContent();
            feed.Description = this.Description.ToSyndicationContent();
            feed.Generator = this.Generator;
            feed.Id = this.Id;
            feed.ImageUrl = this.ImageUrl;
            if ((this.Items != null) && (this.Items.Count > 0))
            {
                Collection<SyndicationItem> items = new Collection<SyndicationItem>();
                foreach (JsonSyndicationItem item in this.Items)
                {
                    items.Add(item.ToSyndicationItem());
                }
                feed.Items = items;
            }
            feed.Language = this.Language;
            feed.LastUpdatedTime = this.LastUpdatedTime;
            if ((this.Links != null) && (this.Links.Count > 0))
            {
                foreach (JsonSyndicationLink link in this.Links)
                {
                    feed.Links.Add(link.ToSyndicationLink());
                }
            }
            feed.Title = this.Title.ToSyndicationContent();

            return feed;
        }
    }
}
