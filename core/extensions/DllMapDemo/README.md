---
languages:
- csharp
- cpp
products:
- dotnet
page_type: sample
name: "DllMap Demo"
urlFragment: "dllmap-demo"
description: "A sample that illustrates the use of NativeLibrary APIs to implement library name mappings"
---

# DllMap Demo

This sample illustrates the use of NativeLibrary APIs to implement library name mappings similar to the [Mono](https://www.mono-project.com/) [Dllmap](https://www.mono-project.com/docs/advanced/pinvoke/dllmap/) feature.

## NativeLibrary APIs

.NET Core 3.1 provides a rich set of APIs to manage native libraries:

- [NativeLibrary APIs](https://docs.microsoft.com/dotnet/api/system.runtime.interopservices.nativelibrary): Perform operations on native libraries (such as `Load()`, `Free()`, get the address of an exported  symbol, etc.) in a platform-independent way from managed code.
- [DllImport Resolver callback](https://docs.microsoft.com/dotnet/api/system.runtime.interopservices.nativelibrary.setdllimportresolver):  Gets a callback for first-chance native library resolution using custom logic.
- [Native Library Resolve event](https://docs.microsoft.com/dotnet/api/system.runtime.loader.assemblyloadcontext.resolvingunmanageddll): Get an event for last-chance native library resolution using custom logic.

## Library Mapping

These APIs can be used to implement custom native library resolution logic, including DllMap, as illustrated in this example. The sample demonstrates:

- An [app](Demo.cs) that pInvokes a method in `OldLib`, but runs in an environment where only [`NewLib`](NewLib.cpp) is available.
- The [XML file](Demo.xml) that maps the library name from `OldLib` to `NewLib`.
- The [Map](Map.cs) implementation, which parses the above mapping and uses `NativeLibrary` APIs to load the correct library.

## Build and Run

1. Install .NET Core 3.1 or newer.

1. Use the .NET Core SDK to build the project via `dotnet build`.

1. Build the native component `NewLib.cpp` as a dynamic library, using the platform's native toolset.

    Place the generated native library (`NewLib.dll` / `libNewLib.so` / `libNewLib.dylib`) in the `dotnet build` output directory.

1. Run the app with `dotnet run`
