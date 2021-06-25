//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------


using System.Activities;
using System.Activities.Expressions;
using System.Activities.Presentation;
using System.Activities.Presentation.Model;
using System.Activities.Presentation.Services;
using System.Activities.Presentation.View;
using System.Activities.Statements;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Microsoft.Samples.Activities.Designer.ProgrammingModelItemTree
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
        }

        WorkflowDesigner wd;

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            wd = new WorkflowDesigner();
            wd.Load(ReturnSequence());
            Grid.SetColumn(wd.PropertyInspectorView, 1);
            windowGrid.Children.Add(wd.PropertyInspectorView);
            treeView1.DataContext = new List<ModelItem> { wd.Context.Services.GetService<ModelService>().Root };
            ChangeButton.IsEnabled = true;
            
        }

        private void treeView1_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            // we need to set the selection in the workflow designer to match this to update the proeprty grid.
            ModelItem mi = e.NewValue as ModelItem;
            if (null != mi) wd.Context.Items.SetValue(new Selection(mi));


        }

        private Activity ReturnSequence()
        {
            return new Sequence
            {
                DisplayName = "mySequence",
                Activities =
                {
                    new Persist(),
                    new Delay(),
                    new While
                    {
                         Condition = new Literal<bool>(true),
                         Body = new Sequence
                         {
                             Activities = 
                             {
                                 new WriteLine()
                             }
                         }
                    }
                    

                }
            };
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            // get the root model item
            ModelItem mi = wd.Context.Services.GetService<ModelService>().Root;
            ModelProperty mp = mi.Properties["Activities"];
            mp.Collection.Add(new Persist());
            ModelItem justAdded = mp.Collection.Last();
            // edit an item
            // note, always go through the SetValue route to ensure change 
            // tracking occurs at the ModelItemTree level
            justAdded.Properties["DisplayName"].SetValue("new name");
        }
    }
}
