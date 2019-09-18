Option Explicit Off
Option Infer On
Option Strict Off

Namespace WinFormGraphics
    Partial Class MainForm
        ''' <summary>
        ''' Required designer variable.
        ''' </summary>
        Private components As System.ComponentModel.IContainer = Nothing

        ''' <summary>
        ''' Clean up any resources being used.
        ''' </summary>
        ''' <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        Protected Overrides Sub Dispose(disposing As Boolean)
            If disposing AndAlso (components IsNot Nothing) Then
                components.Dispose()
            End If
            MyBase.Dispose(disposing)
        End Sub

#Region "Windows Form Designer generated code"

        ''' <summary>
        ''' Required method for Designer support - do not modify
        ''' the contents of this method with the code editor.
        ''' </summary>
        Private Sub InitializeComponent()
            Me.groupBox1 = New System.Windows.Forms.GroupBox()
            Me.label8 = New System.Windows.Forms.Label()
            Me.label7 = New System.Windows.Forms.Label()
            Me.label6 = New System.Windows.Forms.Label()
            Me.label5 = New System.Windows.Forms.Label()
            Me.label4 = New System.Windows.Forms.Label()
            Me.label3 = New System.Windows.Forms.Label()
            Me.label2 = New System.Windows.Forms.Label()
            Me.label1 = New System.Windows.Forms.Label()
            Me.groupBox1.SuspendLayout()
            Me.SuspendLayout()
            '
            ' groupBox1
            '
            Me.groupBox1.Controls.Add(Me.label8)
            Me.groupBox1.Controls.Add(Me.label7)
            Me.groupBox1.Controls.Add(Me.label6)
            Me.groupBox1.Controls.Add(Me.label5)
            Me.groupBox1.Controls.Add(Me.label4)
            Me.groupBox1.Controls.Add(Me.label3)
            Me.groupBox1.Controls.Add(Me.label2)
            Me.groupBox1.Controls.Add(Me.label1)
            Me.groupBox1.Font = New System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, (CByte((0))))
            Me.groupBox1.Location = New System.Drawing.Point(18, 12)
            Me.groupBox1.Name = "groupBox1"
            Me.groupBox1.Size = New System.Drawing.Size(700, 466)
            Me.groupBox1.TabIndex = 9
            Me.groupBox1.TabStop = False
            Me.groupBox1.Text = "Graphics"
            AddHandler Me.groupBox1.Paint, New System.Windows.Forms.PaintEventHandler(AddressOf Me.groupBox1_Paint)
            '
            ' label8
            '
            Me.label8.AutoSize = True
            Me.label8.Location = New System.Drawing.Point(197, 312)
            Me.label8.Name = "label8"
            Me.label8.Size = New System.Drawing.Size(64, 16)
            Me.label8.TabIndex = 7
            Me.label8.Text = "AntiAlias"
            '
            ' label7
            '
            Me.label7.AutoSize = True
            Me.label7.Location = New System.Drawing.Point(62, 312)
            Me.label7.Name = "label7"
            Me.label7.Size = New System.Drawing.Size(41, 16)
            Me.label7.TabIndex = 6
            Me.label7.Text = "None"
            '
            ' label6
            '
            Me.label6.AutoSize = True
            Me.label6.Location = New System.Drawing.Point(31, 168)
            Me.label6.Name = "label6"
            Me.label6.Size = New System.Drawing.Size(119, 16)
            Me.label6.TabIndex = 5
            Me.label6.Text = "2. Draw a curve."
            '
            ' label5
            '
            Me.label5.AutoSize = True
            Me.label5.Location = New System.Drawing.Point(31, 354)
            Me.label5.Name = "label5"
            Me.label5.Size = New System.Drawing.Size(127, 16)
            Me.label5.TabIndex = 4
            Me.label5.Text = "3. Draw an arrow."
            '
            ' label4
            '
            Me.label4.AutoSize = True
            Me.label4.Location = New System.Drawing.Point(338, 226)
            Me.label4.Name = "label4"
            Me.label4.Size = New System.Drawing.Size(263, 16)
            Me.label4.TabIndex = 3
            Me.label4.Text = "5. Draw an ellipse with gradient brush."
            '
            ' label3
            '
            Me.label3.AutoSize = True
            Me.label3.Location = New System.Drawing.Point(338, 63)
            Me.label3.Name = "label3"
            Me.label3.Size = New System.Drawing.Size(173, 16)
            Me.label3.TabIndex = 2
            Me.label3.Text = "4. Draw a vertical string."
            '
            ' label2
            '
            Me.label2.AutoSize = True
            Me.label2.Location = New System.Drawing.Point(31, 63)
            Me.label2.Name = "label2"
            Me.label2.Size = New System.Drawing.Size(104, 16)
            Me.label2.TabIndex = 1
            Me.label2.Text = "1. Draw a line."
            '
            ' label1
            '
            Me.label1.Font = New System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, (CByte((0))))
            Me.label1.Location = New System.Drawing.Point(31, 31)
            Me.label1.Name = "label1"
            Me.label1.Size = New System.Drawing.Size(648, 22)
            Me.label1.TabIndex = 0
            Me.label1.Text = "This example demonstrates basic drawings in Windows Forms application."
            '
            ' MainForm
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6F, 13F)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.ClientSize = New System.Drawing.Size(730, 490)
            Me.Controls.Add(Me.groupBox1)
            Me.Name = "MainForm"
            Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
            Me.Text = "VBWinFormGraphics"
            Me.groupBox1.ResumeLayout(False)
            Me.groupBox1.PerformLayout()
            Me.ResumeLayout(False)

        End Sub

#End Region

        Private groupBox1 As System.Windows.Forms.GroupBox
        Private label4 As System.Windows.Forms.Label
        Private label3 As System.Windows.Forms.Label
        Private label2 As System.Windows.Forms.Label
        Private label1 As System.Windows.Forms.Label
        Private label5 As System.Windows.Forms.Label
        Private label8 As System.Windows.Forms.Label
        Private label7 As System.Windows.Forms.Label
        Private label6 As System.Windows.Forms.Label
    End Class
End Namespace
