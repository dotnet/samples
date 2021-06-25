//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------
using System.Data.Linq.Mapping;
using System.Runtime.Serialization;

namespace Microsoft.Samples.Organization
{
    [Table(Name="Employees")]
    [DataContract]
    public class Employee
    {
        [Column(DbType="Int")]
        [DataMember]
        public string Id { get; set; }

        [Column]
        [DataMember]
        public string Name { get; set; }

        [Column(Name="Department")]
        internal string DepartmentId { get; set; }
        
        [DataMember]
        public Department Department { get; set; }
        
        [Column(Name = "Position")]
        internal string PositionId { get; set; }

        [DataMember]
        public Position Position { get; set; }

        [Column(DbType = "Int")]        
        [DataMember]
        public string ManagerId { get; set; }

        public override string ToString()
        {
            return string.Format("{0} {1} {2} {3}", this.Id, this.Name, this.Position.ToString(), this.Department.ToString());
        }
    }
}