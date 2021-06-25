
//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System.Activities.Presentation.PropertyEditing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Microsoft.Samples.Activities.Designer.PropertyGridExtensibility
{
    class CustomInlineEditor : PropertyValueEditor
    {

        public CustomInlineEditor()
        {
           this.InlineEditorTemplate = new DataTemplate();
            
           FrameworkElementFactory stack = new FrameworkElementFactory(typeof(StackPanel));
           FrameworkElementFactory slider = new FrameworkElementFactory(typeof(Slider));
            Binding sliderBinding = new Binding("Value");
           sliderBinding.Mode = BindingMode.TwoWay;
           slider.SetValue(Slider.MinimumProperty, 0.0);
           slider.SetValue(Slider.MaximumProperty, 100.0);
           slider.SetValue(Slider.ValueProperty, sliderBinding);
           stack.AppendChild(slider);

           FrameworkElementFactory textb = new FrameworkElementFactory(typeof(TextBox));
           Binding textBinding = new Binding("Value");
           textb.SetValue(TextBox.TextProperty, textBinding);
           textb.SetValue(TextBox.IsEnabledProperty, false);

           stack.AppendChild(textb);

           this.InlineEditorTemplate.VisualTree = stack;

        }
    }
}
