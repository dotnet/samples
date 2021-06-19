using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Windows;

namespace SyncWalkthrough
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void startButton_Click(object sender, RoutedEventArgs e)
        {
            resultsTextBox.Clear();
            SumPageSizes();
            resultsTextBox.Text += "\r\nControl returned to startButton_Click.";
        }

        private void SumPageSizes()
        {
            // Make a list of web addresses.
            List<string> urlList = SetUpUrlList();

            var total = 0;
            foreach (var url in urlList)
            {
                // GetUrlContents returns the contents of url as a byte array.
                byte[] urlContents = GetUrlContents(url);

                DisplayResults(url, urlContents);

                // Update the total.
                total += urlContents.Length;
            }

            // Display the total count for all of the web addresses.
            resultsTextBox.Text += $"\n\nTotal bytes returned:  {total}\n";
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

        private byte[] GetUrlContents(string url)
        {
            // Initialize an HttpWebRequest for the current url.
            var webReq = (HttpWebRequest)WebRequest.Create(url);

            // Send the request to the Internet resource and wait for
            // the response.
            using WebResponse response = webReq.GetResponse();
            // Get the stream that is associated with the response.
            using Stream responseStream = response.GetResponseStream();
            // The downloaded resource ends up in the variable named content.
            var content = new MemoryStream();
            // Read the bytes in responseStream and copy them to outputStream.  
            responseStream.CopyTo(content);
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
