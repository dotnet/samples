//-------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved
//-------------------------------------------------------------------

using System.Activities;

namespace Microsoft.Samples.UsingWorkflowItemPresenter
{
    public sealed class SimpleNativeActivity : NativeActivity
    {
        // this property contains an activity that will be scheduled in the execute method
		// the WorkflowItemPresenter in the designer is bound to this to enable editing
		// of the value
        public Activity Body { get; set; }

        protected override void CacheMetadata(NativeActivityMetadata metadata)
        {
           metadata.AddChild(Body);
           base.CacheMetadata(metadata);
            
        }

        protected override void Execute(NativeActivityContext context)
        {
            context.ScheduleActivity(Body);
        }
    }
}
