//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------
using System;

namespace Microsoft.Samples.ContosoHR
{
    // Represents an action that occurred in a HiringRequestInfo    
    public class RequestHistoryRecord
    {
        public string RequestId { get; set; }

        public long RecordNumber { get; set; }

        public string SourceState { get; set; }

        public string Action { get; set; }

        public string Comment { get; set; }

        public string EmployeeId { get; set; }

        public string EmployeeName { get; set; }

        public DateTime Date { get; set; }
    }
}
