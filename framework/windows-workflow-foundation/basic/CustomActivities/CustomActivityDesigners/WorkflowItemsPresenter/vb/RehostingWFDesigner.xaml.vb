'-------------------------------------------------------------------
' Copyright (c) Microsoft Corporation. All rights reserved
'-------------------------------------------------------------------

Imports System
Imports System.Activities.Core.Presentation
Imports System.Activities.Presentation
Imports System.Activities.Presentation.Metadata
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
        ' create the workflow designer
        Dim wd = New WorkflowDesigner()
        wd.Load(New Sequence())
        DesignerBorder.Child = wd.View
        PropertyBorder.Child = wd.PropertyInspectorView
    End Sub

    Sub RegisterCustomMetadata()
        Dim builder As New AttributeTableBuilder()
        builder.AddCustomAttributes(GetType(Parallel), New DesignerAttribute(GetType(CustomParallelDesigner)))
        MetadataStore.AddAttributeTable(builder.CreateTable())
    End Sub



End Class


