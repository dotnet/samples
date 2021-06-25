//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System;
using System.Activities.Tracking;

namespace Microsoft.Samples.SuspendedInstanceManagement
{
    class TrackingListenerConsole : TrackingParticipant
    {
        protected override void Track(TrackingRecord record, TimeSpan timeout)
        {
            WorkflowInstanceRecord workflowInstanceRecord = record as WorkflowInstanceRecord;

            if (workflowInstanceRecord != null)
            {
                string message = string.Format("Sequence: {0} Instance id: {1}, state = {2}", workflowInstanceRecord.RecordNumber, workflowInstanceRecord.InstanceId, workflowInstanceRecord.State);
                Console.WriteLine(message);
            }
        }
    }
}