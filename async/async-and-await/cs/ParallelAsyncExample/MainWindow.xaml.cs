using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace ParallelAsyncExample
{
    public partial class MainWindow : Window
    {
        private readonly HttpClient _client = new HttpClient { MaxResponseContentBufferSize = 1_000_000 };

        private readonly IEnumerable<string> _urlList = new string[]
        {
            "https://docs.microsoft.com",
            "https://docs.microsoft.com/azure",
            "https://docs.microsoft.com/powershell",
            "https://docs.microsoft.com/dotnet",
            "https://docs.microsoft.com/aspnet/core",
            "https://docs.microsoft.com/windows",
            "https://docs.microsoft.com/office",
            "https://docs.microsoft.com/enterprise-mobility-security",
            "https://docs.microsoft.com/visualstudio",
            "https://docs.microsoft.com/microsoft-365",
            "https://docs.microsoft.com/sql",
            "https://docs.microsoft.com/dynamics365",
            "https://docs.microsoft.com/surface",
            "https://docs.microsoft.com/xamarin",
            "https://docs.microsoft.com/azure/devops",
            "https://docs.microsoft.com/system-center",
            "https://docs.microsoft.com/graph",
            "https://docs.microsoft.com/education",
            "https://docs.microsoft.com/gaming"
        };

        private void OnStartButtonClick(object sender, RoutedEventArgs e)
        {
            _startButton.IsEnabled = false;
            _resultsTextBox.Clear();

            Task.Run(() => StartSumPageSizesAsync());
        }

        private async Task StartSumPageSizesAsync()
        {
            await SumPageSizesAsync();
            await Dispatcher.BeginInvoke(() =>
            {
                _resultsTextBox.Text += $"\nControl returned to {nameof(OnStartButtonClick)}.";
                _startButton.IsEnabled = true;
            });
        }

        private async Task SumPageSizesAsync()
        {
            var stopwatch = Stopwatch.StartNew();

            IEnumerable<Task<int>> downloadTasksQuery =
                from url in _urlList
                select ProcessUrlAsync(url, _client);

            Task<int>[] downloadTasks = downloadTasksQuery.ToArray();

            int[] lengths = await Task.WhenAll(downloadTasks);
            int total = lengths.Sum();

            await Dispatcher.BeginInvoke(() =>
            {
                stopwatch.Stop();

                _resultsTextBox.Text += $"\nTotal bytes returned:  {total:#,#}";
                _resultsTextBox.Text += $"\nElapsed time:          {stopwatch.Elapsed}\n";
            });
        }

        private async Task<int> ProcessUrlAsync(string url, HttpClient client)
        {
            byte[] byteArray = await client.GetByteArrayAsync(url);
            await DisplayResultsAsync(url, byteArray);

            return byteArray.Length;
        }

        private Task DisplayResultsAsync(string url, byte[] content) =>
            Dispatcher.BeginInvoke(() =>
                _resultsTextBox.Text += $"{url,-60} {content.Length,10:#,#}\n")
                      .Task;

        protected override void OnClosed(EventArgs e) => _client.Dispose();
    }
}
