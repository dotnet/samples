---
languages:
- csharp
- cpp
products:
- dotnet
page_type: sample
name: "IDynamicInterfaceCastable: Supporting Interfaces Dynamically"
urlFragment: "idynamicinterfacecastable"
description: "A .NET application that shows how to implement IDynamicInterfaceCastable to project a native object as implementing different managed interfaces."
---

# `IDynamicInterfaceCastable` Sample

The [`IDynamicInterfaceCastable` API](https://docs.microsoft.com/dotnet/api/system.runtime.interopservices.idynamicinterfacecastable) was introduced in .NET 5 as a way for creating a .NET class that supports interfaces which are not in its metadata.

This sample provides an implementation of `IDynamicInterfaceCastable` that projects a native object as implementing different known managed interfaces. It uses COM conventions (such as `QueryInterface`) for interacting with the native object, but does not require the actual COM system.

> [!NOTE]
> The sample uses `Marshal` APIs as part of interacting with the native library. The introduction of [C# function pointers](https://github.com/dotnet/csharplang/blob/994c41586e07e38fb6b30902b1715b4025d80c52/proposals/function-pointers.md) will allow that interaction to occur in a more performant manner.

## Prerequisites

* [.NET 5.0 SDK](https://dotnet.microsoft.com/download) Preview 7 or later

* C++ compiler
  * Windows: `cl.exe`
    * See [installation instructions](https://docs.microsoft.com/cpp/build/building-on-the-command-line#download-and-install-the-tools).
  * Linux/macOS: `g++`

## Build and Run

1) In order to build and run, all prerequisites must be installed. The following are also required:

    * On Linux/macOS, the C++ compiler (`g++`) must be on the path.
    * The C++ compiler (`cl.exe` or `g++`) and `dotnet` must be the same bitness (32-bit versus 64-bit).
      * On Windows, the sample is set up to use the bitness of `dotnet` to find the corresponding `cl.exe`

1) Navigate to the root directory.

1) Run the sample. Do one of the following:

    * Use `dotnet run` (which will build and run at the same time).
    * Use `dotnet build` to build the executable. The executable will be in `bin` under a subdirectory for the configuration (`Debug` is the default).
        * Windows: `bin\Debug\IDynamicInterfaceCastableSample.exe`
        * Non-Windows: `bin/Debug/IDynamicInterfaceCastableSample`

The expected output will show information about the native objects as they are represented in manage as well as results from calls to the native objects:

```
Native Object #0
 - does not implement IGreet
 - does not implement ICompute

Native Object #1
 - implements IGreet
    -- Hello World from NativeObject #1
 - does not implement ICompute

Native Object #2
 - does not implement IGreet
 - implements ICompute
    -- Returned sum: 5

Native Object #3
 - implements IGreet
    -- Hello World from NativeObject #3
 - implements ICompute
    -- Returned sum: 6
```

Note: The way the sample is built is relatively complicated. The goal is that it's possible to build and run the sample with simple `dotnet run` with minimal requirements on pre-installed tools. Typically real-world projects which have both managed and native components will use different build systems for each; for example msbuild/dotnet for managed and CMake for native.

## Visual Studio support

The `src\IDynamicInterfaceCastableSample.sln` can be used to open the sample in Visual Studio 2019. In order to be able to build from Visual Studio, though, it has to be started from the correct developer environment. From the developer environment console, start it with `devenv src\IDynamicInterfaceCastableSample.sln`. With that, the solution can be built. To run it, set the start project to `IDynamicInterfaceCastableSample`.
