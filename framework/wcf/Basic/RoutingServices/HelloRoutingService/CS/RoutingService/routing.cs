
//  Copyright (c) Microsoft Corporation.  All Rights Reserved.

using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Routing;

namespace Microsoft.Samples.HelloRoutingService
{
    public class Router
    {

        // Host the service within this EXE console application.
        public static void Main()
        {
            // Create a ServiceHost for the CalculatorService type.
            using (ServiceHost serviceHost =
                new ServiceHost(typeof(RoutingService)))
            {
                //Rename or delete the provided App.config and  
                //uncomment this method call to run a code-based Routing Service
                //ConfigureRouterViaCode(serviceHost);
                
                // Open the ServiceHost to create listeners         
                // and start listening for messages.
                Console.WriteLine("The Routing Service configured, opening....");
                serviceHost.Open();
                Console.WriteLine("The Routing Service is now running.");
                Console.WriteLine("Press <ENTER> to terminate router.");
                
                // The service can now be accessed.
                Console.ReadLine();
            }
        }

        private static void ConfigureRouterViaCode(ServiceHost serviceHost)
        {
            //This code sets up the Routing Sample via code.  Rename or delete the provided
            //App.config and uncomment this method call to run a code-based Routing Service

            //set up some communication defaults
            string clientAddress = "http://localhost:8000/servicemodelsamples/service";
            string routerAddress = "http://localhost:8000/routingservice/router";

            Binding routerBinding = new WSHttpBinding();
            Binding clientBinding = new WSHttpBinding();


            //add the endpoint the router will use to recieve messages
            serviceHost.AddServiceEndpoint(typeof(IRequestReplyRouter), routerBinding, routerAddress);


            //create the client endpoint the router will route messages to
            ContractDescription contract = ContractDescription.GetContract(typeof(IRequestReplyRouter));
            ServiceEndpoint client = new ServiceEndpoint(contract, clientBinding, new EndpointAddress(clientAddress));

            //create a new routing configuration object
            RoutingConfiguration rc = new RoutingConfiguration();

            //create the endpoint list that contains the service endpoints we want to route to
            //in this case we have only one
            List<ServiceEndpoint> endpointList = new List<ServiceEndpoint>();
            endpointList.Add(client);

            //add a MatchAll filter to the Router's filter table
            //map it to the endpoint list defined earlier
            //when a message matches this filter, it will be sent to the endpoint contained in the list
            rc.FilterTable.Add(new MatchAllMessageFilter(), endpointList);

            //attach the behavior to the service host
            serviceHost.Description.Behaviors.Add(new RoutingBehavior(rc));
        }
    }
    

}
