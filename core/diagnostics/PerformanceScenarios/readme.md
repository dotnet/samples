---
languages:
- csharp
products:
- dotnet
page_type: sample
name: "dotnet-trace collect-linux performance scenarios"
urlFragment: "dotnet-trace-collect-linux-performance-scenarios"
description: "Two .NET performance scenarios for the dotnet-trace collect-linux tutorial: a CPU hotspot and large object heap pressure."
ai-usage: ai-assisted
---
# `dotnet-trace collect-linux` performance scenarios

This console application creates two scenarios: a managed CPU hotspot and large object heap (LOH) allocation pressure. Use it with the [`dotnet-trace collect-linux` performance investigation tutorial](https://learn.microsoft.com/dotnet/core/diagnostics/dotnet-trace-collect-linux-performance) to collect trace data and reach a conclusion from the evidence.

## Download the source

Select **Browse code** at the top of this page to open the repository, or clone the [dotnet/samples](https://github.com/dotnet/samples) repository and navigate to `core/diagnostics/PerformanceScenarios`.

## Build and list the scenarios

The sample requires the .NET 10 SDK:

```dotnetcli
dotnet build -c Release
dotnet run -c Release --no-build -- --list
```

## Run a scenario

Pass the scenario name and an optional duration in seconds. Run the scenarios one at a time:

```dotnetcli
dotnet run -c Release --no-build -- cpu-hotspot 45
dotnet run -c Release --no-build -- loh-gc 45
```

The application prints its process ID, the symptom to investigate, and the configured duration. The tutorial shows how to collect machine-wide traces and analyze them in Visual Studio, with PerfView as an alternative.

## Scenario matrix

| Scenario | Distinct investigation question | Differentiating evidence or next step |
| --- | --- | --- |
| `cpu-hotspot` | Which managed method directly consumes the CPU? | Exclusive CPU samples identify one expensive application method and its callers. |
| `loh-gc` | Are large objects causing full collections despite a modest object count? | Large `System.Byte[]` allocations, large object heap growth, and generation 2 collections distinguish large-object pressure from small-object churn. |
