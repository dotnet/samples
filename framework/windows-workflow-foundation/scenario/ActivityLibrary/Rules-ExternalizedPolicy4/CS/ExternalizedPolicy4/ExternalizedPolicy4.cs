//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

using System;
using System.Activities;
using System.ComponentModel;
using System.IO;
using System.Workflow.Activities.Rules;
using System.Workflow.ComponentModel.Compiler;
using System.Workflow.ComponentModel.Serialization;
using System.Xml;

namespace Microsoft.Samples.Activities.Rules
{
    /// <summary>
    /// This activity allows openening an existing .rules file
    /// and execute one of its rules against a TargetObject.
    /// Users can modify the ruleset (add, update, delete rules).
    /// </summary>
    [Designer(typeof(Microsoft.Samples.Activities.Rules.ExternalizedPolicy4Designer))]    
    public sealed class ExternalizedPolicy4<T> : CodeActivity
    {
        // path of the .rules file
        public string RulesFilePath { get; set; }

        // ruleset to execute
        public string RuleSetName { get; set; }

        // target object which to apply the rules
        [RequiredArgument]
        public InArgument<T> TargetObject { get; set; }

        // updated object after the rules are applied
        [RequiredArgument]
        public OutArgument<T> ResultObject { get; set; }

        // list of validation errors
        [DefaultValue(null)]
        public OutArgument<ValidationErrorCollection> ValidationErrors { get; set; }

        protected override void Execute(CodeActivityContext context)
        {                        
            if (RulesFilePath == null || RuleSetName == null)                
            {
                throw new InvalidOperationException("Rules File path and RuleSet Name need to be configured");
            }

            if (!File.Exists(RulesFilePath))
            {
                throw new InvalidOperationException("Rules File " + RulesFilePath + " did not exist");
            }
           
            // Get the RuleSet from the .rules file
            WorkflowMarkupSerializer serializer = new WorkflowMarkupSerializer();
            XmlTextReader reader = new XmlTextReader(RulesFilePath);
            RuleDefinitions rules = serializer.Deserialize(reader) as RuleDefinitions;
            RuleSet ruleSet = rules.RuleSets[RuleSetName];
            if (ruleSet == null)
            {
                throw new InvalidOperationException("RuleSet " + RuleSetName + " not found in " + RulesFilePath);
            }

            // Validate before running
            Type targetType = this.TargetObject.Get(context).GetType();
            RuleValidation validation = new RuleValidation(targetType, null);
            if (!ruleSet.Validate(validation))
            {
                // Set the ValidationErrors OutArgument
                this.ValidationErrors.Set(context, validation.Errors);

                // Throw exception
                throw new ValidationException(string.Format("The ruleset is not valid. {0} validation errors found (check the ValidationErrors property for more information).", validation.Errors.Count));                
            }
            
            // Execute the ruleset
            object evaluatedTarget = this.TargetObject.Get(context);
            RuleEngine engine = new RuleEngine(ruleSet, validation);            
            engine.Execute(evaluatedTarget);

            // Update the Result object
            this.ResultObject.Set(context, evaluatedTarget);               
        }        
    }
}
