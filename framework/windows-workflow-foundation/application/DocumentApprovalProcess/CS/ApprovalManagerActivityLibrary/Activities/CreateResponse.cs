//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System.Activities;
using System.ComponentModel;
using Microsoft.Samples.DocumentApprovalProcess.ApprovalManagerActivityLibrary.Designers;
using Microsoft.Samples.DocumentApprovalProcess.ApprovalMessageContractLibrary;

namespace Microsoft.Samples.DocumentApprovalProcess.ApprovalManagerActivityLibrary
{
    // Generates a ApprovalResponse object given a particular ApprovalRequest object and if the request was approved
    [Designer(typeof(CreateResponseDesigner))]
    public sealed class CreateResponse : CodeActivity
    {
        [RequiredArgument]
        public InArgument<ApprovalRequest> Request { get; set; }
        [RequiredArgument]
        public InArgument<bool> Approved { get; set; }
        [RequiredArgument]
        public OutArgument<ApprovalResponse> Response { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            ApprovalResponse result = new ApprovalResponse(Request.Get(context), Approved.Get(context));
            Response.Set(context, result);
        }
    }
}
