---
languages:
- csharp
products:
- dotnet
page_type: sample
name: "Wasm Sample: Simple greeting and counter (C#)"
description: "A Browser WebAssembly application that contains an example of embedding the mono runtime to invoke unmanaged code with C#."
urlFragment: "mono-wasm-csharp"
---

# Wasm Sample: Simple greeting and counter (C#)

In this sample, the mono runtime is used to invoke JavaScript unmanaged code (index.html) from the C# managed side (WasmSampleApp.cs) and vice versa. With the sample running, you can enter your name and click the corresponding button to modify the greeting message as well as click another button to increment a counter.

> [!NOTE]
> The purpose of this sample is to demonstrate the concept of building a WebAssembly application on top of the mono runtime. The mono runtime headers should be supplied through the build process.

## Sample Prerequisites

This sample will only run on macOS.

- Working browser
- .NET sdk 6.0.100-alpha.1.20628.2.
- dotnet-serve (<https://github.com/natemcmain/dotnet-serve>)

To install a specific version of the .NET SDK, download the latest stable version of the dotnet-install script:

- Bash: <https://dot.net/v1/dotnet-install.sh>

Install version .NET version **6.0.100-alpha.1.20628.2**:

```bash
./dotnet-install.sh --version 6.0.100-alpha.1.20628.2
```

## Building the sample

The source code includes an MSBuild project file for C# (a _.csproj_ file) that targets .NET 6.0. After downloading the _.zip_ file, run the sample by opening the command line, navigating to the downloaded folder, and run `dotnet publish`.

> [!NOTE]
> Before running `dotnet publish`s in succession, remove both the `bin/` and `obj/` folders. Otherwise, `System.IO.IOException: The process cannot access the file ... because it is being used by another process.` may result.
