using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

class Example
{
   static async Task Main()
   {
      string[] args = Environment.GetCommandLineArgs();
      if (args.Length < 2)
         throw new ArgumentNullException("No URIs specified on the command line.");

      // Don't pass the executable file name
      var uris = args.Skip(1).ToArray();

      long characters = await GetPageLengthsAsync(uris);
      Console.WriteLine($"{uris.Length} pages, {characters:N0} characters");
   }

   private static async Task<long> GetPageLengthsAsync(string[] uris)
   {
      var client = new HttpClient();
      long pageLengths = 0;

      foreach (var uri in uris)
      {
         var escapedUri = new Uri(Uri.EscapeUriString(uri));
         string pageContents = await client.GetStringAsync(escapedUri);
         Interlocked.Add(ref pageLengths, pageContents.Length);
      }
      
      return pageLengths;
   }
}
