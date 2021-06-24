//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System;
using System.Activities;
using System.Activities.Statements;
using System.ServiceModel.Activities;
using System.ServiceModel.Description;
using System.Xml.Linq;

namespace Microsoft.Samples.SuspendedInstanceManagement
{
    public class PurchaseOrderWorkflow
    {
        public static ContractDescription poContractDescription = ContractDescription.GetContract(typeof(IPurchaseOrderService));

        public static Activity CreateBody()
        {
            Variable<PurchaseOrder> purchaseOrder = new Variable<PurchaseOrder> { Name = "po" };

            Sequence sequence = new Sequence
            {
                Variables = 
                {
                    purchaseOrder
                },

                Activities =
                {
                    new Receive
                    {
                        OperationName = "SubmitPurchaseOrder",
                        ServiceContractName = XName.Get(poContractDescription.Name),
                        CanCreateInstance = true,
                        Content = new ReceiveParametersContent
                        {
                            Parameters = 
                            {
                                {"po", new OutArgument<PurchaseOrder>(purchaseOrder)}
                            }
                        }
                    },

                    new WriteLine
                    {
                       Text = new InArgument<string> (e=>"Order is received\nPO number = " + purchaseOrder.Get(e).PONumber
                           + " Customer Id = " + purchaseOrder.Get(e).CustomerId)
                    },

                    new Persist(),

                    new ExceptionThrownActivity(),

                    new WriteLine
                    {
                        Text = new InArgument<string>("Order processing is complete")
                    }
                }
            };

            return sequence;
        }
    }

    class ExceptionThrownActivity : NativeActivity
    {
        static int executionCount = 0;

        protected override void Execute(NativeActivityContext context)
        {
            if (executionCount++ == 0)
            {
                throw new InvalidOperationException();
            }
        }

        protected override bool CanInduceIdle
        {
            get
            {
                return false;
            }
        }
    }
}