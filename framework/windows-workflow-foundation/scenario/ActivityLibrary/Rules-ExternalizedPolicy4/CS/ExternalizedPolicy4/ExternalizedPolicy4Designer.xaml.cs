//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

using System;
using System.Activities;
using System.Activities.Presentation;
using System.Activities.Presentation.Model;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using System.Workflow.Activities.Rules;
using System.Workflow.Activities.Rules.Design;
using System.Workflow.ComponentModel.Serialization;

namespace Microsoft.Samples.Activities.Rules
{
    // Interaction logic for ExternalizedPolicy4Designer.xaml
    public partial class ExternalizedPolicy4Designer
    {
        public ExternalizedPolicy4Designer()
        {
            InitializeComponent();
        }       

        void viewRuleSetButton_Click(object sender, RoutedEventArgs e)
        {                    
            // get the rulesFilePath property
            object rulesFilePath = ModelItem.Properties["RulesFilePath"].Value;
            rulesFilePath = ((ModelItem)rulesFilePath).GetCurrentValue();

            // correct the rules file path (in case is a relative path)
            string correctedRulesFilePath = GetRulesFilePath((string)rulesFilePath);

            // verify that RulesFilePath property has been configured            
            if (rulesFilePath == null || !(rulesFilePath is string))
            {                                
                System.Windows.MessageBox.Show("Rules File Path needs to be configured before viewing or editing the rules");
                return;
            }
            else if (!File.Exists(correctedRulesFilePath))
            {
                System.Windows.MessageBox.Show(string.Format("Rules File Path provided not found ({0})", correctedRulesFilePath));
                return;
            }

            // verify that RuleSetName property has been configured
            object ruleSetName = ModelItem.Properties["RuleSetName"].Value;
            ruleSetName = ((ModelItem)ruleSetName).GetCurrentValue();
            if (ruleSetName == null)
            {
                System.Windows.MessageBox.Show("RuleSet Name needs to be configured before viewing or editing the rules");
                return;
            }

            // verify that TargetObject property has been configured
            object targetObject = ModelItem.Properties["TargetObject"].Value;            
            targetObject = ((ModelItem)targetObject).GetCurrentValue();
            if (targetObject == null)
            {
                System.Windows.MessageBox.Show("TargetObject needs to be configured before viewing or editing the rules");
                return;
            }

            // verify that target object is correctly configured
            InArgument targetObjArg = targetObject as InArgument;
            if (targetObjArg == null)
            {
                System.Windows.MessageBox.Show("Invalid target object");
                return;
            }

            // open the ruleset editor
            Type targetObjectType = targetObjArg.ArgumentType;
            WorkflowMarkupSerializer ser = new WorkflowMarkupSerializer();
            RuleDefinitions ruleDefs = ser.Deserialize(new System.Xml.XmlTextReader((string)correctedRulesFilePath)) as RuleDefinitions;
            RuleSet ruleSet = ruleDefs.RuleSets[(string)ruleSetName];
            
            // popup the dialog for viewing the rules            
            RuleSetDialog ruleSetDialog = new RuleSetDialog(targetObjectType, null, ruleSet);
            DialogResult result = ruleSetDialog.ShowDialog();

            // update if they changed the Rules
            if (result == DialogResult.OK) //If OK was pressed
            {
                for (int index = 0; index < ruleDefs.RuleSets.Count; index++)
                {
                    if (ruleDefs.RuleSets[index].Name == (string)ruleSetName)
                    {
                        ruleDefs.RuleSets[index] = ruleSetDialog.RuleSet;
                        break;
                    }
                }
                try
                {
                    ser.Serialize(new System.Xml.XmlTextWriter(correctedRulesFilePath, null), ruleDefs);
                }
                catch (UnauthorizedAccessException)
                {
                    // File does not have write access. Make a local copy so user changes are not lost
                    FileInfo fileInfo = new FileInfo(correctedRulesFilePath);
                    // create local file by adding a random suffix to original filename
                    string localFileCopy = fileInfo.Name.Substring(0, fileInfo.Name.IndexOf('.')) + new Random().Next() + fileInfo.Extension;
                    ser.Serialize(new System.Xml.XmlTextWriter((string)localFileCopy, null), ruleDefs);
                    System.Windows.MessageBox.Show("Rules file is not writeable. Created copy of your changes in " + localFileCopy);
                }
            }   
        }

        void fileBrowseButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = ".rules"; // Default file extension
            dlg.Filter = "Rules Files (.rules)|*.rules"; // Filter files by extension

            // Show open file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                // Open document
                ModelItem.Properties["RulesFilePath"].SetValue(dlg.FileName);
            }
        }

        string GetRulesFilePath(string rulesFilePath)
        {
            // if the path is relative, fix it
            if (!Path.IsPathRooted(rulesFilePath))
            {
                // get the path of the current designer
                WorkflowFileItem workflowFileItem = Context.Items.GetValue<WorkflowFileItem>();
                return Path.Combine(Path.GetDirectoryName(workflowFileItem.LoadedFile), rulesFilePath);
            }
            else
            {
                return rulesFilePath;
            }
        }
    }
}
