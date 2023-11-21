---
languages:
- csharp
products:
- dotnet
page_type: sample
name: "Source-Generated COM Sample"
urlFragment: "generated-comwrappers"
description: "A .NET codebase that uses source-generated COM in .NET"
---
# .NET Source-Generated COM Sample

This tutorial demonstrates how to use COM source generators in .NET 8+ to create a COM server and client for in-process COM.

This example defines an interface `ICalculator` that provides `Add` and `Subtract` methods. The server provides an implementation of `ICalculator` for the client to use after the server has been registered. The client project creates an instance of the object using the [`CoCreateInstance`](https://learn.microsoft.com/windows/win32/api/combaseapi/nf-combaseapi-cocreateinstance) Win32 method, and calls methods on the object.

This sample supports NativeAOT and standard CoreCLR deployments. The native methods that the Windows COM system requires are exported automatically with the `[UnmanagedCallersOnly]` attribute when publishing with NativeAOT. For CoreCLR, the [DNNE](https://github.com/AaronRobinsonMSFT/DNNE) package is used to provide the exported functions.

## Prerequisites

- .NET 8+ SDK
- Windows 10+ OS

## Build and Run

### NativeAOT

Build the Native AOT binaries by running `dotnet publish -r <RID>` where `<RID>` is the RuntimeIdentifier for your OS, for example `win-x64`. The projects will copy the binaries to the `OutputFiles\` directory. After publishing, use `regsvr32.exe` to register `Server.dll` with COM by running run `regsvr.exe .\OutputFiles\Server\Server.dll`. Then, run the client application `.\OutputFiles\Client\Client.exe` and observe the output as it activates and uses a COM instance from `Server.dll`.

### CoreCLR

Build the projects by running `dotnet publish`. The projects will copy the binaries to the `OutputFiles\` directory. After publishing, use `regsvr32.exe` to register the native binary produced by DNNE, `ServerNE.dll` by running `regsvr.exe .\OutputFiles\Server\ServerNE.dll`. `ServerNE.dll` will start the .NET runtime and call the exported methods in the managed `Server.dll` which register the server with COM. Then, run the client application `.\OutputFiles\Client\Client.exe` and observe the output as it activates and uses a COM instance from `ServerNE.dll`.

## See also

- [ComWrappers source generation](https://learn.microsoft.com/dotnet/standard/native-interop/comwrappers-source-generation)
- [Native exports in NativeAOT](https://learn.microsoft.com/dotnet/core/deploying/native-aot/interop#native-exports)
- [COM interop in .NET](https://learn.microsoft.com/dotnet/standard/native-interop/cominterop)
