// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

internal static class IoScenarios
{
    public static Task TinyWritesAsync(CancellationToken token)
    {
        string path = Path.Combine(Path.GetTempPath(), $"tiny-writes-{Environment.ProcessId}.dat");
        try
        {
            using FileStream stream = new(path, FileMode.Create, FileAccess.Write, FileShare.Read, 1);
            byte[] value = [42];
            while (!token.IsCancellationRequested)
            {
                stream.Write(value);
                stream.Flush();
            }
        }
        finally
        {
            File.Delete(path);
        }

        return Task.CompletedTask;
    }

    public static Task SyncIoThreadPoolAsync(CancellationToken token)
    {
        string path = Path.Combine(Path.GetTempPath(), $"sync-io-{Environment.ProcessId}.dat");
        byte[] data = new byte[4096];
        try
        {
            while (!token.IsCancellationRequested)
            {
                Task[] tasks = Enumerable.Range(0, 64).Select(_ => Task.Run(() =>
                {
                    using FileStream stream = new(path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite);
                    stream.Write(data);
                    stream.Flush(flushToDisk: true);
                })).ToArray();
                Task.WaitAll(tasks);
            }
        }
        finally
        {
            File.Delete(path);
        }

        return Task.CompletedTask;
    }
}
