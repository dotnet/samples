//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------
using System;
using System.Activities;
using Microsoft.Samples.ContosoHR;

namespace Microsoft.Samples.HiringService
{
    public class CreateJobPosting : CodeActivity<JobPosting>
    {
        [RequiredArgument]
        public InArgument<HiringRequestInfo> HiringRequestInfo { get; set; }

        protected override JobPosting Execute(CodeActivityContext context)
        {
            HiringRequestInfo hiringRequestInfo = this.HiringRequestInfo.Get(context);
            
            return new JobPosting
            {
                Id = Guid.NewGuid(),
                Title = hiringRequestInfo.Title,
                Description = hiringRequestInfo.Description,
                HiringRequestInfo = new HiringRequestInfo
                {
                    Id = hiringRequestInfo.Id,
                    CreationDate = hiringRequestInfo.CreationDate,
                    Description = hiringRequestInfo.Description,
                    DepartmentId = hiringRequestInfo.DepartmentId,
                    Title = hiringRequestInfo.Title,
                    PositionId = hiringRequestInfo.PositionId,
                    RequesterId = hiringRequestInfo.RequesterId                      
                }
            };
        }
    }
}
