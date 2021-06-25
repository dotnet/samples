//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------
using System.Activities;
using Microsoft.Samples.ContosoHR;

namespace Microsoft.Samples.HiringService
{
    // Saves the information of a HiringRequestInfo. 
    public class SaveHiringRequestInfo: NativeActivity
    {
        [RequiredArgument]
        public InArgument<HiringRequestInfo> HiringRequestInfo { get; set; }

        public InArgument<bool> IsCompleted { get; set; }
        public InArgument<bool> IsSuccess { get; set; }
        public InArgument<bool> IsCancelled { get; set; }        

        protected override void Execute(NativeActivityContext context)
        {
            HiringRequestInfo hiringRequestInfo = this.HiringRequestInfo.Get(context);
            hiringRequestInfo.IsCompleted = this.IsCompleted.Get(context);
            hiringRequestInfo.IsCancelled = this.IsCancelled.Get(context);
            hiringRequestInfo.IsSuccess = this.IsSuccess.Get(context);
            HiringRequestRepository.Save(hiringRequestInfo);
        }
    }   
}
