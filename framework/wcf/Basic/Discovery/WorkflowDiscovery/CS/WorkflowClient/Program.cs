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
        static Activity workflow;

        private static void CreateClientWorkflow()
        {
            Variable<string> message = new Variable<string> { Name = "message" };
            Variable<Uri> serviceUri = new Variable<Uri> { Name = "serviceUri" };

            workflow = new Sequence()
            {
                Variables =  {message, serviceUri},
                Activities = 
                {                    
                    new WriteLine
                    {
                        Text = new InArgument<string>("Searching for WF matching IPrintService Contract") 
                    },
                    new FindActivity
                    {
                        DiscoveredEndpointUri = new OutArgument<Uri>(serviceUri)
                    },
                    new WriteLine  
                    {                         
                        Text = new InArgument<string>(env =>("Found Service at: " + serviceUri.Get(env)))
                    },
                    new WriteLine  
                    {                         
                        Text = new InArgument<string>("Connecting to WF, sending text 'Hello from WF Client'")
                    },    
                    new Send    
                    {
                        ServiceContractName = XName.Get("IPrintService", "http://tempuri.org/"),
                        OperationName = "Print",
                        Content = new SendParametersContent    
                        {
                            Parameters =    
                            {
                                    { "message", new InArgument<string>("Hello from WF Client") }    
                            },    
                        },
                        Endpoint = new Endpoint
                        {
                            ServiceContractName = XName.Get("IPrintService", "http://tempuri.org/"),
                            Binding = new BasicHttpBinding()
                        },                      
                        EndpointAddress = new InArgument<Uri>(serviceUri),
                    },    
                },
            };
        }
        static void Main(string[] args)
        {
            CreateClientWorkflow();
            try
            {
                WorkflowInvoker.Invoke(workflow);
                Console.WriteLine("Workflow completed successfully.");
            }
            catch (Exception e)
            {
                Console.WriteLine("Workflow completed with {0}: {1}.", e.GetType().FullName, e.Message);
            }
            Console.WriteLine("To exit press ENTER.");
            Console.ReadLine();
        }

    }
}

