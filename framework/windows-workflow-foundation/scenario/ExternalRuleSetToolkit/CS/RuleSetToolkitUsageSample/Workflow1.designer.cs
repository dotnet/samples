//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using Microsoft.Samples.Rules.PolicyActivities;

namespace Microsoft.Samples.Rules.RuleSetToolkitUsageSample
{
	public sealed partial class Workflow1
	{
		#region Designer generated code
		
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.CanModifyActivities = true;
            this.policyFromService1 = new PolicyFromService();
            // 
            // policyFromService1
            // 
            this.policyFromService1.MajorVersion = 0;
            this.policyFromService1.MinorVersion = 0;
            this.policyFromService1.Name = "policyFromService1";
            this.policyFromService1.RuleSetName = "DiscountRuleSet";
            // 
            // Workflow1
            // 
            this.Activities.Add(this.policyFromService1);
            this.Name = "Workflow1";
            this.Completed += new System.EventHandler(this.WorkflowCompleted);
            this.CanModifyActivities = false;

		}

		#endregion

        private PolicyFromService policyFromService1;
    }
}

