//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System;
using System.ServiceModel.Activities;
using System.ServiceModel.Activities.Description;
using System.ServiceModel.Description;
using System.Xml.Linq;

namespace Microsoft.Samples.SuspendedInstanceManagement
{
    class WorkflowHost
    {
        public static WorkflowServiceHost CreateWorkflowServiceHost()
        {
            // add the workflow implementation and application endpoint to the host
            WorkflowService service = new WorkflowService()
            {
                Body = PurchaseOrderWorkflow.CreateBody(),

                Endpoints =
                {
                    // adds an application endpoint
                    new System.ServiceModel.Endpoint
                    {
                        Binding = new System.ServiceModel.NetMsmqBinding("NetMsmqBindingTx"),
                        AddressUri = new Uri("net.msmq://localhost/private/ReceiveTx"),
                        ServiceContractName = XName.Get(PurchaseOrderWorkflow.poContractDescription.Name)
                    }
                }
            };
            WorkflowServiceHost workflowServiceHost = new WorkflowServiceHost(service);


            // add the workflow management behaviors
            IServiceBehavior idleBehavior = new WorkflowIdleBehavior { TimeToUnload = TimeSpan.Zero };
            workflowServiceHost.Description.Behaviors.Add(idleBehavior);

            IServiceBehavior workflowUnhandledExceptionBehavior = new WorkflowUnhandledExceptionBehavior() 
            {
                Action = WorkflowUnhandledExceptionAction.AbandonAndSuspend // this is also the default
            };
            workflowServiceHost.Description.Behaviors.Add(workflowUnhandledExceptionBehavior);


            // add the instance store
            SqlWorkflowInstanceStoreBehavior instanceStoreBehavior = new SqlWorkflowInstanceStoreBehavior()
            {
                ConnectionString = "Server=localhost\\SQLEXPRESS;Integrated Security=true;Initial Catalog=DefaultSampleStore;"
            };
            workflowServiceHost.Description.Behaviors.Add(instanceStoreBehavior);


            // add a workflow management endpoint
            ServiceEndpoint workflowControlEndpoint = new WorkflowControlEndpoint()
            {
                Binding = new System.ServiceModel.NetNamedPipeBinding(System.ServiceModel.NetNamedPipeSecurityMode.None),
                Address = new System.ServiceModel.EndpointAddress("net.pipe://workflowInstanceControl")
            };
            workflowServiceHost.AddServiceEndpoint(workflowControlEndpoint);


            // add the tracking participant
            workflowServiceHost.WorkflowExtensions.Add(new TrackingListenerConsole());


            foreach (ServiceEndpoint ep in workflowServiceHost.Description.Endpoints)
            {
                Console.WriteLine(ep.Address);
            }

            return workflowServiceHost;
        }
    }
}