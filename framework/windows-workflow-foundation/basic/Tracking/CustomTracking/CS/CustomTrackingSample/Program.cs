//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System;
using System.Activities;
using System.Activities.Statements;
using System.Activities.Tracking;
using System.Threading;

namespace Microsoft.Samples.CustomTrackingSample
{


    class Program
    {
        // Custom workflow activity demonstrates usage of CustomTrackingRecord
        // The custom activity creates a CustomTrackingRecord and emits the record
        // A custom tracking participant can subscribe for CustomTrackingRecord objects
        sealed class CustomActivity : CodeActivity
        {

            protected override void Execute(CodeActivityContext context)
            {

                Console.WriteLine("In CustomActivity.Execute");
                CustomTrackingRecord customRecord = new CustomTrackingRecord("OrderIn")
                {
                    Data = 
                            {
                                {"OrderId", 200},
                                {"OrderDate", "20 Aug 2001"}
                            }
                };

                // Emit CustomTrackingRecord
                context.Track(customRecord);
            }
        }

        static Activity BuildSampleWorkflow()
        {
            return new Sequence()
            {
                Activities =
                    {
                        new WriteLine() { Text = "Begin Workflow" },
                        new CustomActivity(),
                        new WriteLine() { Text = "End Workflow" },
                    }
            };
        }

        static void Main(string[] args)
        {
            const String all = "*";
            AutoResetEvent syncEvent = new AutoResetEvent(false);
            ConsoleTrackingParticipant customTrackingParticipant = new ConsoleTrackingParticipant()
            {
                // Create a tracking profile to subscribe for tracking records
                // In this sample the profile subscribes for CustomTrackingRecords,
                // workflow instance records and activity state records
                TrackingProfile = new TrackingProfile()
                {
                    Name = "CustomTrackingProfile",
                    Queries = 
                    {
                        new CustomTrackingQuery() 
                        {
                         Name = all,
                         ActivityName = all
                        },
                        new WorkflowInstanceQuery()
                        {
                            // Limit workflow instance tracking records for started and completed workflow states
                            States = { WorkflowInstanceStates.Started, WorkflowInstanceStates.Completed },
                        },
                        new ActivityStateQuery()
                        {
                            // Subscribe for track records from all activities for all states
                            ActivityName = all,
                            States = { all },

                            // Extract workflow variables and arguments as a part of the activity tracking record
                            // VariableName = "*" allows for extraction of all variables in the scope
                            // of the activity
                            Variables = 
                            {                                
                                { all }   
                            }
                        }   
                    }
                }
            };


            WorkflowInvoker invoker = new WorkflowInvoker(BuildSampleWorkflow());
            invoker.Extensions.Add(customTrackingParticipant);

            invoker.Invoke();
        }
    }
}
