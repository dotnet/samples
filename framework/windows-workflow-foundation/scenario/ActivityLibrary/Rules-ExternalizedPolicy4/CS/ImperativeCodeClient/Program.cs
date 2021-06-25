//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

using System;
using System.Activities;
using System.Activities.Statements;
using Microsoft.Samples.Activities.Rules;
using System.Activities.Expressions;

namespace Microsoft.Samples.Activities.Rules.Client
{

    //--------------------------------------
    // Apply discount to orders using a ruleset.
    //
    // Rules:
    // ======
    //     If Value > 500 and CustomerType is Residental => Discount = 5%
    //     If Value > 10000 and CustomerType is Business => Discount = 10%
    //--------------------------------------
    class Program
    {
        static void Main(string[] args)
        {            
            // create orders 
            Variable<Order> order1 = new Variable<Order>() { Name = "Order1", Default =  new LambdaValue<Order>(c => new Order(650, CustomerType.Residential)) };
            Variable<Order> resultOrder1 = new Variable<Order>() { Name = "ResultOrder1" };

            Variable<Order> order2 = new Variable<Order>() { Name = "Order2", Default = new LambdaValue<Order>(c => new Order(15000, CustomerType.Business)) };
            Variable<Order> resultOrder2 = new Variable<Order>() { Name = "ResultOrder2" };

            Variable<Order> order3 = new Variable<Order>() { Name = "Order3", Default = new LambdaValue<Order>(c => new Order(650, CustomerType.Business)) };
            Variable<Order> resultOrder3 = new Variable<Order>() { Name = "ResultOrder3" };

            // create and run workflow instance
            WorkflowInvoker.Invoke(
                new Sequence
                {
                    Variables = { order1, order2, order3, resultOrder1, resultOrder2, resultOrder3 },
                    Activities =
                    {
                        //---------------------------------------
                        // Rule: Order > 500 and CustomerType is Residential
                        //---------------------------------------
                        new WriteLine { Text = new InArgument<string>("OrderValue > 500 and is Residential customer => discount = 5%") },
                        new WriteLine
                        {
                            Text = new InArgument<string>(c => string.Format("   Before Evaluation: {0}", order1.Get(c).ToString()))
                        },
                        new ExternalizedPolicy4<Order>
                        {
                            RulesFilePath = @"..\..\ApplyDiscount.rules", 
                            RuleSetName = "DiscountRuleSet",
                            TargetObject = new InArgument<Order>(order1),
                            ResultObject = new OutArgument<Order>(resultOrder1)
                        },
                        new WriteLine
                        {
                            Text = new InArgument<string>(c => string.Format("   After Evaluation: {0}", resultOrder1.Get(c).ToString()))
                        },

                        //---------------------------------------
                        // Rule: Order > 10000 and CustomerType is Businesss
                        //---------------------------------------
                        new WriteLine(),
                        new WriteLine { Text = new InArgument<string>("OrderValue > 10000 and is Business customer => discount = 10%") },
                        new WriteLine
                        {
                            Text = new InArgument<string>(c => string.Format("   Before Evaluation: {0}", order2.Get(c).ToString()))
                        },
                        new ExternalizedPolicy4<Order>
                        {
                            RulesFilePath = @"..\..\ApplyDiscount.rules", 
                            RuleSetName = "DiscountRuleSet",
                            TargetObject = new InArgument<Order>(order2),
                            ResultObject = new OutArgument<Order>(resultOrder2)
                        },
                        new WriteLine
                        {
                            Text = new InArgument<string>(c => string.Format("   After Evaluation: {0}", resultOrder2.Get(c).ToString()))
                        },

                        //---------------------------------------
                        // No Rules Applied
                        //---------------------------------------
                        new WriteLine(),
                        new WriteLine { Text = new InArgument<string>("This order does not match any of the rules above") },
                        new WriteLine
                        {
                            Text = new InArgument<string>(c => string.Format("   Before Evaluation: {0}", order3.Get(c).ToString()))
                        },
                        new ExternalizedPolicy4<Order>
                        {
                            RulesFilePath = @"..\..\ApplyDiscount.rules", 
                            RuleSetName = "DiscountRuleSet",
                            TargetObject = new InArgument<Order>(order3),
                            ResultObject = new OutArgument<Order>(resultOrder3)
                        },
                        new WriteLine
                        {
                            Text = new InArgument<string>(c => string.Format("   After Evaluation: {0}", resultOrder3.Get(c).ToString()))
                        }

                    }
                }
            );

            // wait until the user press a key
            Console.WriteLine("Press any key to exit...");
            Console.Read();
        }  
    }
}
