//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------
using System;
using System.Windows;

namespace Microsoft.Samples.VisualWorkflowTracking
{
    /// <summary>
    /// Interaction logic for VisualWorkflowTrackingWindow.xaml
    /// </summary>
    public partial class VisualWorkflowTrackingWindow : Window
    {
        public VisualWorkflowTrackingWindow()
        {
            InitializeComponent();
        }

        //Event handler for Run Workflow Menu Item
        private void btnRunLoadedWorkflow_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                WFHost.RunWorkflow();
            }
            catch (Exception ex) 
            {
                System.Windows.MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void fileMenuExit_Click(object sender, RoutedEventArgs e)
        {
            App.Current.Shutdown();
        }
    }
}
