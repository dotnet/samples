//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------
using System;
using System.Runtime.Serialization;

namespace Microsoft.Samples.ContosoHR
{
    [DataContract]
    public class JobPostingResume
    {
        public JobPostingResume() 
        {
        }

        [DataMember]
        public JobPosting JobPosting { get; set; }

        [DataMember]
        public string CandidateMail { get; set; }

        [DataMember]
        public string CandidateName { get; set; }

        [DataMember]
        public string ResumeeText { get; set; }

        [DataMember]
        public DateTime ResumeeDate { get; set; }
    }
}