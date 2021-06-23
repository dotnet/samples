//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

using System;
using System.Activities;

namespace Microsoft.Samples.Activities.Statements
{
    class Program
    {
        static void Main(string[] args)
        {
            WorkflowInvoker.Invoke(new Workflow1());
                        
            Console.WriteLine("");
            Console.WriteLine("Press enter to exit...");
            Console.ReadLine();    
        }
    }
}
