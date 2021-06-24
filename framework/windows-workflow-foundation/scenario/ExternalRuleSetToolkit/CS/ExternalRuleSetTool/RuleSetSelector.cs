//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Forms;
using Microsoft.Samples.Rules.ExternalRuleSetLibrary;

namespace Microsoft.Samples.Rules.ExternalRuleSetToolkit
{
    internal partial class RuleSetSelector : Form
    {
        #region Variables and constructor

        internal RuleSetSelector()
        {
            InitializeComponent();
        }

        private List<RuleSetData> ruleSetDataCollection = new List<RuleSetData>();
        private List<RuleSetData> selectedRuleSetDataCollection = new List<RuleSetData>();
        private bool selectAll;
        private string instructions;

        #endregion

        #region Properties

        internal string Instructions
        {
            get { return instructions; }
            set { instructions = value; }
        }

        internal bool SelectAll
        {
            get { return selectAll; }
            set { selectAll = value; }
        }

        internal List<RuleSetData> RuleSetDataCollection
        {
            get { return ruleSetDataCollection; }
        }

        internal List<RuleSetData> SelectedRuleSetDataCollection
        {
            get { return selectedRuleSetDataCollection; }
        }

        #endregion

        #region Form load
        
        private void RuleSetSelectorForm_Load(object sender, EventArgs e)
        {
            instructionsTextBox.Text = instructions;
            ruleSetDataCollection.Sort();

            ruleSetsListBox.DataSource = ruleSetDataCollection;
            if (selectAll)
            {
                foreach (object item in ruleSetDataCollection)
                {
                    ruleSetsListBox.SelectedItems.Add(item);
                }
            }
        }

        #endregion

        #region Event handlers

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            selectedRuleSetDataCollection.Clear();

            foreach (object item in ruleSetsListBox.SelectedItems)
            {
                selectedRuleSetDataCollection.Add(item as RuleSetData);
            }

            string duplicateRuleSetName;

            if (this.ValidateUniqueness(out duplicateRuleSetName))
            {
                this.Close();
                this.DialogResult = DialogResult.OK;
            }
            else
            {
                MessageBox.Show(string.Format(CultureInfo.InvariantCulture, "Multiple RuleSets selected with the same Name: '{0}'.", duplicateRuleSetName), "Export Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool ValidateUniqueness(out string duplicateRuleSetName)
        {
            foreach (RuleSetData data1 in selectedRuleSetDataCollection)
            {
                foreach (RuleSetData data2 in selectedRuleSetDataCollection)
                {
                    if (data1 != data2 && String.CompareOrdinal(data1.Name, data2.Name) == 0)
                    {
                        duplicateRuleSetName = data1.Name;
                        return false;
                    }
                }
            }
            duplicateRuleSetName = null;
            return true;
        }

        #endregion
    }
}
