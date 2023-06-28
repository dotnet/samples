---
languages:
- csharp
products:
- dotnet
page_type: sample
name: ".NET Framework and .NET Core COM interoperability"
urlFragment: "net-framework-net-core-com-interop"
description: "A sample that shows how to interoperate between .NET Framework and .NET Core with COM interop"
---

# .NET Framework and .NET Core COM interoperability

This sample shows how to interoperate between .NET Framework and .NET Core with COM interop.

## To Build

To test calling into a .NET Core COM server, `dotnet build NetSxS.sln`. If you want to test calling into a .NET Framework COM server, then run `dotnet build /p:NetFXServer=true`.

## To Run

To run the client, run the `Client.exe` file in the output directory of whichever client runtime you want to run the client on.
