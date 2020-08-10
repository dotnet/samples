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
            await UpdateResultText($"\nControl returned to {nameof(OnStartButtonClick)}.");

            _startButton.IsEnabled = true;
        }

        async Task SumPageSizesAsync()
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            IEnumerable<Task<int>> downloadTasksQuery =
                from url in _urlList
                select ProcessURLAsync(url, _client);

            Task<int>[] downloadTasks = downloadTasksQuery.ToArray();

            int[] lengths = await Task.WhenAll(downloadTasks);
            int total = lengths.Sum();

            stopwatch.Stop();
            await UpdateResultText($"\nTotal bytes returned:  {total:#,#}");
            await UpdateResultText($"\nElapsed time:          {stopwatch.Elapsed}\n");
        }

        async Task<int> ProcessURLAsync(string url, HttpClient client)
        {
            byte[] byteArray = await client.GetByteArrayAsync(url);
            await DisplayResults(url, byteArray);

            return byteArray.Length;
        }

        Task DisplayResults(string url, byte[] content)
        {
            int bytes = content.Length;
            var displayURL = url.Replace("https://", "");

            return UpdateResultText($"{displayURL,-60} {bytes,10:#,#}\n");
        }

        Task UpdateResultText(string text) =>
            Dispatcher.BeginInvoke(
                () => _resultsTextBox.Text += text,
                DispatcherPriority.SystemIdle)
                      .Task;

        protected override void OnClosed(EventArgs e) => _client?.Dispose();
    }
}
