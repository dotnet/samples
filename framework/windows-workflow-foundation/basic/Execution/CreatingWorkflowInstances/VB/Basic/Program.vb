'----------------------------------------------------------------
' Copyright (c) Microsoft Corporation.  All rights reserved.
'----------------------------------------------------------------

Imports Microsoft.VisualBasic
Imports System
Imports System.Threading
Imports System.Activities
Imports System.Activities.Statements

Namespace Microsoft.Samples.WF.WorkflowInstances
    Friend Class Program
        Private Shared resetEvent As New ManualResetEvent(False)

        Private Shared Function BuildTestWorkflow() As Activity
            Dim s As New Sequence
            s.Activities.Add(New WriteLine() With {.Text = "one"})
            s.Activities.Add(New WriteLine() With {.Text = "two"})
            s.Activities.Add(New WriteLine() With {.Text = "buckle my shoe"})

            Return s
        End Function

        Shared Sub Main()
            ' This is how you run a workflow instance synchronously
            Dim activity As Activity = BuildTestWorkflow()
            WorkflowInvoker.Invoke(activity)

            ' This is how you run a workflow instance asynchronously,
            ' and can receive an event when it completes
            Dim instance As WorkflowApplication = New WorkflowApplication(BuildTestWorkflow())

            instance.Completed = AddressOf HandleCompleted

            instance.Run()
            resetEvent.WaitOne()

            Console.ReadLine()
        End Sub

        Private Shared Function HandleCompleted(ByVal e As WorkflowApplicationCompletedEventArgs) As UnhandledExceptionAction
            resetEvent.Set()
            Console.WriteLine("workflow instance completed, Id = " & e.InstanceId.ToString())
            Return UnhandledExceptionAction.Abort
        End Function
    End Class
End Namespace
