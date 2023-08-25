---
languages:
- csharp
- cpp
products:
- dotnet
page_type: sample
name: "Custom Marshalling Source Generation"
urlFragment: "custom-marshalling-source-generation"
description: "A .NET application that demonstrates using the custom marshalling mechanism in interop source generation."
---

# Custom marshalling source generation

The ability to use [source generation for P/Invokes](https://docs.microsoft.com/dotnet/standard/native-interop/pinvoke-source-generation) was introduced in .NET 7. This also included a mechanism for [custom marshalling of types](https://docs.microsoft.com/dotnet/standard/native-interop/custom-marshalling-source-generation).

This sample implements and uses custom marshallers for a built-in type and a user-defined type, including both stateless and stateful marshallers. It demonstrates the usage of attributes relevant to source generation for interop:

- [`LibraryImportAttribute`](https://docs.microsoft.com/dotnet/api/system.runtime.interopservices.libraryimportattribute)
- [`CustomMarshallerAttribute`](https://docs.microsoft.com/dotnet/api/system.runtime.interopservices.marshalling.custommarshallerattribute)
- [`MarshalUsingAttribute`](https://docs.microsoft.com/dotnet/api/system.runtime.interopservices.marshalling.marshalusingattribute)
- [`NativeMarshallingAttribute`](https://docs.microsoft.com/dotnet/api/system.runtime.interopservices.marshalling.nativemarshallingattribute)

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download) Preview 7 or later

- C++ compiler
  - Windows: `cl.exe`
    - See [installation instructions](https://docs.microsoft.com/cpp/build/building-on-the-command-line#download-and-install-the-tools).
  - Linux/macOS: `g++`

## Build and Run

1) In order to build and run, all prerequisites must be installed. The following are also required:

    - On Linux/macOS, the C++ compiler (`g++`) must be on the path.
    - The C++ compiler (`cl.exe` or `g++`) and `dotnet` must be the same bitness (32-bit versus 64-bit).
      - On Windows, the sample is set up to use the bitness of `dotnet` to find the corresponding `cl.exe`

1) Navigate to the root directory and run `dotnet build`

1) Run the samples. Do one of the following:

    - Use `dotnet run` (which will build and run at the same time).
    - Use `dotnet build` to build the executable. The executable will be in `bin` under a subdirectory for the configuration (`Debug` is the default).
        - Windows: `bin\Debug\custommarshalling.exe`
        - Non-Windows: `bin/Debug/custommarshalling`

Note: The way the sample is built is relatively complicated. The goal is that it's possible to build and run the sample with a simple `dotnet run` command with minimal requirements for pre-installed tools. Typically, real-world projects that have both managed and native components will use different build systems for each; for example, msbuild/dotnet for managed and CMake for native.

## Visual Studio support

The `src\custommarshalling.sln` can be used to open the sample in Visual Studio 2022. In order to be able to build from Visual Studio, though, it has to be started from the correct developer environment. From the developer environment console, start it with `devenv src\custommarshalling.sln`. With that, the solution can be built. To run it, set the start project to `custommarshalling`.
