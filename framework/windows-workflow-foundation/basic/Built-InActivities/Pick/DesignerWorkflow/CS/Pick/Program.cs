//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System;
using System.Activities;
using System.Threading;

namespace Microsoft.Samples.PickUsage
{
    public class PickSample
    {
        static string bookmarkName = "UserName";
        static WorkflowApplication application;

        public static void Main(string[] args)
        {
            application = new WorkflowApplication(new Sequence1());

            // Notify when the workflow completes.
            ManualResetEvent completedEvent = new ManualResetEvent(false);
            application.Completed += delegate(WorkflowApplicationCompletedEventArgs e)
            {
                completedEvent.Set();
            };

            // Run the workflow.
            application.Run();

            // Get user input from the console and send it to the workflow instance.
            // This is done in a separate thread in order to not block current execution.
            ThreadPool.QueueUserWorkItem(ReadName);

            // Wait until the workflow completes.
            completedEvent.WaitOne();

            Console.WriteLine("Workflow completed.  Waiting 10 seconds before exiting...");
            Thread.Sleep(10000);
        }

        static void ReadName(object state)
        {
            string text = Console.ReadLine();

            // Resume the Activity that set this bookmark (ReadString).
            application.ResumeBookmark(bookmarkName, text);
        }
    }
}
