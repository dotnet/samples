'-------------------------------------------------------------------
' Copyright (c) Microsoft Corporation. All rights reserved
'-------------------------------------------------------------------

Imports System.Activities

Public Class SimpleNativeActivity
    Inherits NativeActivity

    ' this property contains an activity that will be scheduled in the execute method
    ' the WorkflowItemPresenter in the designer is bound to this to enable editing
    ' of the value
    Public Property Body As Activity
    
    Protected Overloads Overrides Sub CacheMetadata(ByVal metadata as System.Activities.NativeActivityMetadata)
		metadata.AddChild(Body)
		MyBase.CacheMetadata(metadata)	
    End Sub

    Protected Overloads Overrides Sub Execute(ByVal context As System.Activities.NativeActivityContext)
        context.ScheduleActivity(Body)
    End Sub
End Class
