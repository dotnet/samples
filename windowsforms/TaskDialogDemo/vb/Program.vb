Option Strict On

Imports System.Windows.Forms

Module Program
    ''' <summary>
    ''' The main entry point for the application.
    ''' </summary>
    <STAThread>
    Sub Main()
        Application.SetHighDpiMode(HighDpiMode.SystemAware)
        ' Note: Calling Application.EnableVisualStyles() is required for the task
        ' dialog to work.
        Application.EnableVisualStyles()
        Application.SetCompatibleTextRenderingDefault(False)
        Application.Run(New Form1())
    End Sub
End Module
