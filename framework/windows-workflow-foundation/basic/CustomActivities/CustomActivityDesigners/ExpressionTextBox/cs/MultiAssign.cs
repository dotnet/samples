//-------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved
//-------------------------------------------------------------------
using System;
using System.Activities;
using System.ComponentModel;

namespace Microsoft.Samples.ExpressionTextBoxSample
{
    // Sets To1 = Value1 and To2 = Value2
    [Designer(typeof(MultiAssignDesigner))]
    public sealed class MultiAssign : CodeActivity
    {
        public OutArgument<String> To1 { get; set; }
        public OutArgument<String> To2 { get; set; }
        public InArgument<String> Value1 { get; set; }
        public InArgument<String> Value2 { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            // Obtain the runtime value of the arguments
            this.To1.Set(context, this.Value1.Get(context));
            this.To2.Set(context, this.Value2.Get(context));
        }
    }
}
