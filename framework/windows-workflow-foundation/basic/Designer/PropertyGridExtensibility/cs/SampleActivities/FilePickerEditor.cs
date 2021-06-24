
//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------


using System.Activities.Presentation.PropertyEditing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;


namespace Microsoft.Samples.Activities.Designer.PropertyGridExtensibility
{
    class FilePickerEditor : DialogPropertyValueEditor
    {
        public FilePickerEditor()
        {
            this.InlineEditorTemplate = new DataTemplate();

            FrameworkElementFactory stack = new FrameworkElementFactory(typeof(StackPanel));
            stack.SetValue(StackPanel.OrientationProperty, Orientation.Horizontal);
            FrameworkElementFactory label = new FrameworkElementFactory(typeof(Label));
            Binding labelBinding = new Binding("Value");
            label.SetValue(Label.ContentProperty, labelBinding);
            label.SetValue(Label.MaxWidthProperty, 90.0);
            
            stack.AppendChild(label);

            FrameworkElementFactory editModeSwitch = new FrameworkElementFactory(typeof(EditModeSwitchButton));
            
            editModeSwitch.SetValue(EditModeSwitchButton.TargetEditModeProperty, PropertyContainerEditMode.Dialog);
            
            stack.AppendChild(editModeSwitch);

            this.InlineEditorTemplate.VisualTree = stack;
        }

        public override void ShowDialog(PropertyValue propertyValue, IInputElement commandSource)
        {
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
            if (ofd.ShowDialog() == true)
            {
                
                propertyValue.Value = ofd.FileName.Substring(ofd.FileName.LastIndexOf('\\')+1);
            }
        }
    }
}
