//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System;
using System.Messaging;
using System.ServiceModel.Activities;

namespace Microsoft.Samples.SuspendedInstanceManagement
{
    class Program
    {
        static void Main(string[] args)
        {
            string queueName = @".\private$\ReceiveTx";
            if (!MessageQueue.Exists(queueName))
            {                
                MessageQueue.Create(queueName, true);
                Console.WriteLine("Message Queue {0} created", queueName);
                Console.WriteLine("Press <enter> to exit");
                Console.ReadLine();
                return;
            }

            WorkflowServiceHost host = WorkflowHost.CreateWorkflowServiceHost();
            host.Open();

            Console.WriteLine("The Workflow service is ready");
            Console.WriteLine("Press <ENTER> to close the service \n");

            WorkflowTestClient.Run(1);

            Console.ReadLine();

            host.Close();
        }
    }
}