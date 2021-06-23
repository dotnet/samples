//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------
using System.Activities;
using System.ComponentModel;
using Microsoft.Samples.ContosoHR;

namespace Microsoft.Samples.ResumeRequest
{
    public class SaveJobPosting : CodeActivity
    {
        [RequiredArgument]
        public InArgument<JobPosting> JobPosting  { get; set; }
                
        [DefaultValue(null)]
        public InArgument<string> State { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            if (this.State.Get(context) != null)
            {
                this.JobPosting.Get(context).Status = this.State.Get(context);
            }
            else
            {
                if (string.IsNullOrEmpty(this.JobPosting.Get(context).Status))
                {
                    this.JobPosting.Get(context).Status = "Waiting for Data";
                }
            }

            JobPostingRepository.Save(this.JobPosting.Get(context));            
        }
    }
}