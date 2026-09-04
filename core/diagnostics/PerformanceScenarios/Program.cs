// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

Dictionary<string, Scenario> scenarios = new(StringComparer.OrdinalIgnoreCase)
{
    ["cpu-hotspot"] = new("One CPU core remains saturated.", CpuScenarios.HotspotAsync),
    ["inlining"] = new("CPU is high, but logical helper frames might be optimized away.", CpuScenarios.InliningAsync),
    ["native-cpu"] = new("One core is busy, but little work appears in managed methods.", CpuScenarios.NativeAsync),
    ["allocation-gc"] = new("A small text transformation consumes excessive CPU and allocation bandwidth.", MemoryScenarios.AllocationGcAsync),
    ["loh-gc"] = new("Full collections occur despite a modest object count.", MemoryScenarios.LohGcAsync),
    ["managed-memory-growth"] = new("Managed memory rises instead of reaching a steady state.", MemoryScenarios.ManagedGrowthAsync),
    ["induced-gc"] = new("Stop-the-world pauses occur more often than allocation warrants.", MemoryScenarios.InducedGcAsync),
    ["native-memory-growth"] = new("RSS rises while the managed heap remains nearly flat.", MemoryScenarios.NativeGrowthAsync),
    ["sync-over-async"] = new("Async-looking work stalls while the ThreadPool worker count grows.", BlockingScenarios.SyncOverAsyncAsync),
    ["async-delay"] = new("Operations take about 250 milliseconds despite negligible CPU use.", BlockingScenarios.AsyncDelayAsync),
    ["lock-contention"] = new("Writes are delayed while read traffic continues.", BlockingScenarios.ReaderWriterContentionAsync),
    ["deadlock"] = new("The application stops making progress with near-zero CPU.", BlockingScenarios.DeadlockAsync),
    ["tiny-writes"] = new("Writing little data produces an unexpectedly high syscall rate.", IoScenarios.TinyWritesAsync),
    ["sync-io-threadpool"] = new("File activity delays unrelated queued work.", IoScenarios.SyncIoThreadPoolAsync),
    ["jit-startup"] = new("Startup consumes CPU before the process becomes idle.", RuntimeScenarios.JitStartupAsync),
    ["swallowed-exceptions"] = new("Throughput falls even though the application reports no errors.", RuntimeScenarios.SwallowedExceptionsAsync),
    ["cpu-competition"] = new("The target receives little CPU despite doing no blocking.", SystemScenarios.CpuCompetitionAsync),
    ["process-churn"] = new("The process launch rate is unexpectedly high.", SystemScenarios.ProcessChurnAsync),
    ["cpu-and-contention"] = new("CPU is high and throughput is low, but one cause might not explain both.", SystemScenarios.CpuAndContentionAsync),
    ["healthy"] = new("CPU, memory, and latency remain healthy.", SystemScenarios.HealthyAsync),
};

if (args.Length == 0 || args[0] is "--list" or "-l")
{
    Console.WriteLine("Usage: dotnet run -- <scenario> [duration-seconds]");
    Console.WriteLine();
    foreach ((string name, Scenario scenario) in scenarios)
    {
        Console.WriteLine($"  {name,-24} {scenario.Symptom}");
    }

    return;
}

if (args[0] == SystemScenarios.WorkerArgument)
{
    int workerDuration = args.Length > 1 ? int.Parse(args[1]) : 30;
    using CancellationTokenSource workerCancellation = new(TimeSpan.FromSeconds(workerDuration));
    await SystemScenarios.CpuWorkerAsync(workerCancellation.Token);
    return;
}

if (!scenarios.TryGetValue(args[0], out Scenario? selected))
{
    Console.Error.WriteLine($"Unknown scenario '{args[0]}'. Run with --list to see the available scenarios.");
    Environment.ExitCode = 1;
    return;
}

int durationSeconds = args.Length > 1 ? int.Parse(args[1]) : 30;
using CancellationTokenSource cancellation = new(TimeSpan.FromSeconds(durationSeconds));

Console.WriteLine($"Process ID: {Environment.ProcessId}");
Console.WriteLine($"Symptom: {selected.Symptom}");
Console.WriteLine($"Duration: {durationSeconds} seconds");

try
{
    await selected.RunAsync(cancellation.Token);
}
catch (OperationCanceledException) when (cancellation.IsCancellationRequested)
{
}

internal sealed record Scenario(string Symptom, Func<CancellationToken, Task> RunAsync);
