//  Copyright (c) Microsoft Corporation. All rights reserved.

using System;
using System.Windows;
using System.Windows.Data;

namespace Microsoft.Samples.DataBinding
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>

    public partial class Window1 : Window
    {
        //Keep client for lifetime of Window
        private AlbumServiceClient client = new AlbumServiceClient();

        public Window1()
        {
            InitializeComponent();
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            // Set the event handlers for when call completes
            client.GetAlbumListCompleted += new EventHandler<GetAlbumListCompletedEventArgs>(client_GetAlbumListCompleted);
            client.AddAlbumCompleted += new EventHandler<System.ComponentModel.AsyncCompletedEventArgs>(client_AddAlbumCompleted);

            // Bind the data returned from the service to the gridPanel UI element
            client.GetAlbumListAsync();
        }

        // Event handler for when call is complete
        void client_AddAlbumCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            client.GetAlbumListAsync();
        }

        // Event handler for when call is complete
        void client_GetAlbumListCompleted(object sender, GetAlbumListCompletedEventArgs e)
        {
            // This is on the UI thread, gridPanel can be accessed directly
            gridPanel.DataContext = e.Result;
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            // Clean up client when Window closes
            // Closing the client gracefully closes the connection and cleans up resources
            client.Close();
        }


        private void OnAddNew(object sender, RoutedEventArgs e)
        {
            string value = newAlbumName.Text;
            client.AddAlbumAsync(value);
        }
    }


    public class TextLen2Bool : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string text = (string)value;
            return (text.Length > 0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }

}
