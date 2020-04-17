namespace Raytracer
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this._isParallelCheckBox = new System.Windows.Forms.CheckBox();
            this._startButton = new System.Windows.Forms.Button();
            this._renderedImage = new System.Windows.Forms.PictureBox();
            this._showThreadsCheckBox = new System.Windows.Forms.CheckBox();
            this._numberOfProcsTrackBar = new System.Windows.Forms.TrackBar();
            this._numberOfProcsLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this._renderedImage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._numberOfProcsTrackBar)).BeginInit();
            this.SuspendLayout();
            // 
            // chkParallel
            // 
            this._isParallelCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._isParallelCheckBox.AutoSize = true;
            this._isParallelCheckBox.Location = new System.Drawing.Point(108, 426);
            this._isParallelCheckBox.Name = "chkParallel";
            this._isParallelCheckBox.Size = new System.Drawing.Size(60, 17);
            this._isParallelCheckBox.TabIndex = 20;
            this._isParallelCheckBox.Text = "Parallel";
            this._isParallelCheckBox.UseVisualStyleBackColor = true;
            this._isParallelCheckBox.CheckedChanged += new System.EventHandler(this.OnIsParallelCheckBoxChanged);
            // 
            // btnStartStop
            // 
            this._startButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._startButton.Location = new System.Drawing.Point(13, 421);
            this._startButton.Name = "btnStartStop";
            this._startButton.Size = new System.Drawing.Size(88, 23);
            this._startButton.TabIndex = 19;
            this._startButton.Text = "Start";
            this._startButton.UseVisualStyleBackColor = true;
            this._startButton.Click += new System.EventHandler(this.OnStartButtonClick);
            // 
            // pbRenderedImage
            // 
            this._renderedImage.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._renderedImage.BackColor = System.Drawing.Color.Black;
            this._renderedImage.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this._renderedImage.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this._renderedImage.Location = new System.Drawing.Point(13, 15);
            this._renderedImage.Name = "pbRenderedImage";
            this._renderedImage.Size = new System.Drawing.Size(469, 400);
            this._renderedImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this._renderedImage.TabIndex = 18;
            this._renderedImage.TabStop = false;
            // 
            // chkShowThreads
            // 
            this._showThreadsCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._showThreadsCheckBox.AutoSize = true;
            this._showThreadsCheckBox.Enabled = false;
            this._showThreadsCheckBox.Location = new System.Drawing.Point(174, 426);
            this._showThreadsCheckBox.Name = "chkShowThreads";
            this._showThreadsCheckBox.Size = new System.Drawing.Size(95, 17);
            this._showThreadsCheckBox.TabIndex = 21;
            this._showThreadsCheckBox.Text = "Show Threads";
            this._showThreadsCheckBox.UseVisualStyleBackColor = true;
            // 
            // tbNumProcs
            // 
            this._numberOfProcsTrackBar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._numberOfProcsTrackBar.Enabled = false;
            this._numberOfProcsTrackBar.Location = new System.Drawing.Point(304, 421);
            this._numberOfProcsTrackBar.Maximum = 24;
            this._numberOfProcsTrackBar.Minimum = 1;
            this._numberOfProcsTrackBar.Name = "tbNumProcs";
            this._numberOfProcsTrackBar.Size = new System.Drawing.Size(178, 45);
            this._numberOfProcsTrackBar.TabIndex = 22;
            this._numberOfProcsTrackBar.Value = 1;
            this._numberOfProcsTrackBar.ValueChanged += new System.EventHandler(this.OnNumberOfProcsTrackBarChanged);
            // 
            // lblNumProcs
            // 
            this._numberOfProcsLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._numberOfProcsLabel.AutoSize = true;
            this._numberOfProcsLabel.Enabled = false;
            this._numberOfProcsLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._numberOfProcsLabel.Location = new System.Drawing.Point(295, 431);
            this._numberOfProcsLabel.Name = "lblNumProcs";
            this._numberOfProcsLabel.Size = new System.Drawing.Size(14, 13);
            this._numberOfProcsLabel.TabIndex = 23;
            this._numberOfProcsLabel.Text = "1";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(495, 459);
            this.Controls.Add(this._numberOfProcsLabel);
            this.Controls.Add(this._numberOfProcsTrackBar);
            this.Controls.Add(this._showThreadsCheckBox);
            this.Controls.Add(this._isParallelCheckBox);
            this.Controls.Add(this._startButton);
            this.Controls.Add(this._renderedImage);
            this.Name = "MainForm";
            this.Text = "Ray Tracer";
            this.Load += new System.EventHandler(this.OnMainFormLoaded);
            ((System.ComponentModel.ISupportInitialize)(this._renderedImage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._numberOfProcsTrackBar)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        internal System.Windows.Forms.CheckBox _isParallelCheckBox;
        private System.Windows.Forms.Button _startButton;
        private System.Windows.Forms.PictureBox _renderedImage;
        internal System.Windows.Forms.CheckBox _showThreadsCheckBox;
        private System.Windows.Forms.TrackBar _numberOfProcsTrackBar;
        private System.Windows.Forms.Label _numberOfProcsLabel;
    }
}

