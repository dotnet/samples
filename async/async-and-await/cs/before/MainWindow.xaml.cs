using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;

namespace SerialAsyncExample
{
    public partial class MainWindow : Window
    {
        readonly HttpClient _client = new HttpClient { MaxResponseContentBufferSize = 1_000_000 };

        readonly IEnumerable<string> _urlList = new string[]
        {
            "https://docs.microsoft.com",
            "https://docs.microsoft.com/azure",
            "https://docs.microsoft.com/powershell",
            "https://docs.microsoft.com/dotnet",
            "https://docs.microsoft.com/aspnet/core",
            "https://docs.microsoft.com/windows"
        };

        async void OnStartButtonClick(object sender, RoutedEventArgs e)
        {
            _resultsTextBox.Clear();
            _startButton.IsEnabled = false;

            await SumPageSizesAsync();

            _resultsTextBox.Text += $"\nControl returned to {nameof(OnStartButtonClick)}.";
            _startButton.IsEnabled = true;
        }

        async Task SumPageSizesAsync()
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            int total = 0;
            foreach (string url in _urlList)
            {
                int contentLength = await ProcessUrlAsync(url, _client);
                total += contentLength;
            }

            stopwatch.Stop();
            _resultsTextBox.Text += $"\nTotal bytes returned:  {total:#,#}";
            _resultsTextBox.Text += $"\nElapsed time:          {stopwatch.Elapsed}\n";
        }

        async Task<int> ProcessUrlAsync(string url, HttpClient client)
        {
            byte[] content = await client.GetByteArrayAsync(url);
            DisplayResults(url, content);

            return content.Length;
        }

        void DisplayResults(string url, byte[] content) =>
            _resultsTextBox.Text += $"{url,-60} {content.Length,10:#,#}\n";

        protected override void OnClosed(EventArgs e) => _client?.Dispose();
    }
}
