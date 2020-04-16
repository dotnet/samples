Option Explicit Off
Option Infer On
Option Strict Off

Namespace MatchingGame
    Partial Class Form1
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
            Me.components = New System.ComponentModel.Container()
            Me.tableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
            Me.label16 = New System.Windows.Forms.Label()
            Me.label15 = New System.Windows.Forms.Label()
            Me.label14 = New System.Windows.Forms.Label()
            Me.label13 = New System.Windows.Forms.Label()
            Me.label12 = New System.Windows.Forms.Label()
            Me.label11 = New System.Windows.Forms.Label()
            Me.label10 = New System.Windows.Forms.Label()
            Me.label9 = New System.Windows.Forms.Label()
            Me.label8 = New System.Windows.Forms.Label()
            Me.label7 = New System.Windows.Forms.Label()
            Me.label6 = New System.Windows.Forms.Label()
            Me.label5 = New System.Windows.Forms.Label()
            Me.label4 = New System.Windows.Forms.Label()
            Me.label3 = New System.Windows.Forms.Label()
            Me.label2 = New System.Windows.Forms.Label()
            Me.label1 = New System.Windows.Forms.Label()
            Me.timer1 = New System.Windows.Forms.Timer(Me.components)
            Me.tableLayoutPanel1.SuspendLayout()
            Me.SuspendLayout()
            '
            ' tableLayoutPanel1
            '
            Me.tableLayoutPanel1.BackColor = System.Drawing.Color.CornflowerBlue
            Me.tableLayoutPanel1.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Inset
            Me.tableLayoutPanel1.ColumnCount = 4
            Me.tableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F))
            Me.tableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F))
            Me.tableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F))
            Me.tableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F))
            Me.tableLayoutPanel1.Controls.Add(Me.label16, 3, 3)
            Me.tableLayoutPanel1.Controls.Add(Me.label15, 2, 3)
            Me.tableLayoutPanel1.Controls.Add(Me.label14, 1, 3)
            Me.tableLayoutPanel1.Controls.Add(Me.label13, 0, 3)
            Me.tableLayoutPanel1.Controls.Add(Me.label12, 3, 2)
            Me.tableLayoutPanel1.Controls.Add(Me.label11, 2, 2)
            Me.tableLayoutPanel1.Controls.Add(Me.label10, 1, 2)
            Me.tableLayoutPanel1.Controls.Add(Me.label9, 0, 2)
            Me.tableLayoutPanel1.Controls.Add(Me.label8, 3, 1)
            Me.tableLayoutPanel1.Controls.Add(Me.label7, 2, 1)
            Me.tableLayoutPanel1.Controls.Add(Me.label6, 1, 1)
            Me.tableLayoutPanel1.Controls.Add(Me.label5, 0, 1)
            Me.tableLayoutPanel1.Controls.Add(Me.label4, 3, 0)
            Me.tableLayoutPanel1.Controls.Add(Me.label3, 2, 0)
            Me.tableLayoutPanel1.Controls.Add(Me.label2, 1, 0)
            Me.tableLayoutPanel1.Controls.Add(Me.label1, 0, 0)
            Me.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill
            Me.tableLayoutPanel1.Location = New System.Drawing.Point(0, 0)
            Me.tableLayoutPanel1.Name = "tableLayoutPanel1"
            Me.tableLayoutPanel1.RowCount = 4
            Me.tableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F))
            Me.tableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F))
            Me.tableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F))
            Me.tableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F))
            Me.tableLayoutPanel1.Size = New System.Drawing.Size(534, 511)
            Me.tableLayoutPanel1.TabIndex = 0
            '
            ' label16
            '
            Me.label16.Dock = System.Windows.Forms.DockStyle.Fill
            Me.label16.Font = New System.Drawing.Font("Webdings", 72F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, (CByte((2))))
            Me.label16.Location = New System.Drawing.Point(404, 383)
            Me.label16.Name = "label16"
            Me.label16.Size = New System.Drawing.Size(125, 126)
            Me.label16.TabIndex = 15
            Me.label16.Text = "c"
            Me.label16.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
            AddHandler Me.label16.Click, New System.EventHandler(AddressOf Me.label_Click)
            '
            ' label15
            '
            Me.label15.Dock = System.Windows.Forms.DockStyle.Fill
            Me.label15.Font = New System.Drawing.Font("Webdings", 72F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, (CByte((2))))
            Me.label15.Location = New System.Drawing.Point(271, 383)
            Me.label15.Name = "label15"
            Me.label15.Size = New System.Drawing.Size(125, 126)
            Me.label15.TabIndex = 14
            Me.label15.Text = "c"
            Me.label15.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
            AddHandler Me.label15.Click, New System.EventHandler(AddressOf Me.label_Click)
            '
            ' label14
            '
            Me.label14.Dock = System.Windows.Forms.DockStyle.Fill
            Me.label14.Font = New System.Drawing.Font("Webdings", 72F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, (CByte((2))))
            Me.label14.Location = New System.Drawing.Point(138, 383)
            Me.label14.Name = "label14"
            Me.label14.Size = New System.Drawing.Size(125, 126)
            Me.label14.TabIndex = 13
            Me.label14.Text = "c"
            Me.label14.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
            AddHandler Me.label14.Click, New System.EventHandler(AddressOf Me.label_Click)
            '
            ' label13
            '
            Me.label13.Dock = System.Windows.Forms.DockStyle.Fill
            Me.label13.Font = New System.Drawing.Font("Webdings", 72F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, (CByte((2))))
            Me.label13.Location = New System.Drawing.Point(5, 383)
            Me.label13.Name = "label13"
            Me.label13.Size = New System.Drawing.Size(125, 126)
            Me.label13.TabIndex = 12
            Me.label13.Text = "c"
            Me.label13.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
            AddHandler Me.label13.Click, New System.EventHandler(AddressOf Me.label_Click)
            '
            ' label12
            '
            Me.label12.Dock = System.Windows.Forms.DockStyle.Fill
            Me.label12.Font = New System.Drawing.Font("Webdings", 72F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, (CByte((2))))
            Me.label12.Location = New System.Drawing.Point(404, 256)
            Me.label12.Name = "label12"
            Me.label12.Size = New System.Drawing.Size(125, 125)
            Me.label12.TabIndex = 11
            Me.label12.Text = "c"
            Me.label12.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
            AddHandler Me.label12.Click, New System.EventHandler(AddressOf Me.label_Click)
            '
            ' label11
            '
            Me.label11.Dock = System.Windows.Forms.DockStyle.Fill
            Me.label11.Font = New System.Drawing.Font("Webdings", 72F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, (CByte((2))))
            Me.label11.Location = New System.Drawing.Point(271, 256)
            Me.label11.Name = "label11"
            Me.label11.Size = New System.Drawing.Size(125, 125)
            Me.label11.TabIndex = 10
            Me.label11.Text = "c"
            Me.label11.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
            AddHandler Me.label11.Click, New System.EventHandler(AddressOf Me.label_Click)
            '
            ' label10
            '
            Me.label10.Dock = System.Windows.Forms.DockStyle.Fill
            Me.label10.Font = New System.Drawing.Font("Webdings", 72F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, (CByte((2))))
            Me.label10.Location = New System.Drawing.Point(138, 256)
            Me.label10.Name = "label10"
            Me.label10.Size = New System.Drawing.Size(125, 125)
            Me.label10.TabIndex = 9
            Me.label10.Text = "c"
            Me.label10.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
            AddHandler Me.label10.Click, New System.EventHandler(AddressOf Me.label_Click)
            '
            ' label9
            '
            Me.label9.Dock = System.Windows.Forms.DockStyle.Fill
            Me.label9.Font = New System.Drawing.Font("Webdings", 72F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, (CByte((2))))
            Me.label9.Location = New System.Drawing.Point(5, 256)
            Me.label9.Name = "label9"
            Me.label9.Size = New System.Drawing.Size(125, 125)
            Me.label9.TabIndex = 8
            Me.label9.Text = "c"
            Me.label9.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
            AddHandler Me.label9.Click, New System.EventHandler(AddressOf Me.label_Click)
            '
            ' label8
            '
            Me.label8.Dock = System.Windows.Forms.DockStyle.Fill
            Me.label8.Font = New System.Drawing.Font("Webdings", 72F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, (CByte((2))))
            Me.label8.Location = New System.Drawing.Point(404, 129)
            Me.label8.Name = "label8"
            Me.label8.Size = New System.Drawing.Size(125, 125)
            Me.label8.TabIndex = 7
            Me.label8.Text = "c"
            Me.label8.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
            AddHandler Me.label8.Click, New System.EventHandler(AddressOf Me.label_Click)
            '
            ' label7
            '
            Me.label7.Dock = System.Windows.Forms.DockStyle.Fill
            Me.label7.Font = New System.Drawing.Font("Webdings", 72F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, (CByte((2))))
            Me.label7.Location = New System.Drawing.Point(271, 129)
            Me.label7.Name = "label7"
            Me.label7.Size = New System.Drawing.Size(125, 125)
            Me.label7.TabIndex = 6
            Me.label7.Text = "c"
            Me.label7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
            AddHandler Me.label7.Click, New System.EventHandler(AddressOf Me.label_Click)
            '
            ' label6
            '
            Me.label6.Dock = System.Windows.Forms.DockStyle.Fill
            Me.label6.Font = New System.Drawing.Font("Webdings", 72F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, (CByte((2))))
            Me.label6.Location = New System.Drawing.Point(138, 129)
            Me.label6.Name = "label6"
            Me.label6.Size = New System.Drawing.Size(125, 125)
            Me.label6.TabIndex = 5
            Me.label6.Text = "c"
            Me.label6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
            AddHandler Me.label6.Click, New System.EventHandler(AddressOf Me.label_Click)
            '
            ' label5
            '
            Me.label5.Dock = System.Windows.Forms.DockStyle.Fill
            Me.label5.Font = New System.Drawing.Font("Webdings", 72F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, (CByte((2))))
            Me.label5.Location = New System.Drawing.Point(5, 129)
            Me.label5.Name = "label5"
            Me.label5.Size = New System.Drawing.Size(125, 125)
            Me.label5.TabIndex = 4
            Me.label5.Text = "c"
            Me.label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
            AddHandler Me.label5.Click, New System.EventHandler(AddressOf Me.label_Click)
            '
            ' label4
            '
            Me.label4.Dock = System.Windows.Forms.DockStyle.Fill
            Me.label4.Font = New System.Drawing.Font("Webdings", 72F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, (CByte((2))))
            Me.label4.Location = New System.Drawing.Point(404, 2)
            Me.label4.Name = "label4"
            Me.label4.Size = New System.Drawing.Size(125, 125)
            Me.label4.TabIndex = 3
            Me.label4.Text = "c"
            Me.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
            AddHandler Me.label4.Click, New System.EventHandler(AddressOf Me.label_Click)
            '
            ' label3
            '
            Me.label3.Dock = System.Windows.Forms.DockStyle.Fill
            Me.label3.Font = New System.Drawing.Font("Webdings", 72F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, (CByte((2))))
            Me.label3.Location = New System.Drawing.Point(271, 2)
            Me.label3.Name = "label3"
            Me.label3.Size = New System.Drawing.Size(125, 125)
            Me.label3.TabIndex = 2
            Me.label3.Text = "c"
            Me.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
            AddHandler Me.label3.Click, New System.EventHandler(AddressOf Me.label_Click)
            '
            ' label2
            '
            Me.label2.Dock = System.Windows.Forms.DockStyle.Fill
            Me.label2.Font = New System.Drawing.Font("Webdings", 72F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, (CByte((2))))
            Me.label2.Location = New System.Drawing.Point(138, 2)
            Me.label2.Name = "label2"
            Me.label2.Size = New System.Drawing.Size(125, 125)
            Me.label2.TabIndex = 1
            Me.label2.Text = "c"
            Me.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
            AddHandler Me.label2.Click, New System.EventHandler(AddressOf Me.label_Click)
            '
            ' label1
            '
            Me.label1.Dock = System.Windows.Forms.DockStyle.Fill
            Me.label1.Font = New System.Drawing.Font("Webdings", 72F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, (CByte((2))))
            Me.label1.Location = New System.Drawing.Point(5, 2)
            Me.label1.Name = "label1"
            Me.label1.Size = New System.Drawing.Size(125, 125)
            Me.label1.TabIndex = 0
            Me.label1.Text = "c"
            Me.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
            AddHandler Me.label1.Click, New System.EventHandler(AddressOf Me.label_Click)
            '
            ' timer1
            '
            Me.timer1.Interval = 750
            AddHandler Me.timer1.Tick, New System.EventHandler(AddressOf Me.timer1_Tick)
            '
            ' Form1
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6F, 13F)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.ClientSize = New System.Drawing.Size(534, 511)
            Me.Controls.Add(Me.tableLayoutPanel1)
            Me.Name = "Form1"
            Me.Text = "Matching Game"
            Me.tableLayoutPanel1.ResumeLayout(False)
            Me.ResumeLayout(False)

        End Sub

#End Region

        Private tableLayoutPanel1 As System.Windows.Forms.TableLayoutPanel
        Private label1 As System.Windows.Forms.Label
        Private label16 As System.Windows.Forms.Label
        Private label15 As System.Windows.Forms.Label
        Private label14 As System.Windows.Forms.Label
        Private label13 As System.Windows.Forms.Label
        Private label12 As System.Windows.Forms.Label
        Private label11 As System.Windows.Forms.Label
        Private label10 As System.Windows.Forms.Label
        Private label9 As System.Windows.Forms.Label
        Private label8 As System.Windows.Forms.Label
        Private label7 As System.Windows.Forms.Label
        Private label6 As System.Windows.Forms.Label
        Private label5 As System.Windows.Forms.Label
        Private label4 As System.Windows.Forms.Label
        Private label3 As System.Windows.Forms.Label
        Private label2 As System.Windows.Forms.Label
        Private timer1 As System.Windows.Forms.Timer
    End Class
End Namespace
