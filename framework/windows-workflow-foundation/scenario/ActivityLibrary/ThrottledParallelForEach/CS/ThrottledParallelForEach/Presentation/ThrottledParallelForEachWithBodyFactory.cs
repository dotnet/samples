//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------
using System.Activities;
using System.Activities.Presentation;
using System.Windows;

namespace Microsoft.Samples.Activities.Statements.Presentation
{
    // creates a ThrottledParallelForEach activity with its Body (ActivityyAction) configured
    public sealed class ThrottledParallelForEachWithBodyFactory<T> : IActivityTemplateFactory
    {
        public Activity Create(DependencyObject target)
        {
            return new Microsoft.Samples.Activities.Statements.ThrottledParallelForEach<T>
            {
                Body = new ActivityAction<T>()
                {
                    Argument = new DelegateInArgument<T>()
                    {
                        Name = "item"
                    }
                }
            };
        }
    }
}