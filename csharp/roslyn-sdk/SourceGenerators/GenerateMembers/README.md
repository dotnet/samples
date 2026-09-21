---
languages:
- csharp
products:
- dotnet
page_type: sample
name: "Attribute-based Member Generation"
urlFragment: "roslyn-generate-members"
description: "A Roslyn incremental source generator that adds members to a type when a marker attribute is applied."
---

# GenerateMembers – Attribute-based Source Generator

This sample demonstrates how to write an [incremental source generator](https://learn.microsoft.com/dotnet/csharp/roslyn-sdk/source-generators-overview) that generates new members inside a `partial` class or struct when a marker attribute (`[GenerateMembers]`) is applied.

## What it generates

For any partial type decorated with `[GenerateMembers]`, the generator emits:

- **`PropertyNames`** – a static `IReadOnlyList<string>` containing the names of all instance properties.
- **`Describe()`** – an instance method that returns a human-readable description of the object and its property values.

## Project structure

| Project | Description |
|---------|-------------|
| `GenerateMembersGenerator` | The source generator (targets `netstandard2.0`). |
| `GenerateMembersDemo` | A console application that uses the generator. |

## How the generator references work

The demo project references the generator as an analyzer, not a regular project reference:

```xml
<ProjectReference Include="..\GenerateMembersGenerator\GenerateMembersGenerator.csproj"
                  OutputItemType="Analyzer"
                  ReferenceOutputAssembly="false" />
```

## Running

```bash
dotnet run --project GenerateMembersDemo
```

### Expected output

```text
Person
  FirstName = Alice
  LastName = Smith
  Age = 30

Properties:
  FirstName
  LastName
  Age
```
