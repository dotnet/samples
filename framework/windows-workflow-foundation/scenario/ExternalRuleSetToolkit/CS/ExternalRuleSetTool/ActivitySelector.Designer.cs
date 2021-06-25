//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

namespace Microsoft.Samples.Rules.ExternalRuleSetToolkit
{
    partial class ActivitySelector
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
            this.pickAssemblyButton = new System.Windows.Forms.Button();
            this.activitiesBox = new System.Windows.Forms.ListBox();
            this.selectActivityLabel = new System.Windows.Forms.Label();
            this.assemblyBox = new System.Windows.Forms.TextBox();
            this.membersLabel = new System.Windows.Forms.Label();
            this.membersBox = new System.Windows.Forms.ListBox();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.selectedAssemblyLabel = new System.Windows.Forms.Label();
            this.instructionsBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // pickAssemblyButton
            // 
            this.pickAssemblyButton.Location = new System.Drawing.Point(391, 68);
            this.pickAssemblyButton.Name = "pickAssemblyButton";
            this.pickAssemblyButton.Size = new System.Drawing.Size(73, 23);
            this.pickAssemblyButton.TabIndex = 1;
            this.pickAssemblyButton.Text = "&Browse...";
            this.pickAssemblyButton.Click += new System.EventHandler(this.pickAssemblyButton_Click);
            // 
            // activitiesBox
            // 
            this.activitiesBox.FormattingEnabled = true;
            this.activitiesBox.HorizontalScrollbar = true;
            this.activitiesBox.Location = new System.Drawing.Point(13, 126);
            this.activitiesBox.Name = "activitiesBox";
            this.activitiesBox.Size = new System.Drawing.Size(451, 95);
            this.activitiesBox.TabIndex = 2;
            this.activitiesBox.SelectedIndexChanged += new System.EventHandler(this.activitiesBox_SelectedIndexChanged);
            // 
            // selectActivityLabel
            // 
            this.selectActivityLabel.AutoSize = true;
            this.selectActivityLabel.Location = new System.Drawing.Point(13, 107);
            this.selectActivityLabel.Name = "selectActivityLabel";
            this.selectActivityLabel.Size = new System.Drawing.Size(90, 13);
            this.selectActivityLabel.TabIndex = 3;
            this.selectActivityLabel.Text = "Contained Types:";
            // 
            // assemblyBox
            // 
            this.assemblyBox.Location = new System.Drawing.Point(12, 70);
            this.assemblyBox.Name = "assemblyBox";
            this.assemblyBox.ReadOnly = true;
            this.assemblyBox.Size = new System.Drawing.Size(373, 20);
            this.assemblyBox.TabIndex = 1;
            this.assemblyBox.TabStop = false;
            // 
            // membersLabel
            // 
            this.membersLabel.AutoSize = true;
            this.membersLabel.Location = new System.Drawing.Point(14, 232);
            this.membersLabel.Name = "membersLabel";
            this.membersLabel.Size = new System.Drawing.Size(53, 13);
            this.membersLabel.TabIndex = 5;
            this.membersLabel.Text = "Members:";
            // 
            // membersBox
            // 
            this.membersBox.BackColor = System.Drawing.SystemColors.Control;
            this.membersBox.FormattingEnabled = true;
            this.membersBox.HorizontalScrollbar = true;
            this.membersBox.Location = new System.Drawing.Point(15, 249);
            this.membersBox.Name = "membersBox";
            this.membersBox.SelectionMode = System.Windows.Forms.SelectionMode.None;
            this.membersBox.Size = new System.Drawing.Size(449, 95);
            this.membersBox.TabIndex = 6;
            this.membersBox.TabStop = false;
            // 
            // okButton
            // 
            this.okButton.Location = new System.Drawing.Point(310, 350);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 0;
            this.okButton.Text = "OK";
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(391, 350);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 3;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // selectedAssemblyLabel
            // 
            this.selectedAssemblyLabel.AutoSize = true;
            this.selectedAssemblyLabel.Location = new System.Drawing.Point(13, 51);
            this.selectedAssemblyLabel.Name = "selectedAssemblyLabel";
            this.selectedAssemblyLabel.Size = new System.Drawing.Size(99, 13);
            this.selectedAssemblyLabel.TabIndex = 0;
            this.selectedAssemblyLabel.Text = "Selected Assembly:";
            // 
            // instructionsBox
            // 
            this.instructionsBox.BackColor = System.Drawing.SystemColors.Control;
            this.instructionsBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.instructionsBox.Location = new System.Drawing.Point(13, 13);
            this.instructionsBox.Name = "instructionsBox";
            this.instructionsBox.Size = new System.Drawing.Size(451, 13);
            this.instructionsBox.TabIndex = 9;
            this.instructionsBox.TabStop = false;
            // 
            // ActivitySelector
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(476, 384);
            this.Controls.Add(this.instructionsBox);
            this.Controls.Add(this.selectedAssemblyLabel);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.membersBox);
            this.Controls.Add(this.membersLabel);
            this.Controls.Add(this.assemblyBox);
            this.Controls.Add(this.selectActivityLabel);
            this.Controls.Add(this.activitiesBox);
            this.Controls.Add(this.pickAssemblyButton);
            this.Name = "ActivitySelector";
            this.Text = "Workflow / Type Selection";
            this.Load += new System.EventHandler(this.ActivitySelector_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button pickAssemblyButton;
        private System.Windows.Forms.ListBox activitiesBox;
        private System.Windows.Forms.Label selectActivityLabel;
        private System.Windows.Forms.TextBox assemblyBox;
        private System.Windows.Forms.Label membersLabel;
        private System.Windows.Forms.ListBox membersBox;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Label selectedAssemblyLabel;
        private System.Windows.Forms.TextBox instructionsBox;
    }
}
