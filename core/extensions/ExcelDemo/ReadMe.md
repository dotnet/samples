Excel Demo
================

This is a basic Excel demo sample in .NET Core. It is designed to work with .NET Core 3.1. It is based on the [How to automate Microsoft Excel from Microsoft Visual C#.NET](https://support.microsoft.com/help/302084/how-to-automate-microsoft-excel-from-microsoft-visual-c-net) sample for .NET Framework.

Key Features
------------

Demonstrates how to consume Office Primary Interop Assemblies (PIA) with .NET Core 3.1.

- Embedding of Interop types (i.e. [No-PIA](https://docs.microsoft.com/dotnet/framework/interop/type-equivalence-and-embedded-interop-types)).
- Support for the [`IDispatch`](https://docs.microsoft.com/windows/desktop/winauto/idispatch-interface) interface.

**Note** Adding COM references to .NET Core projects from Visual Studio is not currently supported. The workaround is to create a .NET Framework project, add the COM references, and then copy the relevant `COMReference` elements in the project. See `ExcelDemo.csproj` for further details.

Build and Run
-------------

To build and run the sample, the project must be loaded in Visual Studio 2019. Build support for `COMReference` elements is not supported in the `dotnet` tool for the Preview 1 release, but the scenario is demoable from within Visual Studio 2017.

1) Install .NET Core 3.1.

1) Load `ExcelDemo.csproj` in Visual Studio 2017.
    - Double click on `ExcelDemo.csproj` in File Explorer.

    or

    - Open a Developer Command prompt and open with `devenv.exe ExcelDemo.csproj`.

1) Press <kbd>F5</kbd> to build and debug the project.
