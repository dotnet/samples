// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

internal static class BlockingScenarios
{
    public static Task SyncOverAsyncAsync(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            int taskCount = Math.Min(64, Math.Max(16, Environment.ProcessorCount * 2));
            Task[] tasks = Enumerable.Range(0, taskCount)
                .Select(_ => Task.Run(() => DelayedOperationAsync().Result))
                .ToArray();
            Task.WaitAll(tasks);
        }

        return Task.CompletedTask;
    }

    public static async Task AsyncDelayAsync(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            await Task.Delay(250, token);
        }
    }

    public static Task ReaderWriterContentionAsync(CancellationToken token)
    {
        using ReaderWriterLockSlim gate = new();
        int readerCount = Math.Min(32, Math.Max(8, Environment.ProcessorCount * 2));
        Thread[] readers = Enumerable.Range(0, readerCount)
            .Select(_ => new Thread(() =>
            {
                while (!token.IsCancellationRequested)
                {
                    gate.EnterReadLock();
                    Thread.Sleep(20);
                    gate.ExitReadLock();
                }
            }))
            .ToArray();
        Thread writer = new(() =>
        {
            while (!token.IsCancellationRequested)
            {
                gate.EnterWriteLock();
                Thread.Sleep(5);
                gate.ExitWriteLock();
            }
        });

        foreach (Thread reader in readers)
        {
            reader.Start();
        }

        writer.Start();
        foreach (Thread reader in readers)
        {
            reader.Join();
        }

        writer.Join();
        return Task.CompletedTask;
    }

    public static async Task DeadlockAsync(CancellationToken token)
    {
        object first = new();
        object second = new();
        using ManualResetEventSlim firstHeld = new();
        using ManualResetEventSlim secondHeld = new();

        new Thread(() =>
        {
            lock (first)
            {
                firstHeld.Set();
                secondHeld.Wait();
                lock (second)
                {
                }
            }
        })
        {
            IsBackground = true,
        }.Start();

        new Thread(() =>
        {
            lock (second)
            {
                secondHeld.Set();
                firstHeld.Wait();
                lock (first)
                {
                }
            }
        })
        {
            IsBackground = true,
        }.Start();

        await Task.Delay(Timeout.Infinite, token);
    }

    private static async Task<int> DelayedOperationAsync()
    {
        await Task.Delay(500);
        return 42;
    }
}
