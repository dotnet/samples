// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics;

internal static class SystemScenarios
{
    internal const string WorkerArgument = "--cpu-competition-worker";

    public static async Task CpuCompetitionAsync(CancellationToken token)
    {
        string processPath = Environment.ProcessPath ?? throw new InvalidOperationException("Unable to locate the workload executable.");
        List<Process> competitors = [];
        try
        {
            int competitorCount = Math.Min(32, Math.Max(2, Environment.ProcessorCount * 2));
            for (int index = 0; index < competitorCount; index++)
            {
                ProcessStartInfo startInfo = new(processPath, $"{WorkerArgument} 60")
                {
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                };
                competitors.Add(Process.Start(startInfo) ?? throw new InvalidOperationException("Unable to start a competing process."));
            }

            await CpuWorkerAsync(token);
        }
        finally
        {
            foreach (Process competitor in competitors)
            {
                if (!competitor.HasExited)
                {
                    competitor.Kill(entireProcessTree: true);
                }

                competitor.Dispose();
            }
        }
    }

    public static Task ProcessChurnAsync(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            using Process process = Process.Start(new ProcessStartInfo("/bin/true")
            {
                UseShellExecute = false,
            }) ?? throw new InvalidOperationException("Unable to start /bin/true.");
            process.WaitForExit();
        }

        return Task.CompletedTask;
    }

    public static async Task CpuAndContentionAsync(CancellationToken token)
    {
        object gate = new();
        Task cpu = Task.Run(() => CpuWorkerAsync(token), token);
        int contenderCount = Math.Min(32, Math.Max(8, Environment.ProcessorCount));
        Task[] contenders = Enumerable.Range(0, contenderCount)
            .Select(_ => Task.Run(() =>
            {
                while (!token.IsCancellationRequested)
                {
                    lock (gate)
                    {
                        Thread.Sleep(20);
                    }
                }
            }, token))
            .ToArray();
        await Task.WhenAll(contenders.Append(cpu));
    }

    public static async Task HealthyAsync(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            long result = 17;
            for (int index = 0; index < 1_000; index++)
            {
                result = unchecked((result * 31) ^ index);
            }

            GC.KeepAlive(result);
            await Task.Delay(20, token);
        }
    }

    internal static Task CpuWorkerAsync(CancellationToken token)
    {
        long result = 0;
        while (!token.IsCancellationRequested)
        {
            for (int index = 0; index < 10_000; index++)
            {
                result = unchecked((result * 31) ^ index);
            }
        }

        GC.KeepAlive(result);
        return Task.CompletedTask;
    }
}
