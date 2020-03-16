# Sample .NET Core Profilers

These profilers are sample profilers intended to demonstrate core concepts to .NET Core profiling. A profiler is a native (C/C++) library that is loaded by the runtime and can interact with the runtime. Profilers can be notified when certain events happen or interact with the runtime to do things like rewrite IL, inspect arguments, and inspect managed callstacks.

The profilers here will be small and only intended to demonstrate a single concept each, but should provide some scaffolding for getting a profiler up and running if need be.
