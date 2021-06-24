'-------------------------------------------------------------------
' Copyright (c) Microsoft Corporation. All rights reserved
'-------------------------------------------------------------------

Imports System
Imports System.Activities.Core.Presentation
Imports System.Activities.Presentation
Imports System.Activities.Statements
Imports System.Windows


    Class RehostingWfDesigner
        Protected Overrides Sub OnInitialized(ByVal e As EventArgs)
            MyBase.OnInitialized(e)
            ' register metadata
            Dim metadata = New DesignerMetadata()
            metadata.Register()

            ' create the workflow designer
            Dim wd = New WorkflowDesigner()
            wd.Load(New Sequence())
            DesignerBorder.Child = wd.View
            PropertyBorder.Child = wd.PropertyInspectorView


        End Sub

    End Class


