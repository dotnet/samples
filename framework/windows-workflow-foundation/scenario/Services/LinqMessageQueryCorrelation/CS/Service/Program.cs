//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System;
using System.Activities;
using System.Activities.Expressions;
using System.Activities.Statements;
using System.ServiceModel;
using System.ServiceModel.Activities;
using System.Xml.Linq;
using Microsoft.Samples.LinqMessageQueryCorrelation.SharedTypes;

namespace Microsoft.Samples.LinqMessageQueryCorrelation.Service
{    
    class Program
    {
        static void Main(string[] args)
        {
            using (WorkflowServiceHost host = new WorkflowServiceHost(GetServiceWorkflow(), new Uri(Constants.ServiceAddress)))
            {
                host.AddServiceEndpoint(Constants.POContractName, Constants.Binding, Constants.ServiceAddress);

                host.Open();
                Console.WriteLine("Service waiting at: " + Constants.ServiceAddress);
                Console.WriteLine("Press [ENTER] to exit");
                Console.ReadLine();
                host.Close();
            }

        }

        static Activity GetServiceWorkflow()
        {
            Variable<PurchaseOrder> po = new Variable<PurchaseOrder>();
            Variable<OrderStatus> orderStatus = new Variable<OrderStatus>();
            Variable<CorrelationHandle> poidHandle = new Variable<CorrelationHandle>();
            Variable<bool> complete = new Variable<bool>() { Default = false };

            Receive submitPO = new Receive
            {
                CanCreateInstance = true,
                ServiceContractName = Constants.POContractName,
                OperationName = Constants.SubmitPOName,
                Content = ReceiveContent.Create(new OutArgument<PurchaseOrder>(po))  // creates a ReceiveMessageContent
            };

            return new Sequence
            {
                Variables = { po, orderStatus, poidHandle, complete },
                Activities =
                {
                    submitPO,
                    new WriteLine { Text = "Received Purchase Order" },
                    new Assign<int>
                    {
                        To = new OutArgument<int>( (e) => po.Get(e).Id ),
                        Value = new InArgument<int>( (e) => new Random().Next() )
                    },                    
                    new SendReply
                    {
                        Request = submitPO,
                        Content = SendContent.Create(new InArgument<int>( (e) => po.Get(e).Id)), // creates a SendMessageContent
                        CorrelationInitializers =
                        {
                            new QueryCorrelationInitializer
                            {
                                // initializes a correlation based on the PurchaseOrder Id sent in the reply message and stores it in the handle
                                CorrelationHandle = poidHandle,
                                MessageQuerySet = new MessageQuerySet
                                {
                                    // Here we use our custom LinqMessageQuery for correlatoin
                                    // int is the name of the parameter being sent in the outgoing response
                                    { "PoId", new LinqMessageQuery(XName.Get("int", Constants.SerializationNamespace))}
                                }
                            }
                        }                        
                    }, 
                    new Assign<OrderStatus>
                    {
                        To = orderStatus,
                        Value = new InArgument<OrderStatus>( (e) => new OrderStatus() { Confirmed = false } )
                    },
                    new While   // Continue the workflow until the PurchaseOrder is confirmed
                    {
                        Condition = ExpressionServices.Convert<bool>(env => orderStatus.Get(env).Confirmed),
                        Body = new Receive
                        {
                            ServiceContractName = Constants.POContractName,
                            OperationName = Constants.ConfirmPurchaseOrder,                                    
                            CorrelatesWith = poidHandle, // identifies that the ConfirmPurchaseOrder operation is waiting on the PurchaseOrderId that was used to initialize this handle
                            CorrelatesOn = new MessageQuerySet // the query that is used on an incoming message to find the requisite PurchaseOrderId specified in the correlation
                            {
                                // Id is the name of the incoming parameter within the OrderStatus
                                { "PoId", new LinqMessageQuery(XName.Get("Id", Constants.DefaultNamespace))}
                            },
                            Content = ReceiveContent.Create(new OutArgument<OrderStatus>(orderStatus)) // creates a ReceiveMessageContent
                        }
                    },
                    new WriteLine { Text = "Purchase Order confirmed." },
                    new WriteLine { Text = new InArgument<string>( (e) => string.Format("Workflow completed for PurchaseOrder {0}: {1} {2}s", po.Get(e).Id, po.Get(e).Quantity, po.Get(e).PartName) ) },
                }
            };
        }
    }

}
