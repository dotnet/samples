//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace System.IO
{
    /// <summary>Extension methods for asynchronously working with streams.</summary>
    public static class StreamExtensions
    {
        private const int BUFFER_SIZE = 0x2000;

        /// <summary>Read from a stream asynchronously.</summary>
        /// <param name="stream">The stream.</param>
        /// <param name="buffer">An array of bytes to be filled by the read operation.</param>
        /// <param name="offset">The offset at which data should be stored.</param>
        /// <param name="count">The number of bytes to be read.</param>
        /// <returns>A Task containing the number of bytes read.</returns>
        public static Task<int> ReadAsync(
            this Stream stream, byte[] buffer, int offset, int count)
        {
            if (stream == null) throw new ArgumentNullException("stream");
            return Task<int>.Factory.FromAsync(
                stream.BeginRead, stream.EndRead,
                buffer, offset, count, stream /* object state */);
        }

        /// <summary>Write to a stream asynchronously.</summary>
        /// <param name="stream">The stream.</param>
        /// <param name="buffer">An array of bytes to be written.</param>
        /// <param name="offset">The offset from which data should be read to be written.</param>
        /// <param name="count">The number of bytes to be written.</param>
        /// <returns>A Task representing the completion of the asynchronous operation.</returns>
        public static Task WriteAsync(
            this Stream stream, byte[] buffer, int offset, int count)
        {
            if (stream == null) throw new ArgumentNullException("stream");
            return Task.Factory.FromAsync(
                stream.BeginWrite, stream.EndWrite,
                buffer, offset, count, stream);
        }

        /// <summary>Reads the contents of the stream asynchronously.</summary>
        /// <param name="stream">The stream.</param>
        /// <returns>A Task representing the contents of the file in bytes.</returns>
        public static Task<byte[]> ReadAllBytesAsync(this Stream stream)
        {
            if (stream == null) throw new ArgumentNullException("stream");

            // Create a MemoryStream to store the data read from the input stream
            int initialCapacity = stream.CanSeek ? (int)stream.Length : 0;
            var readData = new MemoryStream(initialCapacity);

            // Copy from the source stream to the memory stream and return the copied data
            return stream.CopyStreamToStreamAsync(readData).ContinueWith(t =>
            {
                t.PropagateExceptions();
                return readData.ToArray();
            });
        }

        /// <summary>Read the content of the stream, yielding its data in buffers to the provided delegate.</summary>
        /// <param name="stream">The stream.</param>
        /// <param name="bufferSize">The size of the buffers to use.</param>
        /// <param name="bufferAvailable">The delegate to be called when a new buffer is available.</param>
        /// <returns>A Task that represents the completion of the asynchronous operation.</returns>
        public static Task ReadBuffersAsync(this Stream stream, int bufferSize, Action<byte[], int> bufferAvailable)
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));
            if (bufferSize < 1) throw new ArgumentOutOfRangeException(nameof(bufferSize));
            if (bufferAvailable == null) throw new ArgumentNullException(nameof(bufferAvailable));

            // Read from the stream over and over, handing the buffers off to the bufferAvailable delegate
            // as they're available.  Delegate invocation will be serialized.
            return Task.Factory.Iterate(
                ReadIterator(stream, bufferSize, bufferAvailable).Cast<object>());
        }

        /// <summary>
        /// Creates an enumerable to be used with TaskFactoryExtensions.Iterate that reads data
        /// from an input stream and passes it to a user-provided delegate.
        /// </summary>
        /// <param name="input">The source stream.</param>
        /// <param name="bufferSize">The size of the buffers to be used.</param>
        /// <param name="bufferAvailable">
        /// A delegate to be invoked when a buffer is available (provided the
        /// buffer and the number of bytes in the buffer starting at offset 0.
        /// </param>
        /// <returns>An enumerable containing yielded tasks from the operation.</returns>
        private static IEnumerable<Task> ReadIterator(Stream input, int bufferSize, Action<byte[], int> bufferAvailable)
        {
            // Create a buffer that will be used over and over
            var buffer = new byte[bufferSize];

            // Until there's no more data
            while (true)
            {
                // Asynchronously read a buffer and yield until the operation completes
                var readTask = input.ReadAsync(buffer, 0, buffer.Length);
                yield return readTask;

                // If there's no more data in the stream, we're done.
                if (readTask.Result <= 0) break;

                // Otherwise, hand the data off to the delegate
                bufferAvailable(buffer, readTask.Result);
            }
        }

        /// <summary>Copies the contents of a stream to a file, asynchronously.</summary>
        /// <param name="source">The source stream.</param>
        /// <param name="destinationPath">The path to the destination file.</param>
        /// <returns>A Task that represents the asynchronous operation.</returns>
        public static Task CopyStreamToFileAsync(this Stream source, string destinationPath)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (destinationPath == null) throw new ArgumentNullException(nameof(destinationPath));

            // Open the output file for writing
            var destinationStream = FileAsync.OpenWrite(destinationPath);

            // Copy the source to the destination stream, then close the output file.
            return CopyStreamToStreamAsync(source, destinationStream).ContinueWith(t =>
            {
                var e = t.Exception;
                destinationStream.Close();
                if (e != null) throw e;
            }, TaskContinuationOptions.ExecuteSynchronously);
        }

        /// <summary>Copies the contents of one stream to another, asynchronously.</summary>
        /// <param name="source">The source stream.</param>
        /// <param name="destination">The destination stream.</param>
        /// <returns>A Task that represents the completion of the asynchronous operation.</returns>
        public static Task CopyStreamToStreamAsync(this Stream source, Stream destination)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (destination == null) throw new ArgumentNullException(nameof(destination));
            return Task.Factory.Iterate(
                CopyStreamIterator(source, destination).Cast<object>());
        }

        /// <summary>
        /// Creates an enumerable to be used with TaskFactoryExtensions.Iterate that copies data from one
        /// stream to another.
        /// </summary>
        /// <param name="input">The input stream.</param>
        /// <param name="output">The output stream.</param>
        /// <returns>An enumerable containing yielded tasks from the copy operation.</returns>
        private static IEnumerable<Task> CopyStreamIterator(Stream input, Stream output)
        {
            // Create two buffers.  One will be used for the current read operation and one for the current
            // write operation.  We'll continually swap back and forth between them.
            byte[][] buffers = new byte[2][] { new byte[BUFFER_SIZE], new byte[BUFFER_SIZE] };
            int filledBufferNum = 0;
            Task writeTask = null;

            // Until there's no more data to be read
            while (true)
            {
                // Read from the input asynchronously
                var readTask = input.ReadAsync(buffers[filledBufferNum], 0, buffers[filledBufferNum].Length);

                // If we have no pending write operations, just yield until the read operation has
                // completed.  If we have both a pending read and a pending write, yield until both the read
                // and the write have completed.
                if (writeTask == null)
                {
                    yield return readTask;
                    readTask.Wait(); // propagate any exception that may have occurred
                }
                else
                {
                    var tasks = new[] { readTask, writeTask };
                    yield return Task.Factory.WhenAll(tasks);
                    Task.WaitAll(tasks); // propagate any exceptions that may have occurred
                }

                // If no data was read, nothing more to do.
                if (readTask.Result <= 0) break;

                // Otherwise, write the written data out to the file
                writeTask = output.WriteAsync(buffers[filledBufferNum], 0, readTask.Result);

                // Swap buffers
                filledBufferNum ^= 1;
            }
        }
    }
}
