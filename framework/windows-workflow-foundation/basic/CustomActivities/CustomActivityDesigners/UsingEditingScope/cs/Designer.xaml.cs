//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System.Activities.Core.Presentation;
using System.Activities.Presentation;
using System.Activities.Presentation.Hosting;
using System.Activities.Presentation.Model;
using System.Activities.Presentation.Services;
using System.Activities.Statements;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;

namespace Microsoft.Samples.Activities.Designer.UsingEditingScope
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Designer : Window
    {
        WorkflowDesigner workflowDesigner;
        public ObservableCollection<ModelEditingScope> EditingScopes {get; set;}

        public Designer()
        {
            InitializeComponent();
            EditingScopes  = new ObservableCollection<ModelEditingScope>();
             workflowDesigner = new WorkflowDesigner();
            this.DataContext = this;
            (new DesignerMetadata()).Register();
            workflowDesigner.Load(new Sequence
            {
                Activities =
                {
                    new If
                    {
                        Then = new Sequence
                        {
                            Activities = 
                            {
                                new Persist(),
                                new WriteLine { Text = "foo" }
                            }
                        }
                    }
                }
            });
            Grid.SetColumn(workflowDesigner.View, 1);
            Grid.SetColumn(workflowDesigner.PropertyInspectorView, 2);
            ApplicationGrid.Children.Add(workflowDesigner.View);
            ApplicationGrid.Children.Add(workflowDesigner.PropertyInspectorView);
        }

        private void BeginEdit_Click(object sender, RoutedEventArgs e)
        {
            ModelItem mi = workflowDesigner.Context.Services.GetService<ModelService>().Root;
            
            // note that editing scopes can nest, so we will keep track of the "top" with a stack.
            EditingScopes.Insert(0,mi.BeginEdit());
            // we know that the root is a sequence
            // we make multiple changes to the model item tree.
            // you'll note in the UI, nothing has changed yet, as this has not
            // yet been committed to the model item tree

            mi.Properties["Activities"].Collection.Add(new WriteLine { Text = "here is the text" });
            mi.Properties["Activities"].Collection.Add(new Persist());
            mi.Properties["Activities"].Collection.Add(new Flowchart());
            CloseButton.IsEnabled = true;
            RevertButton.IsEnabled = true;
        }

        private void EndEditing_Click(object sender, RoutedEventArgs e)
        {
            // two things to note here when this editing scope is complete.
            //   a.> the activities appear
            //   b.> htting ctl-z to undo will undo the "batch" of work
            EditingScopes[0].Complete();
            EditingScopes.RemoveAt(0);
            if (EditingScopes.Count == 0)
            {
                CloseButton.IsEnabled = false;
                RevertButton.IsEnabled = false;
            }

        }

        private void RevertEditing_Click(object sender, RoutedEventArgs e)
        {
            // to see this, click "open editing scope" twice (this has the impact of adding 6 items)
            // Click revert once and then click end editing scope. You will only see three items added, as the second
            // editing scope was reverted prior to completing the containing editing scope

            EditingScopes[0].Revert();
            EditingScopes.RemoveAt(0); 
            if (EditingScopes.Count == 0)
            {
                CloseButton.IsEnabled = false;
                RevertButton.IsEnabled = false;
            }
        }
    }
}
