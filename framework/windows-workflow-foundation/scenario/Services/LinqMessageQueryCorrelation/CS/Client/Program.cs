//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System;
using System.Activities;
using System.Activities.Statements;
using System.ServiceModel;
using System.ServiceModel.Activities;
using Microsoft.Samples.LinqMessageQueryCorrelation.SharedTypes;

namespace Microsoft.Samples.LinqMessageQueryCorrelation.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            WorkflowInvoker.Invoke(GetClientWorkflow());
            Console.WriteLine("Press [ENTER] to exit");
            Console.ReadLine();
        }

        static Activity GetClientWorkflow()
        {
            Variable<PurchaseOrder> po = new Variable<PurchaseOrder>();
            Variable<OrderStatus> orderStatus = new Variable<OrderStatus>();

            Endpoint clientEndpoint = new Endpoint
            {
                Binding = Constants.Binding,
                AddressUri = new Uri(Constants.ServiceAddress)
            };

            Send submitPO = new Send
            {
                Endpoint = clientEndpoint,
                ServiceContractName = Constants.POContractName,
                OperationName = Constants.SubmitPOName,
                Content = SendContent.Create(new InArgument<PurchaseOrder>(po))
            };

            return new Sequence
            {
                Variables = { po, orderStatus },
                Activities =
                {
                    new WriteLine { Text = "Sending order for 150 widgets." },
                    new Assign<PurchaseOrder> { To = po, Value = new InArgument<PurchaseOrder>( (e) => new PurchaseOrder() { PartName = "Widget", Quantity = 150 } ) },
                    new CorrelationScope
                    {
                        Body = new Sequence
                        {
                            Activities = 
                            {
                                submitPO,                                
                                new ReceiveReply
                                {
                                    Request = submitPO,
                                    Content = ReceiveContent.Create(new OutArgument<int>( (e) => po.Get(e).Id ))
                                }
                            }
                        }
                    },                    
                    new WriteLine { Text = new InArgument<string>( (e) => string.Format("Got PoId: {0}", po.Get(e).Id) ) },
                    new Assign<OrderStatus> { To = orderStatus, Value = new InArgument<OrderStatus>( (e) => new OrderStatus() { Id = po.Get(e).Id, Confirmed = true }) },
                    new Send
                    {
                        Endpoint = clientEndpoint,
                        ServiceContractName = Constants.POContractName,
                        OperationName = Constants.ConfirmPurchaseOrder,
                        Content = SendContent.Create(new InArgument<OrderStatus>(orderStatus))
                    },
                    new WriteLine { Text = "The order was confirmed." },                    
                    new WriteLine { Text = "Client completed." }
                }
            };
        }
    }
}
