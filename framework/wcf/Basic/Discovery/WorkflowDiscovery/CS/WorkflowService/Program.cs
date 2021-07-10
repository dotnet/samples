//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System;
using System.Activities;
using System.Activities.Statements;
using System.ServiceModel;
using System.ServiceModel.Activities;
using System.Xml.Linq;

namespace Microsoft.Samples.Discovery
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting up...");
            WorkflowService service = CreateService();

            WorkflowServiceHost host = new WorkflowServiceHost(service, new Uri("http://localhost:8000/DiscoveryPrintService"));
            try
            {
                // ServiceDiscoveryBehavior and UdpDiscoveryEndpoint are being added through config
                Console.WriteLine("Opening service...");
                host.Open();

                Console.WriteLine("To terminate press ENTER");
                Console.ReadLine();

                host.Close();
            }
            catch (CommunicationException e)
            {
                Console.WriteLine(e.Message);
            }
            catch (TimeoutException e)
            {
                Console.WriteLine(e.Message);
            }
            
            if (host.State != CommunicationState.Closed)
            {
                Console.WriteLine("Aborting service...");
                host.Abort();
            }
        }

        private static WorkflowService CreateService()
        {
            Variable<string> message = new Variable<string> { Name = "message" };
            Receive receiveString = new Receive
            {
                OperationName = "Print",
                ServiceContractName = XName.Get("IPrintService", "http://tempuri.org/"),
                Content = new ReceiveParametersContent
                {
                    Parameters = 
                    {
                        {"message", new OutArgument<string>(message)}
                    }
                },
                CanCreateInstance = true
            };
            Sequence workflow = new Sequence()
            {
                Variables = { message },
                Activities =
                {    
                    receiveString,    
                    new WriteLine                        
                    {    
                        Text = new InArgument<string>(env =>("Message received from Client: " + message.Get(env)))   
                    },
                },
            };
            return new WorkflowService
            {
                Name = "PrintService",
                Body = workflow
            };
        }
    }
}
