'-------------------------------------------------------------------
' Copyright (c) Microsoft Corporation. All rights reserved
'-------------------------------------------------------------------

Imports System
Imports System.Activities.Core.Presentation
Imports System.Activities.Presentation
Imports System.Activities.Presentation.Metadata
Imports System.Activities.Presentation.Toolbox
Imports System.Activities.Statements
Imports System.ComponentModel
Imports System.Windows


Class RehostingWfDesigner
    Protected Overrides Sub OnInitialized(ByVal e As EventArgs)
        MyBase.OnInitialized(e)
        ' register metadata
        Dim metadata = New DesignerMetadata()
        metadata.Register()
        ' register custom metdata
        RegisterCustomMetadata()
        ' Add to Toolbox
        Toolbox.Categories.Add(New ToolboxCategory("Custom Activities"))
        Toolbox.Categories(1).Add(new ToolboxItemWrapper(GetType(SimpleNativeActivity)))
        ' create the workflow designer
        Dim wd = New WorkflowDesigner()
        wd.Load(New Sequence())
        DesignerBorder.Child = wd.View
        PropertyBorder.Child = wd.PropertyInspectorView
    End Sub

    Sub RegisterCustomMetadata()
        Dim builder As New AttributeTableBuilder()
        builder.AddCustomAttributes(GetType(SimpleNativeActivity), New DesignerAttribute(GetType(SimpleNativeDesigner)))
        MetadataStore.AddAttributeTable(builder.CreateTable())
    End Sub



End Class


