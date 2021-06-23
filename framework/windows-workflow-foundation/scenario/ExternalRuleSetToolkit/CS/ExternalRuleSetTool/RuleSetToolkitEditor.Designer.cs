//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

namespace Microsoft.Samples.Rules.ExternalRuleSetToolkit
{
    partial class RuleSetToolkitEditor
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
            this.ruleSetNameCollectionLabel = new System.Windows.Forms.Label();
            this.getActivityButton = new System.Windows.Forms.Button();
            this.membersBox = new System.Windows.Forms.ListBox();
            this.membersLabel = new System.Windows.Forms.Label();
            this.membersGroup = new System.Windows.Forms.GroupBox();
            this.selectedActivityLabel = new System.Windows.Forms.Label();
            this.activityBox = new System.Windows.Forms.TextBox();
            this.editButton = new System.Windows.Forms.Button();
            this.ruleSetsGroupBox = new System.Windows.Forms.GroupBox();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.copyButton = new System.Windows.Forms.Button();
            this.minorVersionBox = new System.Windows.Forms.NumericUpDown();
            this.majorVersionBox = new System.Windows.Forms.NumericUpDown();
            this.ruleSetNameBox = new System.Windows.Forms.TextBox();
            this.minorVersionLabel = new System.Windows.Forms.Label();
            this.majorVersionLabel = new System.Windows.Forms.Label();
            this.ruleSetNameLabel = new System.Windows.Forms.Label();
            this.deleteButton = new System.Windows.Forms.Button();
            this.newButton = new System.Windows.Forms.Button();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dataToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.validateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.membersGroup.SuspendLayout();
            this.ruleSetsGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.minorVersionBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.majorVersionBox)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // ruleSetNameCollectionLabel
            // 
            this.ruleSetNameCollectionLabel.AutoSize = true;
            this.ruleSetNameCollectionLabel.Location = new System.Drawing.Point(12, 17);
            this.ruleSetNameCollectionLabel.Name = "ruleSetNameCollectionLabel";
            this.ruleSetNameCollectionLabel.Size = new System.Drawing.Size(91, 13);
            this.ruleSetNameCollectionLabel.TabIndex = 3;
            this.ruleSetNameCollectionLabel.Text = "RuleSet Versions:";
            // 
            // getActivityButton
            // 
            this.getActivityButton.Location = new System.Drawing.Point(308, 34);
            this.getActivityButton.Name = "getActivityButton";
            this.getActivityButton.Size = new System.Drawing.Size(81, 23);
            this.getActivityButton.TabIndex = 8;
            this.getActivityButton.Text = "&Browse...";
            this.getActivityButton.Click += new System.EventHandler(this.getActivityButton_Click);
            // 
            // membersBox
            // 
            this.membersBox.BackColor = System.Drawing.SystemColors.Control;
            this.membersBox.FormattingEnabled = true;
            this.membersBox.HorizontalScrollbar = true;
            this.membersBox.Location = new System.Drawing.Point(11, 75);
            this.membersBox.Name = "membersBox";
            this.membersBox.SelectionMode = System.Windows.Forms.SelectionMode.None;
            this.membersBox.Size = new System.Drawing.Size(378, 277);
            this.membersBox.TabIndex = 22;
            this.membersBox.TabStop = false;
            // 
            // membersLabel
            // 
            this.membersLabel.AutoSize = true;
            this.membersLabel.Location = new System.Drawing.Point(10, 59);
            this.membersLabel.Name = "membersLabel";
            this.membersLabel.Size = new System.Drawing.Size(53, 13);
            this.membersLabel.TabIndex = 21;
            this.membersLabel.Text = "Members:";
            // 
            // membersGroup
            // 
            this.membersGroup.Controls.Add(this.selectedActivityLabel);
            this.membersGroup.Controls.Add(this.membersBox);
            this.membersGroup.Controls.Add(this.activityBox);
            this.membersGroup.Controls.Add(this.getActivityButton);
            this.membersGroup.Controls.Add(this.membersLabel);
            this.membersGroup.Location = new System.Drawing.Point(376, 40);
            this.membersGroup.Name = "membersGroup";
            this.membersGroup.Size = new System.Drawing.Size(404, 370);
            this.membersGroup.TabIndex = 17;
            this.membersGroup.TabStop = false;
            this.membersGroup.Text = "Associated Type";
            // 
            // selectedActivityLabel
            // 
            this.selectedActivityLabel.AutoSize = true;
            this.selectedActivityLabel.Location = new System.Drawing.Point(10, 17);
            this.selectedActivityLabel.Name = "selectedActivityLabel";
            this.selectedActivityLabel.Size = new System.Drawing.Size(135, 13);
            this.selectedActivityLabel.TabIndex = 18;
            this.selectedActivityLabel.Text = "Selected Workflow / Type:";
            // 
            // activityBox
            // 
            this.activityBox.Location = new System.Drawing.Point(11, 35);
            this.activityBox.Name = "activityBox";
            this.activityBox.ReadOnly = true;
            this.activityBox.Size = new System.Drawing.Size(291, 20);
            this.activityBox.TabIndex = 19;
            this.activityBox.TabStop = false;
            // 
            // editButton
            // 
            this.editButton.Location = new System.Drawing.Point(258, 245);
            this.editButton.Name = "editButton";
            this.editButton.Size = new System.Drawing.Size(75, 23);
            this.editButton.TabIndex = 4;
            this.editButton.Text = "&Edit Rules";
            this.editButton.Click += new System.EventHandler(this.editButton_Click);
            // 
            // ruleSetsGroupBox
            // 
            this.ruleSetsGroupBox.Controls.Add(this.treeView1);
            this.ruleSetsGroupBox.Controls.Add(this.copyButton);
            this.ruleSetsGroupBox.Controls.Add(this.minorVersionBox);
            this.ruleSetsGroupBox.Controls.Add(this.majorVersionBox);
            this.ruleSetsGroupBox.Controls.Add(this.ruleSetNameBox);
            this.ruleSetsGroupBox.Controls.Add(this.minorVersionLabel);
            this.ruleSetsGroupBox.Controls.Add(this.majorVersionLabel);
            this.ruleSetsGroupBox.Controls.Add(this.ruleSetNameLabel);
            this.ruleSetsGroupBox.Controls.Add(this.deleteButton);
            this.ruleSetsGroupBox.Controls.Add(this.newButton);
            this.ruleSetsGroupBox.Controls.Add(this.editButton);
            this.ruleSetsGroupBox.Controls.Add(this.ruleSetNameCollectionLabel);
            this.ruleSetsGroupBox.Location = new System.Drawing.Point(12, 40);
            this.ruleSetsGroupBox.Name = "ruleSetsGroupBox";
            this.ruleSetsGroupBox.Size = new System.Drawing.Size(349, 370);
            this.ruleSetsGroupBox.TabIndex = 2;
            this.ruleSetsGroupBox.TabStop = false;
            this.ruleSetsGroupBox.Text = "RuleSets";
            // 
            // treeView1
            // 
            this.treeView1.Location = new System.Drawing.Point(10, 34);
            this.treeView1.Name = "treeView1";
            this.treeView1.Size = new System.Drawing.Size(326, 205);
            this.treeView1.TabIndex = 0;
            this.treeView1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterSelect);
            // 
            // copyButton
            // 
            this.copyButton.Location = new System.Drawing.Point(177, 245);
            this.copyButton.Name = "copyButton";
            this.copyButton.Size = new System.Drawing.Size(75, 23);
            this.copyButton.TabIndex = 3;
            this.copyButton.Text = "&Copy";
            this.copyButton.UseVisualStyleBackColor = true;
            this.copyButton.Click += new System.EventHandler(this.copyButton_Click);
            // 
            // minorVersionBox
            // 
            this.minorVersionBox.Location = new System.Drawing.Point(108, 342);
            this.minorVersionBox.Name = "minorVersionBox";
            this.minorVersionBox.Size = new System.Drawing.Size(40, 20);
            this.minorVersionBox.TabIndex = 7;
            // 
            // majorVersionBox
            // 
            this.majorVersionBox.Location = new System.Drawing.Point(108, 315);
            this.majorVersionBox.Name = "majorVersionBox";
            this.majorVersionBox.Size = new System.Drawing.Size(40, 20);
            this.majorVersionBox.TabIndex = 6;
            // 
            // ruleSetNameBox
            // 
            this.ruleSetNameBox.Location = new System.Drawing.Point(108, 285);
            this.ruleSetNameBox.Name = "ruleSetNameBox";
            this.ruleSetNameBox.Size = new System.Drawing.Size(144, 20);
            this.ruleSetNameBox.TabIndex = 5;
            // 
            // minorVersionLabel
            // 
            this.minorVersionLabel.AutoSize = true;
            this.minorVersionLabel.Location = new System.Drawing.Point(12, 344);
            this.minorVersionLabel.Name = "minorVersionLabel";
            this.minorVersionLabel.Size = new System.Drawing.Size(74, 13);
            this.minorVersionLabel.TabIndex = 11;
            this.minorVersionLabel.Text = "Minor Version:";
            // 
            // majorVersionLabel
            // 
            this.majorVersionLabel.AutoSize = true;
            this.majorVersionLabel.Location = new System.Drawing.Point(12, 317);
            this.majorVersionLabel.Name = "majorVersionLabel";
            this.majorVersionLabel.Size = new System.Drawing.Size(74, 13);
            this.majorVersionLabel.TabIndex = 10;
            this.majorVersionLabel.Text = "Major Version:";
            // 
            // ruleSetNameLabel
            // 
            this.ruleSetNameLabel.AutoSize = true;
            this.ruleSetNameLabel.Location = new System.Drawing.Point(12, 288);
            this.ruleSetNameLabel.Name = "ruleSetNameLabel";
            this.ruleSetNameLabel.Size = new System.Drawing.Size(79, 13);
            this.ruleSetNameLabel.TabIndex = 9;
            this.ruleSetNameLabel.Text = "RuleSet Name:";
            // 
            // deleteButton
            // 
            this.deleteButton.Location = new System.Drawing.Point(97, 245);
            this.deleteButton.Name = "deleteButton";
            this.deleteButton.Size = new System.Drawing.Size(75, 23);
            this.deleteButton.TabIndex = 2;
            this.deleteButton.Text = "&Delete";
            this.deleteButton.UseVisualStyleBackColor = true;
            this.deleteButton.Click += new System.EventHandler(this.deleteButton_Click);
            // 
            // newButton
            // 
            this.newButton.Location = new System.Drawing.Point(15, 246);
            this.newButton.Name = "newButton";
            this.newButton.Size = new System.Drawing.Size(75, 23);
            this.newButton.TabIndex = 1;
            this.newButton.Text = "&New";
            this.newButton.UseVisualStyleBackColor = true;
            this.newButton.Click += new System.EventHandler(this.newButton_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.dataToolStripMenuItem,
            this.toolsToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(792, 24);
            this.menuStrip1.TabIndex = 18;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(69, 20);
            this.fileToolStripMenuItem.Text = "&Rule Store";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.openToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.openToolStripMenuItem.Text = "&Load";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.saveToolStripMenuItem.Text = "&Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.exitToolStripMenuItem.Text = "E&xit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // dataToolStripMenuItem
            // 
            this.dataToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.importToolStripMenuItem,
            this.exportToolStripMenuItem});
            this.dataToolStripMenuItem.Name = "dataToolStripMenuItem";
            this.dataToolStripMenuItem.Size = new System.Drawing.Size(42, 20);
            this.dataToolStripMenuItem.Text = "D&ata";
            // 
            // importToolStripMenuItem
            // 
            this.importToolStripMenuItem.Name = "importToolStripMenuItem";
            this.importToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
            this.importToolStripMenuItem.Text = "&Import";
            this.importToolStripMenuItem.Click += new System.EventHandler(this.importToolStripMenuItem_Click);
            // 
            // exportToolStripMenuItem
            // 
            this.exportToolStripMenuItem.Name = "exportToolStripMenuItem";
            this.exportToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
            this.exportToolStripMenuItem.Text = "&Export";
            this.exportToolStripMenuItem.Click += new System.EventHandler(this.exportToolStripMenuItem_Click);
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.validateToolStripMenuItem});
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.toolsToolStripMenuItem.Text = "&Tools";
            // 
            // validateToolStripMenuItem
            // 
            this.validateToolStripMenuItem.Name = "validateToolStripMenuItem";
            this.validateToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
            this.validateToolStripMenuItem.Text = "&Validate";
            this.validateToolStripMenuItem.Click += new System.EventHandler(this.validateToolStripMenuItem_Click);
            // 
            // RuleSetEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(792, 428);
            this.Controls.Add(this.membersGroup);
            this.Controls.Add(this.ruleSetsGroupBox);
            this.Controls.Add(this.menuStrip1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "RuleSetEditor";
            this.Text = "RuleSet Browser";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.membersGroup.ResumeLayout(false);
            this.membersGroup.PerformLayout();
            this.ruleSetsGroupBox.ResumeLayout(false);
            this.ruleSetsGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.minorVersionBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.majorVersionBox)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label ruleSetNameCollectionLabel;
        private System.Windows.Forms.Button getActivityButton;
        private System.Windows.Forms.ListBox membersBox;
        private System.Windows.Forms.Label membersLabel;
        private System.Windows.Forms.GroupBox membersGroup;
        private System.Windows.Forms.TextBox activityBox;
        private System.Windows.Forms.Button editButton;
        private System.Windows.Forms.Label selectedActivityLabel;
        private System.Windows.Forms.GroupBox ruleSetsGroupBox;
        private System.Windows.Forms.Button deleteButton;
        private System.Windows.Forms.Button newButton;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.Label minorVersionLabel;
        private System.Windows.Forms.Label majorVersionLabel;
        private System.Windows.Forms.Label ruleSetNameLabel;
        private System.Windows.Forms.NumericUpDown minorVersionBox;
        private System.Windows.Forms.NumericUpDown majorVersionBox;
        private System.Windows.Forms.TextBox ruleSetNameBox;
        private System.Windows.Forms.ToolStripMenuItem dataToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem importToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportToolStripMenuItem;
        private System.Windows.Forms.Button copyButton;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem validateToolStripMenuItem;
        private System.Windows.Forms.TreeView treeView1;
    }
}


