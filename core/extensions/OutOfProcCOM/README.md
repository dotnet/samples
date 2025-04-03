---
languages:
- csharp
- cpp
products:
- dotnet
page_type: sample
name: "Out-of-process COM Server Demo"
urlFragment: "out-of-process-com-server"
description: "An implementation of an out-of-process COM server in .NET Core."
---

# Out-of-process COM Server Demo

This sample demonstrates a way to create an out-of-process COM server in .NET Core 3.1 or later. The .NET SDK and runtime support [exposing an in-process COM server](https://docs.microsoft.com/dotnet/core/native-interop/expose-components-to-com). While there is no built-in support for exposing an out-of-process COM server, it is possible to achieve.

The projects in this sample show a way to provide an out-of-process COM server with the system-supplied [DLL Surrogate](https://docs.microsoft.com/windows/win32/com/dll-surrogates) or using an executable built by the developer. Since .NET Core does not support generating a [type library](https://docs.microsoft.com/windows/win32/midl/com-dcom-and-type-libraries#type-library) from a .NET Core assembly, this sample uses [MIDL](https://docs.microsoft.com/windows/win32/com/midl-compilation) to compile an IDL file into a type library, which the COM server then [registers](https://docs.microsoft.com/windows/win32/com/loading-and-registering-a-type-library).

## Prerequisites

This sample will only build and run on the Windows platform.

* [.NET Core 3.1 SDK](https://dotnet.microsoft.com/download) or later
* C++ build tools for Windows ([installation instructions](https://docs.microsoft.com/cpp/build/building-on-the-command-line#download-and-install-the-tools))

## Build and Run

1. Open a [Developer Command Prompt for Visual Studio](https://docs.microsoft.com/cpp/build/building-on-the-command-line#developer_command_prompt_shortcuts).
1. Navigate to the root directory and build the solution:
    * `msbuild OutOfProcCOM.sln -restore`.
1. Show the instructions for COM server registration:
    * DLL surrogate: `dotnet msbuild -target:ServerUsage DllServer`
    * Executable server: `dotnet msbuild -target:ServerUsage ExeServer`
1. Follow the instructions for registering the server.
1. Run the client:
    * Native: run the `NativeClient.exe` binary
        * Example: `x64\Debug\NativeClient.exe`
    * Managed: run the `ManagedClient.exe` binary
        * Example: `ManagedClient\bin\Debug\netcoreapp3.1\ManagedClient.exe`

The client program should output an estimated value of &#960;:

```
π = 3.140616091322624
```

### Embedded Type Library

The [.NET 6.0 SDK](https://dotnet.microsoft.com/download) (Preview 5 or later) supports [embedding type libraries into the COM DLL](https://docs.microsoft.com/dotnet/core/native-interop/expose-components-to-com#embedding-type-libraries-in-the-com-host). To use this functionality for the DLL surrogate (`DllServer`) in this sample, build with `-p:EmbedTypeLibrary=true`.

**Note:** Remember to unregister the COM server when the demo is complete.
