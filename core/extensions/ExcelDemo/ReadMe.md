Excel Demo
================

This is a basic Excel demo sample in .NET Core. It is designed to work with .NET Core 3.0. It is based on the [How to automate Microsoft Excel from Microsoft Visual C#.NET](https://support.microsoft.com/en-us/help/302084/how-to-automate-microsoft-excel-from-microsoft-visual-c-net) sample for .NET Framework.

Key Features
------------

Demonstrates how to consume Office Primary Interop Assemblies (PIA) with .NET Core 3.0 Preview 1.

- Embedding of Interop types (i.e. [No-PIA](https://docs.microsoft.com/en-us/dotnet/framework/interop/type-equivalence-and-embedded-interop-types)).
- Support for the [`IDispatch`](https://docs.microsoft.com/en-us/windows/desktop/winauto/idispatch-interface) interface.

**Note** Adding COM references to .NET Core projects from Visual Studio is not currently supported. The workaround is to create a .NET Framework project, add the COM references, and then copy the relevant `COMReference` elements in the project. See `ExcelDemo.csproj` for further details.

Build and Run
-------------

To build and run the sample, the project must be loaded in Visual Studio 2017. Developer support in `dotnet` is not complete for the Preview 1 release, but the scenario is demoable from within Visual Studio 2017.

1) Install .NET Core 3.0 Preview 1.

1) Load `ExcelDemo.csproj` in Visual Studio 2017.

1) Press F5 to build and debug the project.
