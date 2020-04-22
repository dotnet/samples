//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//

using System.IO;
using System.Threading.Tasks;

namespace System.Net
{
    /// <summary>Extension methods for working with WebRequest asynchronously.</summary>
    public static class WebRequestExtensions
    {
        /// <summary>Creates a Task that represents an asynchronous request to GetResponse.</summary>
        /// <param name="webRequest">The WebRequest.</param>
        /// <returns>A Task containing the retrieved WebResponse.</returns>
        public static Task<WebResponse> GetResponseAsync(this WebRequest webRequest)
        {
            if (webRequest == null) throw new ArgumentNullException(nameof(webRequest));
            return Task<WebResponse>.Factory.FromAsync(
                webRequest.BeginGetResponse, webRequest.EndGetResponse, webRequest /* object state for debugging */);
        }

        /// <summary>Creates a Task that represents an asynchronous request to GetRequestStream.</summary>
        /// <param name="webRequest">The WebRequest.</param>
        /// <returns>A Task containing the retrieved Stream.</returns>
        public static Task<Stream> GetRequestStreamAsync(this WebRequest webRequest)
        {
            if (webRequest == null) throw new ArgumentNullException(nameof(webRequest));
            return Task<Stream>.Factory.FromAsync(
                webRequest.BeginGetRequestStream, webRequest.EndGetRequestStream, webRequest /* object state for debugging */);
        }

        /// <summary>Creates a Task that respresents downloading all of the data from a WebRequest.</summary>
        /// <param name="webRequest">The WebRequest.</param>
        /// <returns>A Task containing the downloaded content.</returns>
        public static Task<byte[]> DownloadDataAsync(this WebRequest webRequest) =>
            // Asynchronously get the response.  When that's done, asynchronously read the contents.
            webRequest.GetResponseAsync().ContinueWith(response =>
            {
                return response.Result.GetResponseStream().ReadAllBytesAsync();
            }).Unwrap();
    }
}
