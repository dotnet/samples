//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

using System;
using System.Activities;

namespace Microsoft.Samples.Activities.Statements
{

    /// <summary>
    /// This activity returns the instance id of the workflow it is executing in
    /// 
    /// We choose CodeActivity as the base type because we need access to the CodeActivityContext for getting access to the workflow instance ID.
    /// 
    /// This activity returns a System.Guid for the instance ID
    /// </summary>
    /// 
    public sealed class GetWorkflowInstanceId : CodeActivity<Guid>
    {
        protected override Guid Execute(CodeActivityContext context)
        {
            return context.WorkflowInstanceId;
        }
    }
}
