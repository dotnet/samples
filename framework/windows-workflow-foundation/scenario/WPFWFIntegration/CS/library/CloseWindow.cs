//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System;
using System.Activities;
using System.Windows;
using System.Windows.Markup;

namespace Microsoft.Samples.WPFWFIntegration
{

    [ContentProperty("Window")]
    public sealed class CloseWindow : CodeActivity
    {
        public InArgument<Window> Window { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            if (Application.Current == null)
            {
                throw new InvalidOperationException("Must have an application");
            }

            Window targetWindow = Window.Get(context);
            targetWindow.Close();
        }
    }
}
