// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

Dictionary<string, Scenario> scenarios = new(StringComparer.OrdinalIgnoreCase)
{
    ["cpu-hotspot"] = new("One CPU core remains saturated.", CpuScenarios.HotspotAsync),
    ["loh-gc"] = new("Full collections occur despite a modest object count.", MemoryScenarios.LohGcAsync),
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

if (!scenarios.TryGetValue(args[0], out Scenario? selected))
{
    Console.Error.WriteLine($"Unknown scenario '{args[0]}'. Run with --list to see the available scenarios.");
    Environment.ExitCode = 1;
    return;
}

int durationSeconds = 30;
if (args.Length > 1 && (!int.TryParse(args[1], out durationSeconds) || durationSeconds <= 0))
{
    Console.Error.WriteLine($"Invalid duration '{args[1]}'. Specify a positive whole number of seconds.");
    Environment.ExitCode = 1;
    return;
}

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
