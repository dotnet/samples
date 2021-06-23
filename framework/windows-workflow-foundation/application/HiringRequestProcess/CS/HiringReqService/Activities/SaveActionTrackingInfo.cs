//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------
using System;
using System.Activities;
using System.Activities.Tracking;
using HiringRequestService.Contoso.OrgService;

namespace Microsoft.Samples.HiringService
{    
    // Emits a custom tracking record containing information about the last operation in the process
    public sealed class SaveActionTrackingInfo : CodeActivity
    {
        [RequiredArgument]
        public InArgument<string> HiringRequestId   { get; set; }
        public InArgument<string> State             { get; set; }
        public InArgument<string> Comment           { get; set; }
        public InArgument<string> Action            { get; set; }
        public InArgument<Employee> Employee        { get; set; }        
        
        protected override void Execute(CodeActivityContext context)
        {            
            // create and set the record data
            CustomTrackingRecord customRecord = new CustomTrackingRecord("ActionExecuted");
            customRecord.Data.Add("HiringRequestId", this.HiringRequestId.Get(context));
            customRecord.Data.Add("State", this.State.Get(context));
            customRecord.Data.Add("Date", DateTime.Now);
            customRecord.Data.Add("Action", this.Action.Get(context));
            customRecord.Data.Add("Comment", this.Comment.Get(context));            
            if (this.Employee.Get(context) != null)
            {
                customRecord.Data.Add("EmployeeId", this.Employee.Get(context).Id);
                customRecord.Data.Add("EmployeeName", this.Employee.Get(context).Name);
            }

            // emit the record
            context.Track(customRecord);
        }
    }
}