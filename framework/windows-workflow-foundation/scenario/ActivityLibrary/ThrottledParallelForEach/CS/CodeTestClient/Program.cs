//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

using System;
using System.Activities;
using System.Activities.Expressions;
using System.Activities.Statements;
using System.Collections.Generic;

namespace Microsoft.Samples.Activities.Statements
{
    // Code based test client for ThrottledParallelForEach
    // This sample iterates through a list of strings using a ThrottledParallelForEach
    // configured to have 3 concurrent instances and no CompletionCondition.
    //
    // The Body of the ThrottledParallelForEach contains a sequence of activities
    // that show some text to the console and wait for a random ammount of time (using a Delay)
    class Program
    {        
        static void Main(string[] args)
        {
            WorkflowInvoker.Invoke(CreateWF());

            Console.WriteLine("");
            Console.WriteLine("Press [ENTER] to exit...");
            Console.ReadLine();
        }

        static Activity CreateWF()
        {
            DelegateInArgument<string> current = new DelegateInArgument<string>();
            IList<string> data = new List<string>();            
            for (int i = 1; i < 11; i++)
            {
                data.Add(string.Format("Branch {0}", i));
            }
            Variable<int> waitTime = new Variable<int>();
            
            return              
                new Sequence
                {
                    Variables = { waitTime },
                    Activities = 
                    {
                        new ThrottledParallelForEach<string>
                        {                            
                            Values = new LambdaValue<IEnumerable<string>>(c => data),
                            MaxConcurrentBranches = 3,                            
                            Body = new ActivityAction<string>
                            {
                                Argument = current,
                                Handler = new Sequence
                                {
                                    Activities =
                                    {
                                        new WriteLine() { Text = new InArgument<string>(ctx => string.Format("Enter {0}", current.Get(ctx))) },
                                        new Assign<int> { To = waitTime, Value = new InArgument<int>(ctx =>  new Random().Next(0, 2500)) },
                                        new WriteLine() { Text = new InArgument<string>(ctx => string.Format("...{0} will wait for {1} millisenconds...", current.Get(ctx), waitTime.Get(ctx))) },
                                        new Delay { Duration = new InArgument<TimeSpan>(ctx => new TimeSpan(0,0,0,0, waitTime.Get(ctx))) },                                        
                                        new WriteLine() { Text = new InArgument<string>(ctx => string.Format("......Exit {0}", current.Get(ctx))) },
                                    }
                                }
                            }
                        }
                    }
                };
        }
    }
}