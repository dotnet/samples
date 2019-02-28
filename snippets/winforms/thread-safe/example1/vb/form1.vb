Imports System.Drawing
Imports System.Threading
Imports System.Windows.Forms

Public Class InvokeThreadSafeForm : Inherits Form

    Public Shared Sub Main()
        Application.SetCompatibleTextRenderingDefault(False)
        Application.EnableVisualStyles()
        Dim frm As New InvokeThreadSafeForm()
        Application.Run(frm)
    End Sub

    Dim WithEvents Button1 As Button
    Dim TextBox1 As TextBox
    Dim Thread2 as Thread = Nothing

    Delegate Sub SafeCallDelegate(text As String)

    Private Sub New()
        Button1 = New Button()
        With Button1
            .Text = "Set text safely"
            .Location = New Point(15, 55)
        End With
        TextBox1 = New TextBox()
        With TextBox1
            .Location = New Point(15, 15)
            .Size = New Size(240, 20)
        End With
        Controls.Add(Button1)
        Controls.Add(TextBox1)
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        thread2 = New Thread(New ThreadStart(AddressOf SetText))
        thread2.Start()
        Thread.Sleep(1000)
    End Sub

    Private Sub SafeText(text As String)
        If TextBox1.InvokeRequired Then
            Dim d As New SafeCallDelegate(AddressOf SetText)
            Invoke(d, New Object() {text})
        Else
            TextBox1.Text = text
        End If
    End Sub

    Private Sub SetText()
        SafeText("This text was set safely.")
    End Sub
End Class
