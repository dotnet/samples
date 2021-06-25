//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------
using System;
using System.Activities;
using Microsoft.Samples.ContosoHR;

namespace Microsoft.Samples.ResumeRequest
{
    public class PostResume: CodeActivity
    {
        [RequiredArgument]
        public InArgument<JobPostingResume> Resume { get; set; }

        protected override void Execute(CodeActivityContext context)
        {            
            JobPostingRepository.InsertResume(this.Resume.Get(context));
        }
    }

}