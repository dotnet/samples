//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------
using System.Data.Linq.Mapping;
using System.Runtime.Serialization;

namespace Microsoft.Samples.Organization
{
    [Table(Name="PositionTypes")]
    [DataContract]
    public class PositionType
    {
        [Column]
        [DataMember]
        public string Id { get; set; }

        [Column]
        [DataMember]
        public string Name { get; set; }

        public override string ToString()
        {
            return string.Format("{0} {1}", this.Id, this.Name);
        }
    }
}