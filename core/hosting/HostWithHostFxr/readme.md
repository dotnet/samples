.NET Core Hosting Sample
================

This project demonstrates a way for a native process to host .NET Core using the `nethost` and `hostfxr` libraries. Documentation on the `nethost` and `hostfxr` APIs can be found [here](https://github.com/dotnet/core-setup/blob/master/Documentation/design-docs/native-hosting.md).

Key Features
------------

Demonstrates how to locate and initialize .NET Core 3.0 from a non-.NET Core process and subsequently load and call into  a .NET Core assembly.

The `nethost` header and library are part of the Microsoft.NETCore.DotNetAppHost package and are also installed as a runtime pack by the .NET SDK. The library should be deployed alongside the host. This sample uses the files installed with the .NET SDK.

The `coreclr_delegates.h` and `hostfxr.h` files are copied from the [core-setup](https://github.com/dotnet/core-setup) repo.

Addtional comments are contained in source and project files.

Prerequisites
------------

* .NET Core 3.0 (at least Preview 6) - [https://dot.net](https://github.com/dotnet/core-sdk#installers-and-binaries)

* C++ compiler
  * Windows: `cl.exe`
  * Linux/OSX: `g++`

Build and Run
-------------

1) In order to build and run, all prerequisites must be installed. The following are also required:

    * The C++ compiler (`cl.exe` or `g++`) must be on the path.
    * The C++ compiler (`cl.exe` or `g++`) and `dotnet` must be the same bitness.

1) Navigate to the root directory and run `dotnet build`.

1) Run the `nativehost` executable.

    * The executable will be in `bin` under a subdirectory for the configuration (`Debug` is the default).
        * Windows: `bin\Debug\nativehost.exe`
        * Non-Windows: `bin/Debug/nativehost`

The expected output will come from the `DotNetLib` class library.
