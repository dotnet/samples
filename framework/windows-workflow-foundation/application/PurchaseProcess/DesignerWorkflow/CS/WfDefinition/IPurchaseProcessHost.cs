//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------
using System;
using System.Activities;

namespace Microsoft.Samples.WF.PurchaseProcess
{

    /// <summary>
    /// Interface of the host
    /// </summary>
    interface IPurchaseProcessHost
    {
        bool CanSubmitProposalToInstance(Guid instanceId, int vendorId);
        WorkflowApplication CreateAndRun(RequestForProposal rfp);
        bool IsInstanceWaitingForProposals(Guid instanceId);
        WorkflowApplication LoadInstance(Guid instanceId);
        void SubmitVendorProposal(Guid instanceId, int vendorId, double value);
    }
}
