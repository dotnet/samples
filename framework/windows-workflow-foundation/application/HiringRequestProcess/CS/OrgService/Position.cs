//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------
using System.Data.Linq.Mapping;
using System.Runtime.Serialization;

namespace Microsoft.Samples.Organization
{
    [Table(Name="Positions")]
    [DataContract]
    public class Position
    {
        [Column]
        [DataMember]
        public string Id { get; set; }

        [Column]
        [DataMember]
        public string Name { get; set; }

        [Column(Name="PositionType")]
        internal string PositionTypeId { get; set; }

        [DataMember]
        public PositionType PositionType { get; set; }

        public override string ToString()
        {
            return string.Format("{0} {1} ({2})", this.Id, this.Name, this.PositionType.ToString());
        }
    }
}