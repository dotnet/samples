//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------
using System.Data.Linq.Mapping;
using System.Runtime.Serialization;

namespace Microsoft.Samples.Organization
{
    [Table(Name="Departments")]
    [DataContract]
    public class Department
    {
        [Column]
        [DataMember]
        public string Id { get; set; }

        [Column]
        [DataMember]
        public string Name { get; set; }

        [Column(Name = "Owner", DbType = "Int")]
        [DataMember]
        public string OwnerId { get; set; }

        [DataMember]
        public Employee Owner { get; set; }

        public override string ToString()
        {
            return string.Format("{0} {1} (owner: {2})", this.Id, this.Name, this.OwnerId);
        }
    }
}
