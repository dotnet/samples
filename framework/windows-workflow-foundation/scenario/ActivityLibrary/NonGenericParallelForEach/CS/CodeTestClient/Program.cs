//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------
using System;
using System.Activities;
using System.Activities.Statements;
using System.Collections;
using System.Threading;    

namespace Microsoft.Samples.Activities.Statements
{
    class Program
    {
        static void Main(string[] args)
        {
            // iteration variable for the ForEach
            var item = new DelegateInArgument<object>();

            // list of elements to iterate
            ArrayList list = new ArrayList();
            list.Add("Bill");
            list.Add("Steve");
            list.Add("Ray");            
            
            // iterate through the list and show the elements
            Activity act =
                    new ParallelForEach
                    {
                        Values = new InArgument<IEnumerable>(ctx => list),
                        Body = new ActivityAction<object>
                        {
                            Argument = item,
                            Handler = new InvokeMethod
                            {
                                TargetType = typeof(Program),
                                MethodName = "ShowThreadId",
                                RunAsynchronously = true,
                                Parameters =
                                {
                                    new InArgument<string>(c => item.Get(c).ToString())
                                }
                            }
                        }
                    };            
            WorkflowInvoker.Invoke(act);

            Console.WriteLine("");
            Console.WriteLine("Press enter to exit...");
            Console.ReadLine();            
        }

        public static void ShowThreadId(string text)
        {
            Console.WriteLine(string.Format("Showing '{0}' in thread: {1}", text, Thread.CurrentThread.ManagedThreadId.ToString()));
            Thread.Sleep(1000);
        }
    }    
}