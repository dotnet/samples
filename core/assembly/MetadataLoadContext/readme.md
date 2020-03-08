---
languages:
- csharp
products:
- dotnet-core
page_type: sample
name: "Inspect assembly contents using MetadataLoadContext"
description: "A sample console application that shows how to use MetadataLoadContext to load an assembly for inspection purposes"
---
# Inspect assembly contents using MetadataLoadContext

This sample is a .NET Core console application that loads the specified assembly and prints its custom attributes and defined types. It demonstrates how to use the [System.Reflection.MetadataLoadContext](https://www.nuget.org/packages/System.Reflection.MetadataLoadContext) API to load an assembly for inspection purposes.

## Build and run the sample

After downloading the source, open the console in the sample directory and use following commands:

```dotnetcli
dotnet build
dotnet run -- <assembly path>
```
