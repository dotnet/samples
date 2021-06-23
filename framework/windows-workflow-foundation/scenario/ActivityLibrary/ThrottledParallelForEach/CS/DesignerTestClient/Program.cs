//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

using System;
using System.Activities;

namespace Microsoft.Samples.Activities.Statements
{    
    // This sample iterates through a list of strings using a ThrottledParallelForEach
    // configured to have 3 concurrent instances and no CompletionCondition.
    //
    // The Body of the ThrottledParallelForEach contains a sequence of activities
    // that show some text to the console and wait for a random ammount of time (using a Delay)    
    class Program
    {
        static void Main(string[] args)
        {
            WorkflowInvoker.Invoke(new Workflow1());

            Console.WriteLine("");
            Console.WriteLine("Press [ENTER] to exit...");
            Console.ReadLine();
        }
    }
}
