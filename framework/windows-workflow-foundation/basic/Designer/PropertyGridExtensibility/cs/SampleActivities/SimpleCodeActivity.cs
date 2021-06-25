
//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------


using System;
using System.Activities;
using System.Activities.Presentation.Metadata;
using System.Activities.Presentation.PropertyEditing;
using System.ComponentModel;

namespace Microsoft.Samples.Activities.Designer.PropertyGridExtensibility
{

    public sealed class SimpleCodeActivity : CodeActivity
    {
        public InArgument<string> Text { get; set; }
        public double RepeatCount { get; set; }
        public string FileName { get; set; }

        // since designer and activity are in same assembly register in static constructor
        static SimpleCodeActivity()
        {
            AttributeTableBuilder builder = new AttributeTableBuilder();
            builder.AddCustomAttributes(typeof(SimpleCodeActivity), "RepeatCount", new EditorAttribute(typeof(CustomInlineEditor), typeof(PropertyValueEditor)));
            builder.AddCustomAttributes(typeof(SimpleCodeActivity), "FileName", new EditorAttribute(typeof(FilePickerEditor), typeof(DialogPropertyValueEditor)));
            MetadataStore.AddAttributeTable(builder.CreateTable());
        }

        public SimpleCodeActivity()
        {

        }

        protected override void Execute(CodeActivityContext context)
        {
            // Obtain the runtime value of the Text input argument
            string text = context.GetValue(this.Text);
            for (int i = 0; i < RepeatCount; i++)
            {
                Console.WriteLine("Value entered was {0}", text);
            }
        }
    }


}
