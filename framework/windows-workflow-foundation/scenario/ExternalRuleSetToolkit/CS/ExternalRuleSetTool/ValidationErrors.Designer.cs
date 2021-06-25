//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

namespace Microsoft.Samples.Rules.ExternalRuleSetToolkit
{
    partial class ValidationErrorsForm
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
            this.validationErrorsBox = new System.Windows.Forms.ListBox();
            this.continueButton = new System.Windows.Forms.Button();
            this.validationErrorsLabel = new System.Windows.Forms.Label();
            this.cancelButton = new System.Windows.Forms.Button();
            this.errorsMessageBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // validationErrorsBox
            // 
            this.validationErrorsBox.BackColor = System.Drawing.SystemColors.Control;
            this.validationErrorsBox.FormattingEnabled = true;
            this.validationErrorsBox.HorizontalScrollbar = true;
            this.validationErrorsBox.Location = new System.Drawing.Point(13, 25);
            this.validationErrorsBox.Name = "validationErrorsBox";
            this.validationErrorsBox.SelectionMode = System.Windows.Forms.SelectionMode.None;
            this.validationErrorsBox.Size = new System.Drawing.Size(720, 186);
            this.validationErrorsBox.TabIndex = 1;
            this.validationErrorsBox.TabStop = false;
            // 
            // continueButton
            // 
            this.continueButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.continueButton.Location = new System.Drawing.Point(577, 302);
            this.continueButton.Name = "continueButton";
            this.continueButton.Size = new System.Drawing.Size(75, 23);
            this.continueButton.TabIndex = 0;
            this.continueButton.Text = "Continue";
            this.continueButton.Click += new System.EventHandler(this.continueButton_Click);
            // 
            // validationErrorsLabel
            // 
            this.validationErrorsLabel.AutoSize = true;
            this.validationErrorsLabel.Location = new System.Drawing.Point(13, 6);
            this.validationErrorsLabel.Name = "validationErrorsLabel";
            this.validationErrorsLabel.Size = new System.Drawing.Size(86, 13);
            this.validationErrorsLabel.TabIndex = 0;
            this.validationErrorsLabel.Text = "Validation Errors:";
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(658, 302);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 1;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // errorsMessageBox
            // 
            this.errorsMessageBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.errorsMessageBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.errorsMessageBox.Location = new System.Drawing.Point(16, 227);
            this.errorsMessageBox.Multiline = true;
            this.errorsMessageBox.Name = "errorsMessageBox";
            this.errorsMessageBox.ReadOnly = true;
            this.errorsMessageBox.Size = new System.Drawing.Size(720, 58);
            this.errorsMessageBox.TabIndex = 2;
            this.errorsMessageBox.TabStop = false;
            // 
            // ValidationErrorsForm
            // 
            this.AcceptButton = this.continueButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(745, 337);
            this.Controls.Add(this.errorsMessageBox);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.validationErrorsLabel);
            this.Controls.Add(this.continueButton);
            this.Controls.Add(this.validationErrorsBox);
            this.Name = "ValidationErrorsForm";
            this.Text = "Validation Errors";
            this.Load += new System.EventHandler(this.ValidationErrorsForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox validationErrorsBox;
        private System.Windows.Forms.Button continueButton;
        private System.Windows.Forms.Label validationErrorsLabel;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.TextBox errorsMessageBox;
    }
}
