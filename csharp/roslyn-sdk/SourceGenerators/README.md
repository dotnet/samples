---
languages:
- csharp
products:
- dotnet
page_type: sample
name: "Roslyn Source Generator Samples"
urlFragment: "roslyn-source-generators"
description: "Sample Roslyn incremental source generators demonstrating attribute-based member generation and non-C# file compilation."
---

# Roslyn Source Generator Samples

This folder contains sample [incremental source generators](https://learn.microsoft.com/dotnet/csharp/roslyn-sdk/source-generators-overview) built with the Roslyn compiler platform.

## Samples

| Sample | Description |
|--------|-------------|
| [GenerateMembers](GenerateMembers/) | Generates new members (`Describe()` method and `PropertyNames` list) inside a type when a marker attribute is applied. |
| [CsvGenerator](CsvGenerator/) | Reads `.csv` additional files at compile time and generates strongly-typed C# classes from them. |

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) or later

## Building and running

Open the solution in Visual Studio 2022 or later, or build from the command line:

```bash
dotnet build SourceGenerators.sln
```

Then run either demo project:

```bash
dotnet run --project GenerateMembers/GenerateMembersDemo
dotnet run --project CsvGenerator/CsvGeneratorDemo
```
