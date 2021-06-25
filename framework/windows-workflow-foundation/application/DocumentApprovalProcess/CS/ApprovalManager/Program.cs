//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System;
using System.Activities;
using System.Configuration;
using System.ServiceModel;
using System.ServiceModel.Activities;
using System.ServiceModel.Activities.Description;

using Microsoft.Samples.DocumentApprovalProcess.ApprovalManagerActivityLibrary;

namespace Microsoft.Samples.DocumentApprovalProcess.ApprovalManager
{
    class Program
    {
        static string ApprovalProcessDBConnectionString = ConfigurationManager.ConnectionStrings["ApprovalProcessDB"].ConnectionString;

        static void Main(string[] args)
        {
            Activity element = new ApprovalRouteAndExecute();

            WorkflowService shservice = new WorkflowService
            {
                Name = "ApprovalManager",
                ConfigurationName = "Microsoft.Samples.DocumentApprovalProcess.ApprovalManager.ApprovalManager",
                Body = element
            };

            // Cleanup old table of users from previous run
            UserManager.DeleteAllUsers();

            ServiceHost sh = new ServiceHost(typeof(Microsoft.Samples.DocumentApprovalProcess.ApprovalManager.SubscriptionManager), new Uri("http://localhost:8732/Design_Time_Addresses/service/SubscriptionManager/"));
            sh.Open();

            System.ServiceModel.Activities.WorkflowServiceHost wsh = new System.ServiceModel.Activities.WorkflowServiceHost(shservice, new Uri("http://localhost:8732/Design_TimeAddress/service/ApprovalManager"));

            // Setup persistence
            wsh.Description.Behaviors.Add(new SqlWorkflowInstanceStoreBehavior(ApprovalProcessDBConnectionString));
            WorkflowIdleBehavior wib = new WorkflowIdleBehavior();
            wib.TimeToUnload = new TimeSpan(0, 0, 2);
            wsh.Description.Behaviors.Add(wib);

            wsh.Open();

            Console.WriteLine("All services ready, press any key to close the services and exit.");

            Console.ReadLine();
            wsh.Close();
            sh.Close();
        }
    }


}
