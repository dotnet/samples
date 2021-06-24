//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved. 
//----------------------------------------------------------------

using System.Runtime.Serialization;
using System.ServiceModel.Syndication;

namespace Microsoft.Samples.JsonFeeds
{
    [DataContract(Namespace = "")]
    public class JsonSyndicationPerson
    {
        [DataMember]
        public readonly string Email;
        [DataMember]
        public readonly string Name;
        [DataMember]
        public readonly string Uri;

        public JsonSyndicationPerson(string email, string name, string uri)
        {
            this.Email = email;
            this.Name = name;
            this.Uri = uri;
        }
        public JsonSyndicationPerson(SyndicationPerson person)
        {
            if (person != null)
            {
                this.Email = person.Email;
                this.Name = person.Name;
                this.Uri = person.Uri;
            }
        }

        public SyndicationPerson ToSyndicationPerson()
        {
            return new SyndicationPerson(this.Email, this.Name, this.Uri);
        }
    }
}
