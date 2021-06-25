//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System;
using System.Threading;
using System.Workflow.Runtime;

namespace Microsoft.Samples.Rules.RuleSetToolkitUsageSample
{
    class Program
    {
        static void Main(string[] args)
        {
            WorkflowRuntime workflowRuntime = new WorkflowRuntime();
            workflowRuntime.AddService(new Microsoft.Samples.Rules.ExternalRuleSetService.ExternalRuleSetService());
            
            AutoResetEvent waitHandle = new AutoResetEvent(false);
            workflowRuntime.WorkflowCompleted += delegate(object sender, WorkflowCompletedEventArgs e) {waitHandle.Set();};
            workflowRuntime.WorkflowTerminated += delegate(object sender, WorkflowTerminatedEventArgs e)
            {
                Console.WriteLine(e.Exception.Message);
                waitHandle.Set();
            };

            WorkflowInstance instance = workflowRuntime.CreateWorkflow(typeof(Workflow1));
            instance.Start();

            waitHandle.WaitOne();
        }
    }
}

