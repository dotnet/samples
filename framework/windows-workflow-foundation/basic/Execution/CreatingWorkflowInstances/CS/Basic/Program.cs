//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System;
using System.Activities;
using System.Activities.Statements;
using System.Threading;

namespace Microsoft.Samples.WF.WorkflowInstances
{
    class Program
    {
        static Activity BuildTestWorkflow()
        {
            return new Sequence()
            {
                Activities =
                {
                    new WriteLine() { Text = "one" },
                    new WriteLine() { Text = "two" },
                    new WriteLine() { Text = "buckle my shoe" },
                }
            };
        }

        static void Main()
        {
            // This is how you run a workflow instance synchronously
            Activity activity = BuildTestWorkflow();
            WorkflowInvoker.Invoke(activity);

            // This is how you run a workflow instance asynchronously,
            // and can receive an event when it completes
            ManualResetEvent resetEvent = new ManualResetEvent(false);
            WorkflowApplication instance = new WorkflowApplication(BuildTestWorkflow());

            instance.Completed = delegate(WorkflowApplicationCompletedEventArgs e)
            {
                Console.WriteLine("workflow instance completed, Id = " + instance.Id);
                resetEvent.Set();
            };

            instance.Run();
            resetEvent.WaitOne();

            Console.ReadLine();
        }
    }
}
