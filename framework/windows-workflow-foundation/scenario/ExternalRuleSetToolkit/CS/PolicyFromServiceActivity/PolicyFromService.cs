//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System;
using System.ComponentModel;
using System.Drawing;
using System.Workflow.Activities;
using System.Workflow.Activities.Rules;
using System.Workflow.ComponentModel;
using System.Workflow.ComponentModel.Compiler;
using System.Workflow.ComponentModel.Design;
using Microsoft.Samples.Rules.ExternalRuleSetLibrary;

namespace Microsoft.Samples.Rules.PolicyActivities
{
    [ToolboxBitmapAttribute(typeof(PolicyActivity), "Resources.Rule.png")]
    [ToolboxItemAttribute(typeof(ActivityToolboxItem))]
    public partial class PolicyFromService : System.Workflow.ComponentModel.Activity
    {
        public PolicyFromService()
        {
            InitializeComponent();
        }

        protected override ActivityExecutionStatus Execute(ActivityExecutionContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            ExternalRuleSetService.ExternalRuleSetService ruleSetService = context.GetService<ExternalRuleSetService.ExternalRuleSetService>();

            if (ruleSetService != null)
            {
                RuleSet ruleSet = ruleSetService.GetRuleSet(new RuleSetInfo(this.RuleSetName, this.MajorVersion, this.MinorVersion));
                if (ruleSet != null)
                {
                    Activity targetActivity = this.GetRootWorkflow(this.Parent);
                    RuleValidation validation = new RuleValidation(targetActivity.GetType(), null);
                    RuleExecution execution = new RuleExecution(validation, targetActivity, context);
                    ruleSet.Execute(execution);
                }
            }
            else
            {
                throw new InvalidOperationException("A RuleSetService must be configured on the host to use the PolicyFromService activity.");
            }

            return ActivityExecutionStatus.Closed;
        }

        private CompositeActivity GetRootWorkflow(CompositeActivity activity)
        {
            if (activity.Parent != null)
            {
                CompositeActivity workflow = GetRootWorkflow(activity.Parent);
                return workflow;
            }
            else
            {
                return activity;
            }
        }

        #region Dependency properties

        public static DependencyProperty RuleSetNameProperty = DependencyProperty.Register("RuleSetName", typeof(System.String), typeof(PolicyActivities.PolicyFromService));

        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Visible)]
        [ValidationOption(ValidationOption.Required)]
        [BrowsableAttribute(true)]
        public string RuleSetName
        {
            get
            {
                return ((String)(base.GetValue(PolicyActivities.PolicyFromService.RuleSetNameProperty)));
            }
            set
            {
                base.SetValue(PolicyActivities.PolicyFromService.RuleSetNameProperty, value);
            }
        }

        public static DependencyProperty MajorVersionProperty = DependencyProperty.Register("MajorVersion", typeof(System.Int32), typeof(PolicyActivities.PolicyFromService));

        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Visible)]
        [ValidationOption(ValidationOption.Optional)]
        [BrowsableAttribute(true)]
        public int MajorVersion
        {
            get
            {
                return ((int)(base.GetValue(PolicyActivities.PolicyFromService.MajorVersionProperty)));
            }
            set
            {
                base.SetValue(PolicyActivities.PolicyFromService.MajorVersionProperty, value);
            }
        }

        public static DependencyProperty MinorVersionProperty = DependencyProperty.Register("MinorVersion", typeof(System.Int32), typeof(PolicyActivities.PolicyFromService));

        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Visible)]
        [ValidationOption(ValidationOption.Optional)]
        [BrowsableAttribute(true)]
        public int MinorVersion
        {
            get
            {
                return ((int)(base.GetValue(PolicyActivities.PolicyFromService.MinorVersionProperty)));
            }
            set
            {
                base.SetValue(PolicyActivities.PolicyFromService.MinorVersionProperty, value);
            }
        }

        #endregion
    }
}
