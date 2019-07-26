.NET Core PInvoke Marshalling Sample
================

This project demonstrates different ways to marshall arguments to native function when using PInvokes. Documentation can be found here for [PInvokes](https://docs.microsoft.com/en-us/dotnet/standard/native-interop/pinvoke) and here for the [Type marshalling](https://docs.microsoft.com/en-us/dotnet/standard/native-interop/type-marshaling).

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
      * On Windows, a [Developer Command Prompt for Visual Studio](https://docs.microsoft.com/cpp/build/building-on-the-command-line#developer_command_prompt_shortcuts) should be used.
    * The C++ compiler (`cl.exe` or `g++`) and `dotnet` must be the same bitness (32-bit versus 64-bit).
      * On Windows, the default developer command prompt for VS uses the 32-bit compilers, but `dotnet` is typically 64-bit by default. Make sure to select the "x64 Native Tools Command Prompt for VS 2019" (or 2017).

1) Navigate to the root directory and run `dotnet build`

1) Run the samples. Do one of the following:

    * Use `dotnet run` (which will build and run at the same time).
    * Use `dotnet build` to build the executable. The executable will be in `bin` under a subdirectory for the configuration (`Debug` is the default).
        * Windows: `bin\Debug\MarshallingSample.exe`
        * Non-Windows: `bin/Debug/MarshallingSample`

Note: The way the sample is built is relatively complicated. The goal is that it's possible to build and run the sample with simple `dotnet run` with minimal requirements on pre-installed tools. Typically real-world projects which have both managed and native components will use different build systems for each; for example msbuild/dotnet for managed and CMake for native.

Visual Studio support
---------------------

The `src\MarshallingSample.sln` can be used to open the sample in Visual Studio 2019. In order to be able to build from Visual Studio, though, it has to be started from the correct developer environment. From the developer environment console, start it with `devenv src\MarshallingSample.sln`. With that, the solution can be built. To run it set the start project to `MarshallingSample`.
