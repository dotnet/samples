---
languages:
- cpp
products:
- dotnet
page_type: sample
name: "ICorProfiler Samples"
urlFragment: "icorprofiler-samples"
description: "Example implementations of ICorProfiler for dotnet."
---

# Sample .NET Core profilers

These profilers are sample profilers intended to demonstrate core concepts to .NET Core profiling. A profiler is a native (C/C++) library that is loaded by the runtime and can interact with the runtime. Profilers can be notified when certain events happen or interact with the runtime to do things like rewrite IL, inspect arguments, and inspect managed callstacks.

The profilers here are small and only intended to demonstrate a single concept each. But they provide some scaffolding for getting a profiler up and running.
