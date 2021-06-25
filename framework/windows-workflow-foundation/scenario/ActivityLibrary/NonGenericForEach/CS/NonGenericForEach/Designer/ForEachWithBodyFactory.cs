//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

using System.Activities;
using System.Activities.Presentation;
using System.Windows;
using Microsoft.Samples.Activities.Statements;

namespace Microsoft.Samples.Activities.Statements.Presentation
{
    // creates a ForEach activity with its Body (ActivityyAction) configured
    public sealed class ForEachWithBodyFactory : IActivityTemplateFactory
    {
        public Activity Create(DependencyObject target)
        {
            return new ForEach()
            {
                Body = new ActivityAction<object>()
                {
                    Argument = new DelegateInArgument<object>()
                    {
                        Name = "item"
                    }
                }
            };
        }
    } 
}