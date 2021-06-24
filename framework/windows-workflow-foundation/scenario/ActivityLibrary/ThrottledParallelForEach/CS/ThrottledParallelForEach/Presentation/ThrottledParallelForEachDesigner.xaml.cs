//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------
using System;
using System.Activities.Presentation;
using System.Activities.Presentation.View;
using System.Collections.Generic;
using System.Windows;

namespace Microsoft.Samples.Activities.Statements.Presentation
{
    /// <summary>
    /// Interaction logic for ForEachDesigner.xaml
    /// </summary>
    partial class ParallelForEachDesigner
    {
        public ParallelForEachDesigner()
        {
            InitializeComponent();
        }

        void OnValuesBoxLoaded(object sender, RoutedEventArgs e)
        {
            ExpressionTextBox etb = sender as ExpressionTextBox;            
            etb.ExpressionType = typeof(IEnumerable<>).MakeGenericType(this.ModelItem.ItemType.GetGenericArguments());
        }
    }
}

