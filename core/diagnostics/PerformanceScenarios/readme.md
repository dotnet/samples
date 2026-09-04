---
languages:
- csharp
products:
- dotnet
page_type: sample
name: "dotnet-trace collect-linux performance scenarios"
urlFragment: "dotnet-trace-collect-linux-performance-scenarios"
description: "Runnable .NET performance scenarios for practicing Linux investigation with dotnet-trace collect-linux."
---
# `dotnet-trace collect-linux` performance scenarios

This console application creates 20 distinct CPU, memory, garbage collection, blocking, contention, I/O, exception, startup, process, mixed-cause, and healthy-control scenarios. Use it with the [`dotnet-trace collect-linux` performance investigation tutorial](https://learn.microsoft.com/dotnet/core/diagnostics/dotnet-trace-collect-linux-scenarios) to practice selecting trace data and reaching a conclusion from the evidence.

## Download the source

Select **Browse code** at the top of this page to open the repository, or clone the [dotnet/samples](https://github.com/dotnet/samples) repository and navigate to `core/diagnostics/PerformanceScenarios`.

## Build and list the scenarios

The sample requires the .NET 10 SDK:

```dotnetcli
dotnet build -c Release
dotnet run -c Release --no-build -- --list
```

## Run a scenario

Pass the scenario name and an optional duration in seconds:

```dotnetcli
dotnet run -c Release --no-build -- cpu-hotspot 45
```

The application prints its process ID, the symptom to investigate, and the configured duration. The tutorial groups the scenarios by the CPU, GC, thread-time, syscall, exception, startup, or machine-wide trace strategy needed to diagnose them.

## Scenario matrix

Each scenario exercises a materially different diagnostic question, evidence source, collection timing requirement, or limitation. Scenarios that use the same trace configuration remain separate when the investigator must interpret the evidence differently or switch to a different diagnostic artifact.

| Scenario | Distinct investigation question | Differentiating evidence or next step |
| --- | --- | --- |
| `cpu-hotspot` | Which managed method directly consumes the CPU? | Exclusive CPU samples identify one expensive application method and its callers. |
| `inlining` | What can a CPU trace prove when optimization removes logical methods from physical stacks? | Samples localize the expensive surviving frame, but source or disassembly is required to divide cost among inlined methods. |
| `native-cpu` | Did CPU consumption move from managed code into a native library? | Symbolized stacks cross the P/Invoke boundary and attribute samples to `memcpy`. |
| `allocation-gc` | Is CPU cost driven by a high rate of small managed allocations and collections? | Allocation types and call stacks correlate with GC frequency, pauses, and CPU samples. |
| `loh-gc` | Are large objects causing full collections despite a modest object count? | Large `System.Byte[]` allocations, large object heap growth, and generation 2 collections distinguish large-object pressure from small-object churn. |
| `managed-memory-growth` | Is rising memory caused by managed objects that remain reachable? | Managed heap growth and allocation stacks identify the types and creation sites; a GC dump or process dump is required to prove retention roots. |
| `induced-gc` | Are explicit `GC.Collect` calls causing otherwise unexplained pauses? | GC start events report the `Induced` reason and connect collections to the application call site. |
| `native-memory-growth` | Is process memory rising outside the managed heap? | RSS rises while managed heap metrics remain stable, requiring a native memory profiler or operating-system memory map instead of more managed allocation data. |
| `sync-over-async` | Are synchronously blocked ThreadPool workers causing starvation? | Worker growth, cooperative-blocking events, task-wait stacks, and low CPU distinguish starvation from ordinary asynchronous waiting. |
| `async-delay` | Can physical thread stacks identify the logical operation that initiated an asynchronous delay? | Thread-time data proves timer waiting, but activities or application instrumentation are needed when the initiating request isn't preserved on the physical stack. |
| `lock-contention` | Which lock acquisition path is delayed, and is reader activity delaying writers? | Separate reader and writer stacks plus contention duration identify the affected acquisition path. |
| `deadlock` | How did a deadlock form, and what artifact proves the final ownership cycle? | Pre-reproduction contention events preserve opposing acquisition paths; a process dump is still required to prove current owners and the complete wait cycle. |
| `tiny-writes` | Is poor I/O efficiency caused by excessive syscalls for very little data? | `write` event stacks and external operation counts reveal syscall amplification and expose trace-event loss when counts disagree. |
| `sync-io-threadpool` | Is synchronous durable I/O tying up workers and delaying unrelated work? | `fsync` stacks identify the operation while thread-time data measures worker unavailability, requiring both I/O and scheduling evidence. |
| `jit-startup` | Is startup CPU dominated by JIT compilation before the process becomes idle? | Collection must begin before launch so early CPU samples can be correlated with JIT method events and `libclrjit` stacks. |
| `swallowed-exceptions` | Are caught exceptions reducing throughput without producing error logs? | First-chance exception events count every throw and identify repeated throw sites even when exceptions are handled. |
| `cpu-competition` | Is the target slow because other processes are consuming the machine? | Machine-wide CPU and scheduling data show overlapping competitors and runnable target threads receiving less CPU. |
| `process-churn` | Is repeated creation of short-lived processes causing overhead? | Process lifecycle and `execve` events preserve child launches that are too brief to receive CPU samples. |
| `cpu-and-contention` | Are multiple independent causes required to explain the symptom? | The trace contains both a CPU-intensive path and lock contention, preventing the investigation from stopping at the first plausible finding. |
| `healthy` | What does normal activity look like, and can the workflow avoid inventing a problem? | Stable CPU, memory, GC, and latency provide a negative control with no dominant anomalous stack, wait, pause, or event rate. |
