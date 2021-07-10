//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved. 
//----------------------------------------------------------------

using System.Runtime.Serialization;
using System.ServiceModel.Syndication;

namespace Microsoft.Samples.JsonFeeds
{
    [DataContract(Namespace = "")]
    public class JsonSyndicationCategory
    {
        [DataMember]
        public readonly string Label;
        [DataMember]
        public readonly string Name;
        [DataMember]
        public readonly string Scheme;

        public JsonSyndicationCategory(string name, string scheme, string label)
        {
            this.Label = label;
            this.Name = name;
            this.Scheme = scheme;
        }
        public JsonSyndicationCategory(SyndicationCategory category)
        {
            if (category != null)
            {
                this.Label = category.Label;
                this.Name = category.Name;
                this.Scheme = category.Scheme;
            }
        }

        public SyndicationCategory ToSyndicationCategory()
        {
            return new SyndicationCategory(this.Name, this.Scheme, this.Label);
        }
    }
}
