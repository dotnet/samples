---
languages:
- csharp
- cpp
products:
- dotnet
page_type: sample
name: "C++/CLI Library Demo"
urlFragment: "cpp-cli-library"
description: "Usage of a C++/CLI library in .NET"
---

# C++/CLI Library Demo

This sample demonstrates usage of a [C++/CLI library in .NET](https://docs.microsoft.com/dotnet/core/porting/cpp-cli). It contains the following projects:

- `ManagedApp`: managed (.NET) application - uses `MixedLibrary`
- `ManagedLibrary`: managed (.NET) library
- `MixedLibrary`: C++/CLI library - depends on `ManagedLibrary`
- `NativeApp`: native (C++) application - uses `MixedLibrary`

The `ManagedApp` consumes `MixedLibrary` by directly referencing it to instantiate a managed class and by [p/invoking](https://docs.microsoft.com/dotnet/standard/native-interop/pinvoke) into a native entry point.

The `NativeApp` consumes `MixedLibrary` via an import library and by explicitly loading it. This starts the .NET runtime and loads the assembly. Documentation on the details for activation of C++/CLI assemblies can be found [here](https://github.com/dotnet/runtime/tree/main/docs/design/features/IJW-activation.md#ijw-dlls-and-delayed-activation-thunks).

## Prerequisites

This sample will only build and run on the Windows platform.

- [.NET 5.0 SDK](https://dotnet.microsoft.com/download) or later
- [Visual Studio 2019 16.8](https://visualstudio.microsoft.com/downloads/) or later

## Build and Run

1. Open a [Developer Command Prompt for Visual Studio](https://docs.microsoft.com/cpp/build/building-on-the-command-line#developer_command_prompt_shortcuts).
1. Navigate to the root directory and build the solution:
    - `msbuild CPP-CLI.sln -restore`.
1. Run the app:
    - Managed: run the `ManagedApp.exe` binary
        - Example: `bin\Debug\x64\ManagedApp.exe`
    - Native: run the `NativeApp.exe` binary
        - Example: `bin\Debug\x64\NativeApp.exe`

The expected output will come from classes in the C++/CLI library and include a message passed from the app to the library:

For `ManagedApp`:

```
=== Managed class ===
Hello from ManagedClass in MixedLibrary
Hello from NativeClass in MixedLibrary
-- message: from managed app!

=== P/Invoke ===
Hello from NativeEntryPoint_CallNative in MixedLibrary
Hello from NativeClass in MixedLibrary
-- message: from managed app!
```

For `NativeApp`:

```
=== Import library ===
Hello from NativeEntryPoint_CallManaged in MixedLibrary
Hello from ManagedLibrary.Greet
-- message: from native app!

=== LoadLibrary ===
Hello from NativeEntryPoint_CallManaged in MixedLibrary
Hello from ManagedLibrary.Greet
-- message: from native app!
```
