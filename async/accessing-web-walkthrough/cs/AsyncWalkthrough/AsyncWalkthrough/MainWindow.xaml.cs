using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Windows;

namespace AsyncWalkthrough
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

            // Two-step async call.
            //Task sumTask = SumPageSizesAsync();
            //await sumTask;

            resultsTextBox.Text += "\nControl returned to startButton_Click.\n";

            // Re-enable the button in case you want to run the operation again.
            startButton.IsEnabled = true;
        }

        private async Task SumPageSizesAsync()
        {
            // Make a list of web addresses.
            List<string> urlList = SetUpUrlList();

            var total = 0;

            foreach (var url in urlList)
            {
                byte[] urlContents = await GetUrlContentsAsync(url);

                DisplayResults(url, urlContents);

                // Update the total.          
                total += urlContents.Length;
            }
            // Display the total count for all of the websites.
            resultsTextBox.Text += $"\r\rTotal bytes returned:  {total}\n";
        }

        private List<string> SetUpUrlList()
        {
            var urls = new List<string>
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

        private async Task<byte[]> GetUrlContentsAsync(string url)
        {
            // Initialize an HttpWebRequest for the current url.
            var webReq = (HttpWebRequest)WebRequest.Create(url);

            // Send the request to the Internet resource and wait for
            // the response.
            using WebResponse response = await webReq.GetResponseAsync();
            // Get the stream that is associated with the response.
            using Stream responseStream = response.GetResponseStream();
            // The downloaded resource ends up in the variable named content.
            var content = new MemoryStream();
            // Read the bytes in responseStream and copy them to outputStream.  
            await responseStream.CopyToAsync(content);
            // Return the result as a byte array.
            return content.ToArray();
        }

        private void DisplayResults(string url, byte[] content)
        {
            // Display the length of each web site. The string format 
            // is designed to be used with a monospaced font, such as
            // Lucida Console or Global Monospace.
            int bytes = content.Length;
            // Strip off the "https://".
            string displayUrl = url.Replace("https://", "");
            resultsTextBox.Text += $"\n{displayUrl,-58} {bytes,8}";
        }
    }
}
