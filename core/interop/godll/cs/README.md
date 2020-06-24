---
languages:
- csharp
- go
products:
- dotnet-core
page_type: sample
name: "Calling Go functions from C# code"
urlFragment: "golang-shared-c-dotnet-cs"
description: "A simple example of using golang's cgo library and .NET's P/Invoke to call Go functions from C# code like a C-style API."
---

# Calling Go functions from C# code

Go has a library called [cgo](https://golang.org/cmd/cgo/) that enables the creation of Go packages that call C code.  One of cgo's coolest features is that it enables Go packages to be compiled into C shared libraries, exposing Go functions as a C-style APIs.  This sample shows how we can compile a Go project into a Shared C **.dll** and call Go functions from within a .NET Core project using .NET's [P/Invoke](https://docs.microsoft.com/en-us/dotnet/standard/native-interop/pinvoke).

This project consists of 2 primary components:

1. The .NET Core app that imports and calls functions from the Shared C libraries.  This code is found in `<SolutionDir>/SharedC/Program.cs`.
2. The Go packages that define the imported functions and are compiled into the Shared C libraries.  This code is located in `<SolutionDir>/Go/src/shared-c/cmd/*`.  There are 2 Go projects, **helloworld** and **math**.  Each deal with different use cases and exhibits how to write C-style APIs in Go that handle a variety of data types and situations.

Another part of the repo is the **GoBuilder** project.  This lives in  `<SolutionDir>/Go/` and is responsible for automating the building of the Go libraries.  On build, **GoBuilder** executes a build-all powershell script that builds all of the Go packages in the repo.  If errors occur when building the Go libraries the Go build logs can be found in  `<SolutionDir>/Go/GoBuilder/bin/<BuildConfig>/netstandard2.0`.

## Building and Running

### Prereqs

* **.NET Core 3.1** - [download](https://dotnet.microsoft.com/download/dotnet-core/3.1)

* **Golang v1.12+** - You must have Golang installed on your machine to build the Golang executables.  If you do not have Golang installed, check out [install Golang](https://golang.org/dl/ "Installing Golang") and for Windows machines, download and install the Microsoft Windows **.msi**.  Follow the installation instructions on the [download page](https://golang.org/doc/install?download=go1.14.4.windows-amd64.msi "The Go Programming Language: Getting Started").

* **GCC Compiler** - In Windows we need a runtime environment for GCC to support binaries native to Windows 64/32-bit operating systems.  The recommended tool for this is [MinGW](http://www.mingw.org/ "MinGW").  MinGW (Minimalist GNU for Windows) is a minimalist development environment for native Microsoft Windows applications.  Install MinGW [here](https://sourceforge.net/projects/mingw-w64/ "MinGW for windows download"), and follow these [installation instructions](https://code.visualstudio.com/docs/cpp/config-mingw "MinGW installation instructions for Windows").  You only need to follow instruction through the "Prerequisites" section.  Make sure that when you install, you install for your specific architecture (i.e. x86_64 (AMD64)).  You will need to find and add **gcc.exe** to your **PATH**.

### Building the Shared Libraries

Before running the **SharedC** project you must build the solution first.  This will automatically build the 2 Go packages used in this sample (**helloworld** and **math**) and add them to `<ProjectRoot>/SharedC/bin/<BuildConfig>/netcoreapp3.1`.  By default, the project is configured to build only the Windows **.dll's**.  If you want to build the Go libraries on a Linux machine you will need to change the target operating system in the **GoBuilder** powershell scripts.  One of the pitfalls of cgo is cross platform compilation, it is not super simple.  There are ways to make cross compilation work with cgo and if you are interested in cgo cross platfrom compilation [this article](https://www.bluematador.com/blog/golang-pros-cons-part-5-cross-platform-compiling) will help with the proper environment configurations.

### Running the Project

Once the solution has been successfully built, all you need to do is run the `SharedC` project in **VisualStudio**.  A console will pop up, execute the imported Go functions, and output the results.  You can debug the `Program.cs` as well if you want to step through and get a better idea of what is going on.
