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

Go has a library called [cgo](https://golang.org/cmd/cgo/) that enables the creation of Go packages that call C code.  One of cgo's coolest features is that it enables Go packages to be compiled into C shared libraries, exposing Go functions as a C-style APIs.  This sample shows how we can compile a Go project into a Shared C **.dll** and call Go functions from within .NET Core project using .NET's [P/Invoke](https://docs.microsoft.com/en-us/dotnet/standard/native-interop/pinvoke).

This project consists of 2 primary components:

1. The .NET Core app that imports and calls functions from the Shared C libraries.  This code is found in `<SolutionDir>/SharedC/Program.cs`.
2. The Go packages that define the imported functions and are compiled into the Shared C libraries.  This code is located in `<SolutionDir>/Go/src/shared-c/cmd/*`.  There are 2 Go projects, **helloworld** and **math**.  Each deal with different use cases and exhibits how to write C-style APIs in Go that handle a variety of data types and situations.

Another part of the repo is the `GoBuilder` project.  This lives in  `<SolutionDir>/Go/` and solely responsible for automating the building of the Go libraries when the solution is built.  If errors occur when building the Go libraries the Go build logs can be found in  `<SolutionDir>/Go/GoBuilder/bin/<BuildConfig>/netstandard2.0`.

## Building and Running

### Prereqs

You must have Golang installed on your machine to build the Golang executables.  If you do not have Golang installed, check out [install Golang](https://golang.org/dl/ "Installing Golang") and for Windows machines, download and install the Microsoft Windows **.msi**.  Follow the installation instructions on the [download page](https://golang.org/doc/install?download=go1.14.4.windows-amd64.msi "The Go Programming Language: Getting Started").

### Building the Shared Libraries

If you want to run the **SharedC** project you have to build the solution first.  This will automatically build the 2 Go packages used in this sample (**helloworld** and **math**).
This will build the libraries and add them to `<ProjectRoot>/SharedC/bin/<BuildConfig>/netcoreapp3.1`.  By default, the project is configured to build only the Windows **.dll's**.  If you want to build the Go libraries for on a Linux machine you will need to change the target operating system in the `GoBuilder` powershell scripts.  These can be changed via the `GOOS` environment vairable.  One of the pitfalls of cgo is cross platform compilation, it is not super simple.  The reason This is because cgo in Linux uses a different C compiler than stock Windows.  If you are interested in cgo cross platfrom compilation [this article] will help with the proper compiler configurations.

### Running the Project

Once the solution has been successfully built, all you need to do is run the `SharedC` project in **VisualStudio**.  A console will pop up and output the results of executing the imported Go functions.  These can be seen in `Program.cs`.
