//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using System.Workflow.Activities.Rules;

namespace Microsoft.Samples.Rules.ExternalRuleSetToolkit
{
    internal partial class ActivitySelector : Form
    {
        #region Variables and constructor

        internal ActivitySelector()
        {
            InitializeComponent();
        }
        private Type activity;
        private string assemblyPath;
        private RuleSet ruleSet;
        private string initialDirectory;
       
        #endregion

        #region Properties

        internal RuleSet RuleSet
        {
            get { return ruleSet; }
            set { ruleSet = value; }
        }

        internal string AssemblyPath
        {
            get
            {
                return assemblyPath;
            }
            set
            {
                assemblyPath = value;
            }
        }

        internal Type Activity
        {
            get
            {
                return activity;
            }
            set
            {
                activity = value;
            }
        }

        internal string InitialDirectory
        {
            get { return initialDirectory; }
            set { initialDirectory = value; }
        }

        #endregion

        #region Form load and setup
        
        private void ActivitySelector_Load(object sender, EventArgs e)
        {
            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.AssemblyResolve += new ResolveEventHandler(activitySelector_AssemblyResolve);
            
            if (!String.IsNullOrEmpty(assemblyPath))
            {
                assemblyBox.Text = assemblyPath;
                this.PopulateActivities();
            }

            instructionsBox.Text = String.Format(CultureInfo.InvariantCulture, "Select a target Type associated with the RuleSet: '{0}'", ruleSet.Name);
        }

        Assembly activitySelector_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            if (!String.IsNullOrEmpty(assemblyBox.Text))
            {
                return RuleSetToolkitEditor.ResolveAssembly(assemblyBox.Text, args.Name);
            }
            else
            {
                return null;
            }
        }

        #endregion

        #region Event handlers

        private void activitiesBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            membersBox.Items.Clear();

            if (activitiesBox.SelectedItem != null)
            {
               membersBox.Items.AddRange(RuleSetToolkitEditor.GetMembers(activitiesBox.SelectedItem as Type).ToArray());
            }
        }

        private void pickAssemblyButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Component Files (*.dll;*.exe)|*.dll;*.exe";
            if (!String.IsNullOrEmpty(assemblyPath))
                dialog.InitialDirectory = Path.GetDirectoryName(assemblyPath);
            else if (!String.IsNullOrEmpty(initialDirectory))
                dialog.InitialDirectory = initialDirectory;

            DialogResult result = dialog.ShowDialog();
            if (result == DialogResult.OK && dialog.FileName != null)
            {
                assemblyBox.Text = dialog.FileName;
                this.PopulateActivities();
            }
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            if (ruleSet == null) //no ruleset picked yet, so no validation required
                this.Close();

            if (activitiesBox.SelectedItem != null)
            {
                if (activity == null)
                {
                    //in most cases this means that it is a new RuleSet, but I may have opened a rule set file
                    //that didn't have the activity set, so need to validate that this is the correct activity
                    if (ValidateRuleSet(ruleSet, activitiesBox.SelectedItem as Type, true))
                    {
                        activity = activitiesBox.SelectedItem as Type;
                        assemblyPath = assemblyBox.Text;
                        this.Close();
                    }
                }
                else if (activity.UnderlyingSystemType != (activitiesBox.SelectedItem as Type).UnderlyingSystemType)
                {
                    // user picked a different Type
                    DialogResult result = MessageBox.Show("Are you sure you want to change the Type associated with this RuleSet?", "Change Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        if (ValidateRuleSet(ruleSet, activitiesBox.SelectedItem as Type,true))
                        {
                            activity = activitiesBox.SelectedItem as Type;
                            assemblyPath = assemblyBox.Text;
                            this.Close();
                        }
                    }
                }
                else // user didn't change the Type
                {
                    this.Close();
                }
            }
            else // no Type selected so nothing to validate
            {
                this.Close();
            }
        }
      
        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #endregion

        #region Other

        private void PopulateActivities()
        {
            activitiesBox.Items.Clear();

            Assembly assembly = RuleSetToolkitEditor.LoadAssembly(assemblyBox.Text);

            if (assembly != null)
            {
                try
                {
                    Type[] types = assembly.GetTypes();
                    foreach (Type type in types)
                    {
                        // add a check here if you want to constrain the kinds of Types (e.g. Activity) that rulesets can be authored against
                        
                        //if (type.IsSubclassOf(typeof(Activity)))
                        //{
                            activitiesBox.Items.Add(type);
                        //}
                    }
                }
                catch (ReflectionTypeLoadException ex)
                {
                    MessageBox.Show(string.Format(CultureInfo.InvariantCulture, "Error loading types from assembly '{0}': \r\n\n{1}", assembly.FullName, ex.LoaderExceptions[0].Message), "Type Load Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            if (activity != null && activitiesBox.Items.Contains(activity))
            {
                activitiesBox.SelectedItem = activity;
            }

            else if (activitiesBox.Items.Count > 0)
            {
                activitiesBox.SetSelected(0, true);
            }
        }

        internal static bool ValidateRuleSet(RuleSet ruleSetToValidate, Type targetType, bool promptForContinue)
        {
            if (ruleSetToValidate == null)
            {
                MessageBox.Show("No RuleSet selected.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;  
            }

            if (targetType == null)
            {
                MessageBox.Show("No Type is associated with the RuleSet.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;  
            }

            RuleValidation ruleValidation = new RuleValidation(targetType, null);
            ruleSetToValidate.Validate(ruleValidation);
            if (ruleValidation.Errors.Count == 0)
            {
                return true;
            }
            else
            {
                ValidationErrorsForm validationDialog = new ValidationErrorsForm();
                validationDialog.SetValidationErrors(ruleValidation.Errors);
                validationDialog.PromptForContinue = promptForContinue;
                validationDialog.ErrorText = "The RuleSet failed validation.  Ensure that the selected Type has the public members referenced by this RuleSet.  Note that false errors may occur if you are referencing different copies of an assembly with the same strong name.";
                if (promptForContinue)
                    validationDialog.ErrorText += "  Select Continue to proceed or Cancel to return.";
                
                validationDialog.ShowDialog();

                if (!promptForContinue)
                    return false;
                else
                    return validationDialog.ContinueWithChange;
            }
        }

        #endregion
    }
}
