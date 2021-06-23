//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------
using System;
using System.Activities;
using Microsoft.Samples.ContosoHR;

namespace Microsoft.Samples.ResumeRequest
{
    public class IncremementResumesCount: CodeActivity
    {
        [RequiredArgument]
        public InArgument<Guid> JobPostingId { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            JobPostingRepository.IncrementResumesCount(this.JobPostingId.Get(context));
        }
    }
}