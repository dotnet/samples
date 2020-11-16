---
languages:
- csharp
products:
- dotnet-core
page_type: sample
name: "Wasm Sample: Simple greeting and counter (C#)"
description: "A Browser WebAssembly application that contains an example of embedding the mono runtime to invoke unmanaged code with C#."
urlFragment: "mono-wasm-csharp"
---

# Wasm Sample: Simple greeting and counter (C#)

In this sample, the mono runtime is used to invoke javascript unmanaged code (index.html) from the C# managed side (WasmSampleApp.cs) and vice versa. With the sample running, you can enter your name and click the corresponding button to modify the greeting message as well as click another button to increment a counter.

> [!NOTE]
> The purpose of this sample is to demonstrate the concept of building a WebAssembly application on top of the mono runtime. The mono runtime headers should be supplied through the build process.

## Sample Prerequisites

This sample will only run on macOS.

- Python 2 or 3
- Working browser
- .NET sdk 6.0.100-alpha.1.20531.2 (Installation instructions in parent directory).

To install a specific version of the dotnet sdk, download the latest stable version of the dotnet-install script:

- Bash (Linux/macOS): <https://dot.net/v1/dotnet-install.sh>
- PowerShell (Windows): <https://dot.net/v1/dotnet-install.ps1>

Install version .NET version **6.0.100-alpha.1.20531.2**:

```bash
./dotnet-install.sh --version 6.0.100-alpha.1.20531.2
```

```powershell
./dotnet-install.ps1 -Version 6.0.100-alpha.1.20531.2
```

> [!NOTE]

## Building the sample

The source code includes an MSBuild project file for C# (a _.csproj_ file) that targets .NET 6.0. After downloading the _.zip_ file, publish the sample by opening the command line, navigating to the downloaded folder, and run `dotnet publish`. To run the sample, navigate to the publish directory `<sample_folder>/bin/<configuration>/<TargetFramework>/browser-wasm/publish/` (e.g. `<sample_folder>/bin/Release/net6.0/browser-wasmpublish`), run `python server.py`, and open `<http://localhost:8000/>` on your browser.
