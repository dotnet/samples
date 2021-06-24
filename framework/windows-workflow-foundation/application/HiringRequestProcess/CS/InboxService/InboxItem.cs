//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------
using System;
using System.Runtime.Serialization;

namespace Microsoft.Samples.Inbox
{
    // Item in the inbox
    [DataContract]
    public class InboxItem
    {
        [DataMember]
        public string RequestId { get; set; }

        [DataMember]
        public string StartedBy { get; set; }

        [DataMember]
        public string Title { get; set; }

        [DataMember]
        public string State { get; set; }

        [DataMember]
        public DateTime InboxEntryDate { get; set; }
    }
}
