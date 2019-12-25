using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;


namespace AsyncWalkthrough_HttpClient
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void startButton_Click(object sender, RoutedEventArgs e)
        {
            // Disable the button until the operation is complete.
            startButton.IsEnabled = false;

            resultsTextBox.Clear();

            // One-step async call.
            await SumPageSizesAsync();

            //// Two-step async call.
            //Task sumTask = SumPageSizesAsync();
            //await sumTask;

            resultsTextBox.Text += "\nControl returned to startButton_Click.\n";

            // Re-enable the button in case you want to run the operation again.
            startButton.IsEnabled = true;
        }


        private async Task SumPageSizesAsync()
        {
            // Declare an HttpClient object.
            var client = new HttpClient();

            // Make a list of web addresses.
            List<string> urlList = SetUpUrlList();

            var total = 0;

            foreach (var url in urlList)
            {
                // GetByteArrayAsync returns a task. At completion, the task
                // produces a byte array.
                byte[] urlContents = await client.GetByteArrayAsync(url);

                DisplayResults(url, urlContents);

                // Update the total.
                total += urlContents.Length;
            }

            // Display the total count for all of the websites.
            resultsTextBox.Text += $"\n\nTotal bytes returned:  {total}\n";
        }


        private List<string> SetUpUrlList()
        {
            List<string> urls = new List<string>
            {
                "https://docs.microsoft.com",
                "https://docs.microsoft.com/aspnet",
                "https://docs.microsoft.com/dotnet",
                "https://docs.microsoft.com/dotnet/core",
                "https://docs.microsoft.com/dotnet/core/tools",
                "https://docs.microsoft.com/dotnet/csharp/tour-of-csharp",
                "https://docs.microsoft.com/dotnet/csharp/whats-new/csharp-8",
                "https://docs.microsoft.com/dotnet/desktop-wpf",
                "https://docs.microsoft.com/visualstudio",
                "https://docs.microsoft.com/visualstudio/subscriptions"
            };
            return urls;
        }

        private void DisplayResults(string url, byte[] content)
        {
            // Display the length of each website. The string format 
            // is designed to be used with a monospaced font, such as
            // Lucida Console or Global Monospace.
            var bytes = content.Length;
            // Strip off the "https://".
            var displayUrl = url.Replace("https://", "");
            resultsTextBox.Text += $"\n{displayUrl,-58} {bytes,8}";
        }
    }
}
