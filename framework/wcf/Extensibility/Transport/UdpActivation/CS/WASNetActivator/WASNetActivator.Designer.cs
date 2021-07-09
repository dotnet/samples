//  Copyright (c) Microsoft Corporation.  All Rights Reserved.

namespace Microsoft.ServiceModel.Samples
{
    partial class WASNetActivator
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
            this.label1 = new System.Windows.Forms.Label();
            this.btnStart = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.cbbProtocol = new System.Windows.Forms.ComboBox();
            this.tabCtrlActions = new System.Windows.Forms.TabControl();
            this.activationTabPage = new System.Windows.Forms.TabPage();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.setupTabPage = new System.Windows.Forms.TabPage();
            this.btnUninstall = new System.Windows.Forms.Button();
            this.btnInstall = new System.Windows.Forms.Button();
            this.chkProtocols = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.chkListenerAdapter = new System.Windows.Forms.CheckBox();
            this.tabCtrlActions.SuspendLayout();
            this.activationTabPage.SuspendLayout();
            this.setupTabPage.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(264, 17);
            this.label1.TabIndex = 0;
            this.label1.Text = "Select the protocol that you want to activate through";
            this.label1.UseCompatibleTextRendering = true;
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(33, 115);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(75, 23);
            this.btnStart.TabIndex = 2;
            this.btnStart.Text = "Start";
            this.btnStart.UseCompatibleTextRendering = true;
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // btnStop
            // 
            this.btnStop.Enabled = false;
            this.btnStop.Location = new System.Drawing.Point(130, 114);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(75, 23);
            this.btnStop.TabIndex = 3;
            this.btnStop.Text = "Stop";
            this.btnStop.UseCompatibleTextRendering = true;
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // cbbProtocol
            // 
            this.cbbProtocol.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbbProtocol.FormattingEnabled = true;
            this.cbbProtocol.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.cbbProtocol.Items.AddRange(new object[] {
            "UDP"});
            this.cbbProtocol.Location = new System.Drawing.Point(74, 51);
            this.cbbProtocol.Name = "cbbProtocol";
            this.cbbProtocol.Size = new System.Drawing.Size(96, 21);
            this.cbbProtocol.TabIndex = 1;
            // 
            // tabCtrlActions
            // 
            this.tabCtrlActions.Controls.Add(this.activationTabPage);
            this.tabCtrlActions.Controls.Add(this.setupTabPage);
            this.tabCtrlActions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabCtrlActions.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabCtrlActions.Location = new System.Drawing.Point(0, 0);
            this.tabCtrlActions.Name = "tabCtrlActions";
            this.tabCtrlActions.SelectedIndex = 0;
            this.tabCtrlActions.Size = new System.Drawing.Size(307, 280);
            this.tabCtrlActions.TabIndex = 6;
            // 
            // activationTabPage
            // 
            this.activationTabPage.Controls.Add(this.label3);
            this.activationTabPage.Controls.Add(this.label2);
            this.activationTabPage.Controls.Add(this.label1);
            this.activationTabPage.Controls.Add(this.cbbProtocol);
            this.activationTabPage.Controls.Add(this.btnStop);
            this.activationTabPage.Controls.Add(this.btnStart);
            this.activationTabPage.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.activationTabPage.Location = new System.Drawing.Point(4, 22);
            this.activationTabPage.Name = "activationTabPage";
            this.activationTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.activationTabPage.Size = new System.Drawing.Size(299, 254);
            this.activationTabPage.TabIndex = 0;
            this.activationTabPage.Text = "Activation";
            this.activationTabPage.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(10, 83);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(133, 17);
            this.label3.TabIndex = 6;
            this.label3.Text = "Start or stop the activator:";
            this.label3.UseCompatibleTextRendering = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(10, 31);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(183, 17);
            this.label2.TabIndex = 5;
            this.label2.Text = "Windows Activation Service (WAS):";
            this.label2.UseCompatibleTextRendering = true;
            // 
            // setupTabPage
            // 
            this.setupTabPage.Controls.Add(this.btnUninstall);
            this.setupTabPage.Controls.Add(this.btnInstall);
            this.setupTabPage.Controls.Add(this.chkProtocols);
            this.setupTabPage.Controls.Add(this.label4);
            this.setupTabPage.Controls.Add(this.chkListenerAdapter);
            this.setupTabPage.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.setupTabPage.Location = new System.Drawing.Point(4, 22);
            this.setupTabPage.Name = "setupTabPage";
            this.setupTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.setupTabPage.Size = new System.Drawing.Size(299, 254);
            this.setupTabPage.TabIndex = 1;
            this.setupTabPage.Text = "Setup";
            this.setupTabPage.UseVisualStyleBackColor = true;
            // 
            // btnUninstall
            // 
            this.btnUninstall.Location = new System.Drawing.Point(148, 90);
            this.btnUninstall.Name = "btnUninstall";
            this.btnUninstall.Size = new System.Drawing.Size(75, 23);
            this.btnUninstall.TabIndex = 7;
            this.btnUninstall.Text = "Uninstall";
            this.btnUninstall.UseCompatibleTextRendering = true;
            this.btnUninstall.UseVisualStyleBackColor = true;
            this.btnUninstall.Click += new System.EventHandler(this.btnUninstall_Click);
            // 
            // btnInstall
            // 
            this.btnInstall.Location = new System.Drawing.Point(26, 90);
            this.btnInstall.Name = "btnInstall";
            this.btnInstall.Size = new System.Drawing.Size(75, 23);
            this.btnInstall.TabIndex = 6;
            this.btnInstall.Text = "Install";
            this.btnInstall.UseCompatibleTextRendering = true;
            this.btnInstall.UseVisualStyleBackColor = true;
            this.btnInstall.Click += new System.EventHandler(this.btnInstall_Click);
            // 
            // chkProtocols
            // 
            this.chkProtocols.AutoSize = true;
            this.chkProtocols.Location = new System.Drawing.Point(40, 51);
            this.chkProtocols.Name = "chkProtocols";
            this.chkProtocols.Size = new System.Drawing.Size(140, 18);
            this.chkProtocols.TabIndex = 2;
            this.chkProtocols.Text = "UDP Protocol Handlers";
            this.chkProtocols.UseCompatibleTextRendering = true;
            this.chkProtocols.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(8, 7);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(276, 17);
            this.label4.TabIndex = 1;
            this.label4.Text = "Select components that you want to install or uninstall:";
            this.label4.UseCompatibleTextRendering = true;
            // 
            // chkListenerAdapter
            // 
            this.chkListenerAdapter.AutoSize = true;
            this.chkListenerAdapter.Location = new System.Drawing.Point(40, 27);
            this.chkListenerAdapter.Name = "chkListenerAdapter";
            this.chkListenerAdapter.Size = new System.Drawing.Size(133, 18);
            this.chkListenerAdapter.TabIndex = 0;
            this.chkListenerAdapter.Text = "UDP Listener Adapter";
            this.chkListenerAdapter.UseCompatibleTextRendering = true;
            this.chkListenerAdapter.UseVisualStyleBackColor = true;
            // 
            // WASNetActivator
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(307, 280);
            this.Controls.Add(this.tabCtrlActions);
            this.Name = "WASNetActivator";
            this.Text = "WASNetActivator";
            this.tabCtrlActions.ResumeLayout(false);
            this.activationTabPage.ResumeLayout(false);
            this.activationTabPage.PerformLayout();
            this.setupTabPage.ResumeLayout(false);
            this.setupTabPage.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.ComboBox cbbProtocol;
        private System.Windows.Forms.TabControl tabCtrlActions;
        private System.Windows.Forms.TabPage activationTabPage;
        private System.Windows.Forms.TabPage setupTabPage;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox chkProtocols;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox chkListenerAdapter;
        private System.Windows.Forms.Button btnUninstall;
        private System.Windows.Forms.Button btnInstall;
    }
}


