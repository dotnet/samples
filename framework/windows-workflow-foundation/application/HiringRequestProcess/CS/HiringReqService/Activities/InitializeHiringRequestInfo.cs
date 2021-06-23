//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------
using System.Activities;
using Microsoft.Samples.ContosoHR;

namespace Microsoft.Samples.HiringService
{
    public class InitializeHiringRequestInfo: CodeActivity
    {
        [RequiredArgument]
        public InArgument<HiringRequestInfo> HiringRequestInfo { get; set; }

        [RequiredArgument]
        public OutArgument<HiringRequestInfo> HiringRequestInfoOut { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            HiringRequestInfo hiringRequestInfo = this.HiringRequestInfo.Get(context);
            hiringRequestInfo.WorkflowInstanceId = context.WorkflowInstanceId;
            hiringRequestInfo.IsCompleted = false;
            hiringRequestInfo.IsCancelled = false;
            hiringRequestInfo.IsSuccess = false;
            this.HiringRequestInfoOut.Set(context, hiringRequestInfo);
        }
    }
}
