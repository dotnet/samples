Option Explicit Off
Option Infer On
Option Strict Off
Imports System
Imports System.Windows.Forms

Namespace MatchingGame
    Module Program
        ''' <summary>
        ''' The main entry point for the application.
        ''' </summary>
        Public Sub Main()
            Application.EnableVisualStyles()
            Application.SetCompatibleTextRenderingDefault(False)
            Application.Run(New Form1())
        End Sub
    End Module
End Namespace
