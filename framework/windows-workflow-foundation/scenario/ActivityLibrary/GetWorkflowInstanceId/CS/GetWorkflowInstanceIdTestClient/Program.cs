//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

using System;
using System.Threading;
using System.Activities;

namespace Microsoft.Samples.Activities.Statements
{

    class Program
    {
        static void Main(string[] args)
        {
            AutoResetEvent syncEvent = new AutoResetEvent(false);
            
            // create the workflow app and add handlers for the Completed action
            WorkflowApplication app = new WorkflowApplication(new Sequence1());
            app.Completed = delegate(WorkflowApplicationCompletedEventArgs e)
            {
                syncEvent.Set();
            };

            // start the application
            app.Run();
            syncEvent.WaitOne();

            Console.WriteLine("Press [ENTER] to exit...");
            Console.ReadLine();
        }
        
    } 
}
