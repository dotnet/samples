//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System;
using System.Workflow.Activities;

namespace Microsoft.Samples.Rules.RuleSetToolkitUsageSample
{
	public sealed partial class Workflow1: SequentialWorkflowActivity
	{
		public Workflow1()
		{
			InitializeComponent();
		}
        private double orderValue = 12000;
        private double discount = 0;

        private void WorkflowCompleted(object sender, EventArgs e)
        {
            Console.WriteLine("Workflow completed");
            Console.WriteLine("OrderValue = {0}", orderValue);
            Console.WriteLine("Discount = {0}", discount);
        }
	}
}

