using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows;

namespace SyncExample
{
    public partial class MainWindow : Window
    {
        readonly IEnumerable<string> _urlList = new string[]
        {
            "https://docs.microsoft.com",
            "https://docs.microsoft.com/azure",
            "https://docs.microsoft.com/powershell",
            "https://docs.microsoft.com/dotnet",
            "https://docs.microsoft.com/aspnet/core",
            "https://docs.microsoft.com/windows"
        };

        void OnStartButtonClick(object sender, RoutedEventArgs e)
        {
            _resultsTextBox.Clear();

            SumPageSizes();

            _resultsTextBox.Text += $"\nControl returned to {nameof(OnStartButtonClick)}.";
        }

        void SumPageSizes()
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            int total = _urlList.Select(url => ProcessUrl(url)).Sum();

            stopwatch.Stop();
            _resultsTextBox.Text += $"\nTotal bytes returned:  {total:#,#}";
            _resultsTextBox.Text += $"\nElapsed time:          {stopwatch.Elapsed}\n";
        }

        int ProcessUrl(string url)
        {
            using var memoryStream = new MemoryStream();
            var webReq = (HttpWebRequest)WebRequest.Create(url);

            using WebResponse response = webReq.GetResponse();
            using Stream responseStream = response.GetResponseStream();
            responseStream.CopyTo(memoryStream);

            byte[] content = memoryStream.ToArray();
            DisplayResults(url, content);

            return content.Length;
        }

        void DisplayResults(string url, byte[] content) =>
            _resultsTextBox.Text += $"{url,-60} {content.Length,10:#,#}\n";
    }
}
