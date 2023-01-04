using System.Windows;

namespace WPF_WinRT_NetFx
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
#pragma warning disable CA1416 // Validate platform compatibility
            label1.Content = new Windows.Devices.Input.TouchCapabilities().TouchPresent == 1 ?
                                    "Touch Present" : "Touch not present";
#pragma warning restore CA1416 // Validate platform compatibility
        }
    }
}
