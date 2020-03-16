# .Net Core stack sampling profiler

This sample profiler demonstrates how to use `ICorProfilerInfo10::SuspendRuntime` and `ICorProfiler10::ResumeRuntime` to sample managed callstacks.

When a profiler wanted to sample callstacks on .Net Framework, our guidance was to use the Win32 `SuspendThread` API to suspend a thread, then walk the native parts and pass the context to the first managed frame to `ICorProfilerInfo2::DoStackSnapshot`. This worked mainly, but had some nasty corner cases that the profiler could run in to unsuspectingly. In addition, there is no equivalent API to suspend a random thread on non-Windows platforms.

For .Net Core 3.0 we introduced the `SuspendRuntime` and `ResumeRuntime` APIs that pause all managed threads at a known good, walkable state. This allows the profiler to suspend an application, sample the threads, and resume without having to worry about the platform you are running on or the corner cases of suspending a thread on Windows.
