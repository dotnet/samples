---
languages:
- csharp
products:
- dotnet
page_type: sample
name: "COM Server Demo"
urlFragment: "com-server-demo"
description: "A basic example of a managed COM server in .NET Core"
---

# COM Server Demo

This is a basic example of providing a managed COM server in .NET Core 3.1. Documentation on the inner workings of activation can be found [here](https://github.com/dotnet/runtime/blob/main/docs/design/features/COM-activation.md).

## Key Features

Demonstrates how to provide a COM server in .NET Core 3.1 or later.

Additional comments are contained in source and project files.

## Build and Run

The project will only build and run on the Windows platform. You can build and run the example either by registering the COM server or by using registration-free COM.

### Registered COM

1. Install .NET Core 3.1 or later.

1. Navigate to the root directory and run `dotnet.exe build`.

1. Follow the instructions for COM server registration that were emitted during the build.

1. Navigate to `COMClient/` and run `dotnet.exe run`.

Program should output an estimated value of &#960;.

**Note** Remember to unregister the COM server when the demo is complete.

### RegFree COM

1. Install .NET Core 3.1 or later.

1. Navigate to the root directory and run `dotnet.exe build /p:RegFree=True`.

    - If the Registered COM demo was previously run, the project should be cleaned first - `dotnet.exe clean`

1. Run the generated binary directly. For example, `COMClient\bin\Debug\netcoreapp3.1\COMClient.exe`.

Program should output an estimated value of &#960;.

### Default AssemblyLoadContext

1. Install .NET 8 or later.

1. Navigate to the root directory and run `dotnet.exe build /p:DefaultALC=True`.

1. Follow the instructions for COM server registration that were emitted during the build.

1. Run the generated binary directly. For example, `COMClient\bin\Debug\net8.0\COMClient.exe`.

**Note** The RegFree COM scenario requires a customized [application manifest](https://docs.microsoft.com/windows/desktop/sbscs/manifests) in the executing binary. This means that attempting to execute through `dotnet.exe` will not work and instead trigger a rebuild of the project.

**Note** Running the "Registered COM" first and then immediately following it by "RegFree COM" will not work, since the build system will not correctly rebuild all files (the simple property change is not detected as a reason for a full rebuild). To fix this, run `dotnet clean` between the two samples.
