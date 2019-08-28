using System;
using System.Net.Http;
using System.Threading.Tasks;

public class AwaitOperator
{
    public static async Task Main()
    {
        Task<int> downloading = DownloadDocsMainPage();
        Console.WriteLine($"{nameof(Main)}: Launched downloading.");

        int bytesLoaded = await downloading;
        Console.WriteLine($"{nameof(Main)}: Downloaded {bytesLoaded} bytes.");
    }

    private static async Task<int> DownloadDocsMainPage()
    {
        Console.WriteLine($"{nameof(DownloadDocsMainPage)}: About to start downloading.");
        
        var client = new HttpClient();
        byte[] content = await client.GetByteArrayAsync("https://docs.microsoft.com/en-us/");
        
        Console.WriteLine($"{nameof(DownloadDocsMainPage)}: Finished downloading.");
        return content.Length;
    }
}
// Output similar to:
// DownloadDocsMainPage: About to start downloading.
// Main: Launched downloading.
// DownloadDocsMainPage: Finished downloading.
// Main: Downloaded 27700 bytes.
