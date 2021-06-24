
' Copyright (c) Microsoft Corporation.  All rights reserved.

Namespace Microsoft.Samples.Federation

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class BookStoreClientForm
        Inherits System.Windows.Forms.Form

        'Form overrides dispose to clean up the component list.
        <System.Diagnostics.DebuggerNonUserCode()> _
        Protected Overrides Sub Dispose(ByVal disposing As Boolean)
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
            MyBase.Dispose(disposing)
        End Sub

        'Required by the Windows Form Designer
        Private components As System.ComponentModel.IContainer

        'NOTE: The following procedure is required by the Windows Form Designer
        'It can be modified using the Windows Form Designer.  
        'Do not modify it using the code editor.
        <System.Diagnostics.DebuggerStepThrough()> _
        Private Sub InitializeComponent()
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(BookStoreClientForm))
            Me.pictureBox1 = New System.Windows.Forms.PictureBox
            Me.lstBooks = New System.Windows.Forms.ListBox
            Me.btnBuy = New System.Windows.Forms.Button
            Me.btnBrowse = New System.Windows.Forms.Button
            CType(Me.pictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.SuspendLayout()
            '
            'pictureBox1
            '
            Me.pictureBox1.Image = CType(resources.GetObject("pictureBox1.Image"), System.Drawing.Image)
            Me.pictureBox1.Location = New System.Drawing.Point(154, 12)
            Me.pictureBox1.Name = "pictureBox1"
            Me.pictureBox1.Size = New System.Drawing.Size(252, 173)
            Me.pictureBox1.TabIndex = 3
            Me.pictureBox1.TabStop = False
            '
            'lstBooks
            '
            Me.lstBooks.FormattingEnabled = True
            Me.lstBooks.Location = New System.Drawing.Point(27, 249)
            Me.lstBooks.Name = "lstBooks"
            Me.lstBooks.Size = New System.Drawing.Size(506, 186)
            Me.lstBooks.TabIndex = 6
            '
            'btnBuy
            '
            Me.btnBuy.Enabled = False
            Me.btnBuy.Location = New System.Drawing.Point(319, 200)
            Me.btnBuy.Name = "btnBuy"
            Me.btnBuy.Size = New System.Drawing.Size(87, 37)
            Me.btnBuy.TabIndex = 5
            Me.btnBuy.Text = "Buy Book"
            '
            'btnBrowse
            '
            Me.btnBrowse.Location = New System.Drawing.Point(154, 200)
            Me.btnBrowse.Name = "btnBrowse"
            Me.btnBrowse.Size = New System.Drawing.Size(87, 37)
            Me.btnBrowse.TabIndex = 4
            Me.btnBrowse.Text = "Browse Books"
            '
            'Form1
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.ClientSize = New System.Drawing.Size(569, 468)
            Me.Controls.Add(Me.lstBooks)
            Me.Controls.Add(Me.btnBuy)
            Me.Controls.Add(Me.btnBrowse)
            Me.Controls.Add(Me.pictureBox1)
            Me.Name = "Form1"
            Me.Text = "BookStoreClient"
            CType(Me.pictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
            Me.ResumeLayout(False)

        End Sub
        Private WithEvents pictureBox1 As System.Windows.Forms.PictureBox
        Private WithEvents lstBooks As System.Windows.Forms.ListBox
        Private WithEvents btnBuy As System.Windows.Forms.Button
        Private WithEvents btnBrowse As System.Windows.Forms.Button
    End Class

End Namespace
