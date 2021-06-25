//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System;
using System.ServiceModel.Activities;
using System.ServiceModel.Description;
using System.Xml.Linq;
using System.ServiceModel.Activities.Description;

namespace Sample
{
    class WorklfowHost
    {
        public static WorkflowServiceHost CreateWorkflowServiceHost()
        {
            WorkflowService service = new WorkflowService()
            {
                Body = PurchaseOrderWorkflow.CreateBody(),

                Endpoints =
                {
                    new System.ServiceModel.Endpoint
                    {
                        Binding = new System.ServiceModel.NetMsmqBinding("NetMsmqBindingTx"),
                        AddressUri = new Uri("net.msmq://localhost/private/ReceiveTx"),
                        ServiceContractName = XName.Get(PurchaseOrderWorkflow.poContractDescription.Name)
                    }
                }
            };

            WorkflowServiceHost workflowServiceHost = new WorkflowServiceHost(service);

            IServiceBehavior idleBehavior = new WorkflowIdleBehavior { TimeToUnload = TimeSpan.Zero };
            workflowServiceHost.Description.Behaviors.Add(idleBehavior);

            IServiceBehavior workflowUnhandledExceptionBehavior = new WorkflowUnhandledExceptionBehavior() 
            {
                Action = WorkflowUnhandledExceptionAction.AbandonAndSuspend
            };
            workflowServiceHost.Description.Behaviors.Add(workflowUnhandledExceptionBehavior);

            SqlWorkflowInstanceStoreBehavior instanceStoreBehavior = new SqlWorkflowInstanceStoreBehavior()
            {
                ConnectionString = "Server=localhost\\SQLEXPRESS;Integrated Security=true;Initial Catalog=DefaultSampleStore;"
            };
            workflowServiceHost.Description.Behaviors.Add(instanceStoreBehavior);

            ServiceEndpoint workflowControlEndpoint = new WorkflowControlEndpoint()
            {
                Binding = new System.ServiceModel.NetNamedPipeBinding(System.ServiceModel.NetNamedPipeSecurityMode.None),
                Address = new System.ServiceModel.EndpointAddress("net.pipe://workflowInstanceControl")
            };

            workflowServiceHost.AddServiceEndpoint(workflowControlEndpoint);
            workflowServiceHost.WorkflowExtensions.Add(new TrackingListenerConsole());

            foreach (ServiceEndpoint ep in workflowServiceHost.Description.Endpoints)
            {
                Console.WriteLine(ep.Address);
            }

            return workflowServiceHost;
        }
    }
}