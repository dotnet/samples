<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Form1
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
      Me.ValueLabel = New System.Windows.Forms.Label()
      Me.Value = New System.Windows.Forms.TextBox()
      Me.NumberBox = New System.Windows.Forms.RadioButton()
      Me.DateBox = New System.Windows.Forms.RadioButton()
      Me.FormatLabel = New System.Windows.Forms.Label()
      Me.FormatStrings = New System.Windows.Forms.ComboBox()
      Me.Result = New System.Windows.Forms.TextBox()
      Me.ResultLabel = New System.Windows.Forms.Label()
      Me.OKButton = New System.Windows.Forms.Button()
      Me.StatusBar = New System.Windows.Forms.StatusStrip()
      Me.CulturesLabel = New System.Windows.Forms.Label()
      Me.CultureNames = New System.Windows.Forms.ComboBox()
      Me.SuspendLayout()
      '
      'ValueLabel
      '
      Me.ValueLabel.AutoSize = True
      Me.ValueLabel.Location = New System.Drawing.Point(13, 13)
      Me.ValueLabel.Name = "ValueLabel"
      Me.ValueLabel.Size = New System.Drawing.Size(73, 13)
      Me.ValueLabel.TabIndex = 0
      Me.ValueLabel.Text = "ValueTextBox"
      '
      'Value
      '
      Me.Value.Location = New System.Drawing.Point(33, 29)
      Me.Value.Name = "Value"
      Me.Value.Size = New System.Drawing.Size(186, 20)
      Me.Value.TabIndex = 1
      '
      'NumberBox
      '
      Me.NumberBox.AutoSize = True
      Me.NumberBox.Location = New System.Drawing.Point(286, 13)
      Me.NumberBox.Name = "NumberBox"
      Me.NumberBox.Size = New System.Drawing.Size(90, 17)
      Me.NumberBox.TabIndex = 2
      Me.NumberBox.TabStop = True
      Me.NumberBox.Text = "RadioButton1"
      Me.NumberBox.UseVisualStyleBackColor = True
      '
      'DateBox
      '
      Me.DateBox.AutoSize = True
      Me.DateBox.Location = New System.Drawing.Point(286, 37)
      Me.DateBox.Name = "DateBox"
      Me.DateBox.Size = New System.Drawing.Size(90, 17)
      Me.DateBox.TabIndex = 3
      Me.DateBox.TabStop = True
      Me.DateBox.Text = "RadioButton2"
      Me.DateBox.UseVisualStyleBackColor = True
      '
      'FormatLabel
      '
      Me.FormatLabel.AutoSize = True
      Me.FormatLabel.Location = New System.Drawing.Point(16, 69)
      Me.FormatLabel.Name = "FormatLabel"
      Me.FormatLabel.Size = New System.Drawing.Size(90, 13)
      Me.FormatLabel.TabIndex = 4
      Me.FormatLabel.Text = "FormatComboBox"
      '
      'FormatStrings
      '
      Me.FormatStrings.FormattingEnabled = True
      Me.FormatStrings.Location = New System.Drawing.Point(33, 85)
      Me.FormatStrings.Name = "FormatStrings"
      Me.FormatStrings.Size = New System.Drawing.Size(192, 21)
      Me.FormatStrings.TabIndex = 5
      '
      'Result
      '
      Me.Result.Location = New System.Drawing.Point(33, 188)
      Me.Result.Name = "Result"
      Me.Result.ReadOnly = True
      Me.Result.Size = New System.Drawing.Size(192, 20)
      Me.Result.TabIndex = 6
      '
      'ResultLabel
      '
      Me.ResultLabel.AutoSize = True
      Me.ResultLabel.Location = New System.Drawing.Point(19, 169)
      Me.ResultLabel.Name = "ResultLabel"
      Me.ResultLabel.Size = New System.Drawing.Size(76, 13)
      Me.ResultLabel.TabIndex = 7
      Me.ResultLabel.Text = "ResultTextBox"
      '
      'OKButton
      '
      Me.OKButton.Location = New System.Drawing.Point(286, 186)
      Me.OKButton.Name = "OKButton"
      Me.OKButton.Size = New System.Drawing.Size(75, 23)
      Me.OKButton.TabIndex = 8
      Me.OKButton.Text = "OK"
      Me.OKButton.UseVisualStyleBackColor = True
      '
      'StatusBar
      '
      Me.StatusBar.Location = New System.Drawing.Point(0, 240)
      Me.StatusBar.Name = "StatusBar"
      Me.StatusBar.Size = New System.Drawing.Size(446, 22)
      Me.StatusBar.TabIndex = 9
      Me.StatusBar.Text = "StatusStrip1"
      '
      'CulturesLabel
      '
      Me.CulturesLabel.AutoSize = True
      Me.CulturesLabel.Location = New System.Drawing.Point(16, 116)
      Me.CulturesLabel.Name = "CulturesLabel"
      Me.CulturesLabel.Size = New System.Drawing.Size(91, 13)
      Me.CulturesLabel.TabIndex = 4
      Me.CulturesLabel.Text = "CultureComboBox"
      '
      'CultureNames
      '
      Me.CultureNames.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
      Me.CultureNames.FormattingEnabled = True
      Me.CultureNames.Location = New System.Drawing.Point(33, 132)
      Me.CultureNames.Name = "CultureNames"
      Me.CultureNames.Size = New System.Drawing.Size(192, 21)
      Me.CultureNames.TabIndex = 5
      '
      'Form1
      '
      Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
      Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
      Me.ClientSize = New System.Drawing.Size(446, 262)
      Me.Controls.Add(Me.StatusBar)
      Me.Controls.Add(Me.OKButton)
      Me.Controls.Add(Me.ResultLabel)
      Me.Controls.Add(Me.Result)
      Me.Controls.Add(Me.CultureNames)
      Me.Controls.Add(Me.CulturesLabel)
      Me.Controls.Add(Me.FormatStrings)
      Me.Controls.Add(Me.FormatLabel)
      Me.Controls.Add(Me.DateBox)
      Me.Controls.Add(Me.NumberBox)
      Me.Controls.Add(Me.Value)
      Me.Controls.Add(Me.ValueLabel)
      Me.Name = "Form1"
      Me.Text = "WindowCaption"
      Me.ResumeLayout(False)
      Me.PerformLayout()

   End Sub
   Friend WithEvents ValueLabel As System.Windows.Forms.Label
   Friend WithEvents Value As System.Windows.Forms.TextBox
   Friend WithEvents NumberBox As System.Windows.Forms.RadioButton
   Friend WithEvents DateBox As System.Windows.Forms.RadioButton
   Friend WithEvents FormatLabel As System.Windows.Forms.Label
   Friend WithEvents FormatStrings As System.Windows.Forms.ComboBox
   Friend WithEvents Result As System.Windows.Forms.TextBox
   Friend WithEvents ResultLabel As System.Windows.Forms.Label
   Friend WithEvents OKButton As System.Windows.Forms.Button
   Friend WithEvents StatusBar As System.Windows.Forms.StatusStrip
   Friend WithEvents CulturesLabel As System.Windows.Forms.Label
   Friend WithEvents CultureNames As System.Windows.Forms.ComboBox

End Class
