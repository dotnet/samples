//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------
using System;
using System.Activities.DurableInstancing;
using System.Configuration;
using System.ServiceModel;
using System.ServiceModel.Activities;
using System.ServiceModel.Activities.Description;

namespace Microsoft.Samples.HiringService
{
    class Program
    {
        // start the service
        static void Main(string[] args)
        {
            string persistenceConnectionString = ConfigurationManager.ConnectionStrings["WorkflowPersistence"].ConnectionString;
            string baseAddr = "http://localhost:8080/Contoso/HiringRequestService";

            using (WorkflowServiceHost host = new WorkflowServiceHost(new HiringRequestProcessServiceDefinition(), new Uri(baseAddr)))
            {
                SqlWorkflowInstanceStoreBehavior instanceStoreBehavior = new SqlWorkflowInstanceStoreBehavior(persistenceConnectionString);
                instanceStoreBehavior.InstanceCompletionAction = InstanceCompletionAction.DeleteAll;
                instanceStoreBehavior.InstanceEncodingOption = InstanceEncodingOption.GZip;

                host.Description.Behaviors.Add(instanceStoreBehavior);
                host.Description.Behaviors.Add(new WorkflowIdleBehavior() { TimeToPersist = new TimeSpan(0) });

                host.WorkflowExtensions.Add(new HiringRequestInfoPersistenceParticipant());

                // configure the unknown message handler
                host.UnknownMessageReceived += new EventHandler<System.ServiceModel.UnknownMessageReceivedEventArgs>(Program.UnknownMessageReceive);

                // add the control endpoint
                WorkflowControlEndpoint publicEndpoint = new WorkflowControlEndpoint(
                                                                new BasicHttpBinding(),
                                                                new EndpointAddress(new Uri("http://127.0.0.1/hiringProcess")));
                host.AddServiceEndpoint(publicEndpoint);
                host.AddDefaultEndpoints();
                
                // start the service
                Console.WriteLine("Starting ...");

                host.Open();

                // end when the user hits enter
                Console.WriteLine("Service is waiting at: " + baseAddr);
                Console.WriteLine("Press [Enter] to exit");
                Console.ReadLine();
                host.Close();
            }
        }

        static void UnknownMessageReceive(object sender, System.ServiceModel.UnknownMessageReceivedEventArgs e)
        {
            Console.WriteLine("Unknown message - " + e.ToString());
        }
    }    
}
