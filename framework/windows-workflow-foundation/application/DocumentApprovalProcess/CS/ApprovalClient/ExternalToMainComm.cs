//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System;
using System.IO;

using Microsoft.Samples.DocumentApprovalProcess.ApprovalMessageContractLibrary;

namespace Microsoft.Samples.DocumentApprovalProcess.ApprovalClient
{
    // This class is used to communicate with the Main WPF window from external sources
    // that don't have a reference to the UI (ex, for WCF services and WorkflowServiceHost)
    public static class ExternalToMainComm
    {
        public static Main Context { get; set; }

        // Called if ApprovalRequestsService got new approval request
        public static void NewApprovalRequest(ApprovalRequest request)
        {
            if (Context != null)
                Context.AddApprovalItem(request);
        }

        // Called if ApprovalRequestsService was told to cancel an existing request
        public static void CancelApprovalRequest(ApprovalRequest request)
        {
            if (Context != null)
                Context.RemoveApprovalItem(request);
        }

        // Called if the ApprovalRequestsService gets a response to a approval request generated locally
        public static void ApprovalRequestResponse(ApprovalResponse response)
        {
            if (Context != null)
                Context.ProcessResponse(response);
        }

        // Returns a writer that writes to the WPF status window -- this is used for client side tracking
        public static TextWriter GetStatusWriter()
        {
            if (Context != null)
                return new WindowTextWriter(Context.GetStatusTextBox());
            else
                return null;
        }

        // Simpler but less flexable way to write to the WPF status window when compared 
        // to getting a writer using GetStatusWriter() -- this is used to output debug messages
        public static void WriteStatusLine(String status)
        {
            if (Context != null)
            {
                GetStatusWriter().WriteLine(status);
            }
        }
    }
}
