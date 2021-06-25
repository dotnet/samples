//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System;
using System.Activities.Presentation.Metadata;
using System.ComponentModel;

namespace Microsoft.Samples.Activities.Statements
{
    public partial class SendMailDesigner
    {
        public SendMailDesigner()
        {
            InitializeComponent();
        }

        public static void RegisterMetadata(AttributeTableBuilder builder)
        {
            Type type = typeof(SendMail);
            builder.AddCustomAttributes(type, new DesignerAttribute(typeof(SendMailDesigner)));
            builder.AddCustomAttributes(type, type.GetProperty("To"), BrowsableAttribute.No);
            builder.AddCustomAttributes(type, type.GetProperty("From"), BrowsableAttribute.No);
            builder.AddCustomAttributes(type, type.GetProperty("Subject"), BrowsableAttribute.No);
            builder.AddCustomAttributes(type, type.GetProperty("Host"), BrowsableAttribute.No);
        }
    }
}
