//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------
using System.Activities;
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.Samples.DocumentApprovalProcess.ApprovalMessageContractLibrary;

namespace Microsoft.Samples.DocumentApprovalProcess.ApprovalManagerActivityLibrary
{
    [Designer(typeof(TallyResponses))]
    public sealed class TallyResponses : CodeActivity
    {
        [RequiredArgument]
        public InArgument<LinkedList<ApprovalResponse>> ResponseList { get; set; }
        [RequiredArgument]
        public OutArgument<int> Tally { get; set; }

        // Counts positive responses (approvals) in a list of responses
        //  This is used by the quorum activity
        protected override void Execute(CodeActivityContext context)
        {
            int count = 0;
            LinkedList<ApprovalResponse> responses = ResponseList.Get(context);
            foreach (ApprovalResponse response in responses)
            {
                if (response.Approved) count++;
            }
            Tally.Set(context, count);
        }
    }
}
