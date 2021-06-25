//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

using System;
using System.Activities;

namespace Microsoft.Samples.Activities.Rules.Client
{

    class Program
    {
        static void Main(string[] args)
        {
            WorkflowInvoker.Invoke(new Sequence1());

            Console.WriteLine("");
            Console.WriteLine("Press any key to exit...");
            Console.Read();
        }
    }
}
