//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------
using System;
using System.Runtime.Serialization;

namespace Microsoft.Samples.ContosoHR
{    
    [DataContract]
    public class JobPosting
    {
        [DataMember]
        public Guid Id { get; set; }
        
        [DataMember]
        public HiringRequestInfo HiringRequestInfo { get; set; }

        [DataMember]
        public string Title { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public DateTime CreationDate { get; set; }

        [DataMember]
        public string Status { get; set; }

        public static JobPosting Create(HiringRequestInfo hiringRequestInfo)
        {
            return new JobPosting { Id = Guid.NewGuid(), HiringRequestInfo = hiringRequestInfo };
        }
    }
}