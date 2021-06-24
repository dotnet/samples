//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

namespace Microsoft.Samples.Rules.ExternalRuleSetToolkit
{
    partial class RuleSetSelector
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.instructionsTextBox = new System.Windows.Forms.TextBox();
            this.ruleSetsListBox = new System.Windows.Forms.ListBox();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // instructionsTextBox
            // 
            this.instructionsTextBox.BackColor = System.Drawing.SystemColors.MenuBar;
            this.instructionsTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.instructionsTextBox.Location = new System.Drawing.Point(13, 13);
            this.instructionsTextBox.Multiline = true;
            this.instructionsTextBox.Name = "instructionsTextBox";
            this.instructionsTextBox.Size = new System.Drawing.Size(301, 49);
            this.instructionsTextBox.TabIndex = 0;
            this.instructionsTextBox.TabStop = false;
            this.instructionsTextBox.Text = "Select the RuleSets you would like included.";
            // 
            // ruleSetsListBox
            // 
            this.ruleSetsListBox.FormattingEnabled = true;
            this.ruleSetsListBox.Location = new System.Drawing.Point(13, 68);
            this.ruleSetsListBox.Name = "ruleSetsListBox";
            this.ruleSetsListBox.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.ruleSetsListBox.Size = new System.Drawing.Size(301, 238);
            this.ruleSetsListBox.TabIndex = 1;
            // 
            // okButton
            // 
            this.okButton.Location = new System.Drawing.Point(158, 317);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 0;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(239, 317);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 2;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // RuleSetSelectorForm
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(326, 352);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.ruleSetsListBox);
            this.Controls.Add(this.instructionsTextBox);
            this.Name = "RuleSetSelectorForm";
            this.Text = "RuleSet Selector";
            this.Load += new System.EventHandler(this.RuleSetSelectorForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox instructionsTextBox;
        private System.Windows.Forms.ListBox ruleSetsListBox;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
    }
}
