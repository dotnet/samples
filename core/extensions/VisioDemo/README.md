---
languages:
- csharp
products:
- dotnet
page_type: sample
name: "Visio Demo"
urlFragment: "visio-demo"
description: "A sample that demonstrates interoperation with Visio in .NET Core"
---

# Visio Demo

This is a basic Visio demo sample in .NET Core. It is designed to work with .NET Core 3.1. It is based on the Excel Demo sample found here:

- [https://github.com/dotnet/samples/tree/main/core/extensions/ExcelDemo](https://github.com/dotnet/samples/tree/main/core/extensions/ExcelDemo)

## Key Features

Demonstrates how to consume Office Primary Interop Assemblies (PIA) with .NET Core 3.1.

- Embedding of Interop types (i.e. [No-PIA](https://docs.microsoft.com/dotnet/framework/interop/type-equivalence-and-embedded-interop-types)).
- Support for the [`IDispatch`](https://docs.microsoft.com/windows/desktop/winauto/idispatch-interface) interface.

**Note** Adding COM references to .NET Core projects from Visual Studio is not currently supported. The workaround is to create a .NET Framework project, add the COM references, and then copy the relevant `COMReference` elements in the project. See `ExcelDemo.csproj` for further details.

## Build and Run

To build and run the sample, you could load it in Visual Studio 2019 or use MSBuild command line. Build support for `COMReference` elements is not supported in the `dotnet` tool for the Preview 2 release, but the scenario is demoable from within Visual Studio 2019.

To build and run this sample using Visual Studio:

1. Install .NET Core 3.1.

1. Load `VisioDemo.csproj` in Visual Studio 2019.
    - Double click on `VisioDemo.csproj` in File Explorer.

    or

    - Open a Developer Command prompt and open with `devenv.exe VisioDemo.csproj`.

1. Press <kbd>F5</kbd> to build and debug the project.

To build and run this sample using MSBuild command line:

1) Install .NET Core 3.1.

1) Open a Developer Command prompt in the sample directory

1) Enter the following commands

```cmd
msbuild -t:Restore;Build VisioDemo.csproj
bin\Debug\netcoreapp3.1\VisioDemo.exe
```
