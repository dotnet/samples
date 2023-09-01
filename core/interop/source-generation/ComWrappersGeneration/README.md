---
languages:
- csharp
products:
- dotnet
page_type: sample
name: "Source-Generated COM Sample"
urlFragment: "generated-comwrappers"
description: "A .NET solution that uses source-generated COM in .NET"
---

# .NET Source-Generated COM Sample

This project demonstrates how to use the COM source generators in the .NET SDK to expose .NET objects to COM and import COM objects to .NET, with source-generated parameter marshalling. This tutorial compiles the projects to a NativeAOT application to demonstrate the compatibility of source-generated COM, and to export native methods from the Server project that the Windows COM system requires.

This example defines an interface `ICalculator` that provides `Add` and `Subtract` methods. The server provides an implementation of `ICalculator` and exposes it to COM through registration. The client project creates an instance of the object through the `CoCreateInstance` Win32 method, and calls methods on the object.

## Prerequites

- .NET 8 SDK
- Windows device

## Build and Run

Build the Native AOT binaries by running `dotnet publish -r <RID>` where `<RID>` is the RuntimeIdentifier for your windows device, for example `win-x64`. The projects will copy the binaries to the OutputFiles directory. After publishing, use `regsvr32.exe` to register Server.dll to COM (run `regsvr.exe ./OutputFiles/Server.dll`). Then, run `./OutputFiles/Client.exe` and see the output as it gets a COM object from the Server.dll and calls methods on it. Once finished, unregister the server by running `regsvr32.exe -u ./OutputFiles/Server.dll`.
