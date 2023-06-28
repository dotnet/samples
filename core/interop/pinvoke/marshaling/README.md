---
languages:
- csharp
- cpp
products:
- dotnet
page_type: sample
name: "P/Invoke Marshalling Sample"
urlFragment: "pinvoke-marshal-arguments"
description: "A .NET application that demonstrates different ways to marshal arguments to native functions when using P/Invokes."
---

# .NET Core P/Invoke Marshalling Sample

This project demonstrates different ways to marshal arguments to native functions when using P/Invokes. Documentation can be found here for [P/Invokes](https://docs.microsoft.com/dotnet/standard/native-interop/pinvoke) and here for the [Type marshaling](https://docs.microsoft.com/dotnet/standard/native-interop/type-marshaling).

## Prerequisites

* [.NET Core 3.1 SDK](https://dotnet.microsoft.com/download)

* C++ compiler
  * Windows: `cl.exe`
    * See [installation instructions](https://docs.microsoft.com/cpp/build/building-on-the-command-line#download-and-install-the-tools).
  * Linux/macOS: `g++`

## Build and Run

1) In order to build and run, all prerequisites must be installed. The following are also required:

    * On Linux/macOS, the C++ compiler (`g++`) must be on the path.
    * The C++ compiler (`cl.exe` or `g++`) and `dotnet` must be the same bitness (32-bit versus 64-bit).
      * On Windows, the sample is set up to use the bitness of `dotnet` to find the corresponding `cl.exe`

1) Navigate to the root directory and run `dotnet build`

1) Run the samples. Do one of the following:

    * Use `dotnet run` (which will build and run at the same time).
    * Use `dotnet build` to build the executable. The executable will be in `bin` under a subdirectory for the configuration (`Debug` is the default).
        * Windows: `bin\Debug\MarshalingSample.exe`
        * Non-Windows: `bin/Debug/MarshalingSample`

Note: The way the sample is built is relatively complicated. The goal is that it's possible to build and run the sample with simple `dotnet run` with minimal requirements on pre-installed tools. Typically real-world projects which have both managed and native components will use different build systems for each; for example msbuild/dotnet for managed and CMake for native.

## Visual Studio support

The `src\MarshalingSample.sln` can be used to open the sample in Visual Studio 2019. In order to be able to build from Visual Studio, though, it has to be started from the correct developer environment. From the developer environment console, start it with `devenv src\MarshalingSample.sln`. With that, the solution can be built. To run it, set the start project to `MarshalingSample`.
