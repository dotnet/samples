//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------
namespace Microsoft.Samples.WinFormsClient
{
    partial class ShowProposals
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ShowProposals));
            this.label1 = new System.Windows.Forms.Label();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.lblStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.label2 = new System.Windows.Forms.Label();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.tbbCreate = new System.Windows.Forms.ToolStripButton();
            this.tbbRefresh = new System.Windows.Forms.ToolStripButton();
            this.btnView = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.cboParticipant = new System.Windows.Forms.ToolStripComboBox();
            this.lstFinished = new System.Windows.Forms.ListView();
            this.lstActive = new System.Windows.Forms.ListView();
            this.statusStrip1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Cursor = System.Windows.Forms.Cursors.Default;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 43);
            this.label1.Name = "label1";
            this.label1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label1.Size = new System.Drawing.Size(71, 25);
            this.label1.TabIndex = 0;
            this.label1.Text = "Active";
            // 
            // statusStrip1
            // 
            this.statusStrip1.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.statusStrip1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblStatus});
            this.statusStrip1.Location = new System.Drawing.Point(0, 529);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.statusStrip1.Size = new System.Drawing.Size(1029, 22);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // lblStatus
            // 
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(97, 17);
            this.lblStatus.Text = "No RFP selected...";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Cursor = System.Windows.Forms.Cursors.Default;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(12, 291);
            this.label2.Name = "label2";
            this.label2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label2.Size = new System.Drawing.Size(94, 25);
            this.label2.TabIndex = 2;
            this.label2.Text = "Finished";
            // 
            // toolStrip1
            // 
            this.toolStrip1.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.toolStrip1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tbbCreate,
            this.tbbRefresh,
            this.btnView,
            this.toolStripSeparator1,
            this.toolStripLabel1,
            this.cboParticipant});
            this.toolStrip1.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.toolStrip1.Size = new System.Drawing.Size(1029, 25);
            this.toolStrip1.TabIndex = 3;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // tbbCreate
            // 
            this.tbbCreate.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tbbCreate.Image = ((System.Drawing.Image)(resources.GetObject("tbbCreate.Image")));
            this.tbbCreate.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbbCreate.Name = "tbbCreate";
            this.tbbCreate.Size = new System.Drawing.Size(66, 22);
            this.tbbCreate.Text = "Create RFP";
            this.tbbCreate.Click += new System.EventHandler(this.tbbCreate_Click);
            // 
            // tbbRefresh
            // 
            this.tbbRefresh.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tbbRefresh.Image = ((System.Drawing.Image)(resources.GetObject("tbbRefresh.Image")));
            this.tbbRefresh.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbbRefresh.Name = "tbbRefresh";
            this.tbbRefresh.Size = new System.Drawing.Size(49, 22);
            this.tbbRefresh.Text = "Refresh";
            this.tbbRefresh.Click += new System.EventHandler(this.tbbRefresh_Click);
            // 
            // btnView
            // 
            this.btnView.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnView.Image = ((System.Drawing.Image)(resources.GetObject("btnView.Image")));
            this.btnView.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnView.Name = "btnView";
            this.btnView.Size = new System.Drawing.Size(53, 22);
            this.btnView.Text = "View Rfp";
            this.btnView.Click += new System.EventHandler(this.btnView_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(61, 22);
            this.toolStripLabel1.Text = "Connect as";
            // 
            // cboParticipant
            // 
            this.cboParticipant.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboParticipant.Name = "cboParticipant";
            this.cboParticipant.Size = new System.Drawing.Size(200, 25);
            // 
            // lstFinished
            // 
            this.lstFinished.Cursor = System.Windows.Forms.Cursors.Default;
            this.lstFinished.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lstFinished.FullRowSelect = true;
            this.lstFinished.GridLines = true;
            this.lstFinished.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lstFinished.Location = new System.Drawing.Point(17, 333);
            this.lstFinished.Name = "lstFinished";
            this.lstFinished.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lstFinished.Size = new System.Drawing.Size(997, 171);
            this.lstFinished.TabIndex = 5;
            this.lstFinished.UseCompatibleStateImageBehavior = false;
            this.lstFinished.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.lstFinished_ItemSelectionChanged);
            this.lstFinished.DoubleClick += new System.EventHandler(this.lstFinished_DoubleClick);
            // 
            // lstActive
            // 
            this.lstActive.Cursor = System.Windows.Forms.Cursors.Default;
            this.lstActive.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lstActive.FullRowSelect = true;
            this.lstActive.GridLines = true;
            this.lstActive.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lstActive.Location = new System.Drawing.Point(17, 83);
            this.lstActive.Name = "lstActive";
            this.lstActive.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lstActive.Size = new System.Drawing.Size(997, 171);
            this.lstActive.TabIndex = 6;
            this.lstActive.UseCompatibleStateImageBehavior = false;
            this.lstActive.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.lstActive_ItemSelectionChanged);
            this.lstActive.DoubleClick += new System.EventHandler(this.lstActive_DoubleClick);
            // 
            // ShowProposals
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1029, 551);
            this.Controls.Add(this.lstActive);
            this.Controls.Add(this.lstFinished);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "ShowProposals";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "View All Requests For Proposals";
            this.Load += new System.EventHandler(this.ShowProposals_Load);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton tbbCreate;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripComboBox cboParticipant;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ListView lstFinished;
        private System.Windows.Forms.ToolStripButton tbbRefresh;
        private System.Windows.Forms.ListView lstActive;
        private System.Windows.Forms.ToolStripButton btnView;
        private System.Windows.Forms.ToolStripStatusLabel lblStatus;
    }
}