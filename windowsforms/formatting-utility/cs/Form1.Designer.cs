namespace Formatter
{
   partial class Form1
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
         this.OKButton = new System.Windows.Forms.Button();
         this.ResultLabel = new System.Windows.Forms.Label();
         this.Result = new System.Windows.Forms.TextBox();
         this.FormatStrings = new System.Windows.Forms.ComboBox();
         this.FormatLabel = new System.Windows.Forms.Label();
         this.DateBox = new System.Windows.Forms.RadioButton();
         this.NumberBox = new System.Windows.Forms.RadioButton();
         this.Value = new System.Windows.Forms.TextBox();
         this.ValueLabel = new System.Windows.Forms.Label();
         this.StatusBar = new System.Windows.Forms.StatusStrip();
         this.CultureNames = new System.Windows.Forms.ComboBox();
         this.CulturesLabel = new System.Windows.Forms.Label();
         this.SuspendLayout();
         // 
         // OKButton
         // 
         this.OKButton.Location = new System.Drawing.Point(281, 195);
         this.OKButton.Name = "OKButton";
         this.OKButton.Size = new System.Drawing.Size(75, 23);
         this.OKButton.TabIndex = 17;
         this.OKButton.Text = "OK";
         this.OKButton.UseVisualStyleBackColor = true;
         this.OKButton.Click += new System.EventHandler(this.OKButton_Click);
         // 
         // ResultLabel
         // 
         this.ResultLabel.AutoSize = true;
         this.ResultLabel.Location = new System.Drawing.Point(14, 179);
         this.ResultLabel.Name = "ResultLabel";
         this.ResultLabel.Size = new System.Drawing.Size(76, 13);
         this.ResultLabel.TabIndex = 16;
         this.ResultLabel.Text = "ResultTextBox";
         // 
         // Result
         // 
         this.Result.Location = new System.Drawing.Point(28, 198);
         this.Result.Name = "Result";
         this.Result.ReadOnly = true;
         this.Result.Size = new System.Drawing.Size(192, 20);
         this.Result.TabIndex = 15;
         // 
         // FormatStrings
         // 
         this.FormatStrings.FormattingEnabled = true;
         this.FormatStrings.Location = new System.Drawing.Point(28, 90);
         this.FormatStrings.Name = "FormatStrings";
         this.FormatStrings.Size = new System.Drawing.Size(192, 21);
         this.FormatStrings.TabIndex = 14;
         this.FormatStrings.SelectedIndexChanged += new System.EventHandler(this.FormatStrings_SelectedIndexChanged);
         // 
         // FormatLabel
         // 
         this.FormatLabel.AutoSize = true;
         this.FormatLabel.Location = new System.Drawing.Point(11, 74);
         this.FormatLabel.Name = "FormatLabel";
         this.FormatLabel.Size = new System.Drawing.Size(90, 13);
         this.FormatLabel.TabIndex = 13;
         this.FormatLabel.Text = "FormatComboBox";
         // 
         // DateBox
         // 
         this.DateBox.AutoSize = true;
         this.DateBox.Location = new System.Drawing.Point(281, 42);
         this.DateBox.Name = "DateBox";
         this.DateBox.Size = new System.Drawing.Size(90, 17);
         this.DateBox.TabIndex = 12;
         this.DateBox.TabStop = true;
         this.DateBox.Text = "RadioButton2";
         this.DateBox.UseVisualStyleBackColor = true;
         this.DateBox.CheckedChanged += new System.EventHandler(this.DateBox_CheckedChanged);
         // 
         // NumberBox
         // 
         this.NumberBox.AutoSize = true;
         this.NumberBox.Location = new System.Drawing.Point(281, 18);
         this.NumberBox.Name = "NumberBox";
         this.NumberBox.Size = new System.Drawing.Size(90, 17);
         this.NumberBox.TabIndex = 11;
         this.NumberBox.TabStop = true;
         this.NumberBox.Text = "RadioButton1";
         this.NumberBox.UseVisualStyleBackColor = true;
         this.NumberBox.CheckedChanged += new System.EventHandler(this.NumberBox_CheckedChanged);
         // 
         // Value
         // 
         this.Value.Location = new System.Drawing.Point(28, 34);
         this.Value.Name = "Value";
         this.Value.Size = new System.Drawing.Size(186, 20);
         this.Value.TabIndex = 10;
         this.Value.TextChanged += new System.EventHandler(this.Value_TextChanged);
         // 
         // ValueLabel
         // 
         this.ValueLabel.AutoSize = true;
         this.ValueLabel.Location = new System.Drawing.Point(8, 18);
         this.ValueLabel.Name = "ValueLabel";
         this.ValueLabel.Size = new System.Drawing.Size(73, 13);
         this.ValueLabel.TabIndex = 9;
         this.ValueLabel.Text = "ValueTextBox";
         // 
         // StatusBar
         // 
         this.StatusBar.Location = new System.Drawing.Point(0, 240);
         this.StatusBar.Name = "StatusBar";
         this.StatusBar.Size = new System.Drawing.Size(430, 22);
         this.StatusBar.TabIndex = 18;
         this.StatusBar.Text = "StatusStrip1";
         // 
         // CultureNames
         // 
         this.CultureNames.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
         this.CultureNames.FormattingEnabled = true;
         this.CultureNames.Location = new System.Drawing.Point(28, 140);
         this.CultureNames.Name = "CultureNames";
         this.CultureNames.Size = new System.Drawing.Size(192, 21);
         this.CultureNames.TabIndex = 20;
         this.CultureNames.SelectedIndexChanged += new System.EventHandler(this.CultureNames_SelectedIndexChanged);
         // 
         // CulturesLabel
         // 
         this.CulturesLabel.AutoSize = true;
         this.CulturesLabel.Location = new System.Drawing.Point(11, 124);
         this.CulturesLabel.Name = "CulturesLabel";
         this.CulturesLabel.Size = new System.Drawing.Size(91, 13);
         this.CulturesLabel.TabIndex = 19;
         this.CulturesLabel.Text = "CultureComboBox";
         // 
         // Form1
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.ClientSize = new System.Drawing.Size(430, 262);
         this.Controls.Add(this.CultureNames);
         this.Controls.Add(this.CulturesLabel);
         this.Controls.Add(this.StatusBar);
         this.Controls.Add(this.OKButton);
         this.Controls.Add(this.ResultLabel);
         this.Controls.Add(this.Result);
         this.Controls.Add(this.FormatStrings);
         this.Controls.Add(this.FormatLabel);
         this.Controls.Add(this.DateBox);
         this.Controls.Add(this.NumberBox);
         this.Controls.Add(this.Value);
         this.Controls.Add(this.ValueLabel);
         this.Name = "Form1";
         this.Text = "Form1";
         this.Load += new System.EventHandler(this.Form1_Load);
         this.ResumeLayout(false);
         this.PerformLayout();

      }

      #endregion

      internal System.Windows.Forms.Button OKButton;
      internal System.Windows.Forms.Label ResultLabel;
      internal System.Windows.Forms.TextBox Result;
      internal System.Windows.Forms.ComboBox FormatStrings;
      internal System.Windows.Forms.Label FormatLabel;
      internal System.Windows.Forms.RadioButton DateBox;
      internal System.Windows.Forms.RadioButton NumberBox;
      internal System.Windows.Forms.TextBox Value;
      internal System.Windows.Forms.Label ValueLabel;
      internal System.Windows.Forms.StatusStrip StatusBar;
      internal System.Windows.Forms.ComboBox CultureNames;
      internal System.Windows.Forms.Label CulturesLabel;
   }
}

