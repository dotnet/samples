//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------
using System;
using System.Runtime.Serialization;

namespace Microsoft.Samples.ContosoHR
{
    // contains the information of a hiring request
    [DataContract]
    public class HiringRequestInfo
    {
        [DataMember]
        public Guid Id { get; set; }

        [DataMember]
        public string RequesterId { get; set; }

        [DataMember]
        public DateTime CreationDate { get; set; }

        [DataMember]
        public string PositionId { get; set; }

        [DataMember]
        public string DepartmentId { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public string Title { get; set; }

        [DataMember]
        public Guid WorkflowInstanceId { get; set; }
        
        public bool IsCompleted { get; set; }

        public bool IsSuccess { get; set; }

        public bool IsCancelled { get; set; }
    }
}
