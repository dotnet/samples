# .NET Core EventPipe profiler

This sample profiler demonstrates how to write and receive EventPipe events via ICorProfiler.

On desktop .NET Framework, we used ETW (Event Tracing for Windows) extensively to log runtime events. With the advent of .NET Core, ETW was not available on non-Windows platforms. To address this need, the runtime team added the ability for the runtime to log and buffer events. We call this mechanism EventPipe.

Starting with .NET 5.0, profilers can both write and read EventPipe events.

## Writing events

To write EventPipe events, you first create a provider, then define events on that provider. Once you have defined your provider and event(s), you can write events to your session.

## Reading events

When EventPipe events are written, they are emitted according to the [file format specification](https://github.com/microsoft/perfview/blob/main/src/TraceEvent/EventPipe/EventPipeFormat.md). The spec is designed to be efficient for both streaming and writing to a file for processing at a later date.

The EventPipe reading APIs allow you to create an in-process streaming session to receive events. You create a session by passing in the providers that you wish to receive events from, along with the keywords and level for each provider. Providers are identified by name.

Once you create your session, you will continue to receive any events that match the provider, keyword, and level until you terminate your session.

The events that are passed to `ICorProfilerCallback10::EventPipeEventDelivered` will have their metadata blobs and event data blobs already encoded according to the file format specification. It is the responsibility of the profiler to decode the data/metadata as needed.
