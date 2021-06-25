//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System;
using System.Windows.Forms;
using System.Workflow.ComponentModel.Compiler;

namespace Microsoft.Samples.Rules.ExternalRuleSetToolkit
{
    internal partial class ValidationErrorsForm : Form
    {

        #region Constructor and variables
        
        internal ValidationErrorsForm()
        {
            InitializeComponent();
        }

        private bool promptForContinue;
        private bool continueWithChange;

        #endregion

        #region Form load

        private void ValidationErrorsForm_Load(object sender, EventArgs e)
        {
            if (!promptForContinue) //just showing the errors, so change the representation in the UI
            {
                continueButton.Hide();
                cancelButton.Text = "OK";
            }
        }

        internal void SetValidationErrors(ValidationErrorCollection errors)
        {

                foreach(ValidationError error in errors)
                {
                    validationErrorsBox.Items.Add(error);
                }
        }

        #endregion

        #region Properties

        internal bool PromptForContinue
        {
            get { return promptForContinue; }
            set { promptForContinue = value; }
        }

        internal string ErrorText
        {
            get { return errorsMessageBox.Text; }
            set { errorsMessageBox.Text = value; }
        }

        internal bool ContinueWithChange
        {
            get { return continueWithChange; }
        }

        #endregion

        #region Event handlers

        private void continueButton_Click(object sender, EventArgs e)
        {
            continueWithChange = true;
            this.Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #endregion
    }
}
