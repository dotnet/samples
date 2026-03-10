---
languages:
- csharp
products:
- dotnet
page_type: sample
name: "CSV to C# Source Generator"
urlFragment: "roslyn-csv-generator"
description: "A Roslyn incremental source generator that compiles CSV files into strongly-typed C# classes at build time."
---

# CsvGenerator – Non-C# File to C# Source Generator

This sample demonstrates how to write an [incremental source generator](https://learn.microsoft.com/dotnet/csharp/roslyn-sdk/source-generators-overview) that reads a non-C# source file (CSV) and generates C# code from it at compile time.

## What it does

The generator scans for `.csv` files registered as `AdditionalFiles` in the project.  For each CSV file it finds, it generates a strongly-typed C# class where:

- The **first row** is treated as column headers, which become property names.
- Each **subsequent row** becomes a static instance accessible through a generated `All` property.
- A `ToString()` override is generated for easy display.

## Project structure

| Project | Description |
|---------|-------------|
| `CsvGenerator` | The source generator (targets `netstandard2.0`). |
| `CsvGeneratorDemo` | A console application that uses the generator with a sample CSV file. |

## How CSV files are exposed to the generator

In the demo project's `.csproj`, CSV files are included as additional files:

```xml
<ItemGroup>
  <AdditionalFiles Include="Data\*.csv" />
</ItemGroup>
```

## Running

```bash
dotnet run --project CsvGeneratorDemo
```

### Expected output

```text
=== Cities loaded from CSV ===

Name=Tokyo, Country=Japan, Population=13960000
Name=Delhi, Country=India, Population=11030000
Name=Shanghai, Country=China, Population=24870000
Name=São Paulo, Country=Brazil, Population=12330000
Name=Mexico City, Country=Mexico, Population=9210000

Total cities: 5
```
