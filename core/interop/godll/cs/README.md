# Golang-.NET-SharedC
A simple example of using golang's cgo library to build shared-c libraries from go code and import exported function entrypoints into C# code.

## Building and Running
You must have Golang installed on your machine to build the Golang executables.  If you do not have Golang installed, check out [install Golang](https://golang.org/dl/ "Installing Golang") and for Windows machines, download and install the Microsoft Windows `.msi`.  Follow the installation instructions on the [download page](https://golang.org/doc/install?download=go1.12.4.windows-amd64.msi "The Go Programming Language: Getting Started").

### Prerequisites for building in Windows
In Windows we need a runtime environment for GCC to support binaries native to Windows 64/32-bit operating systems.  The recommended tool for this is [MinGW](http://www.mingw.org/ "MinGW").  MinGW (Minimalist GNU for Windows) is a minimalist development environment for native Microsoft Windows applications.  Install MinGW [here](https://sourceforge.net/projects/mingw-w64/ "MinGW for windows download"), and follow these [installation instructions](https://code.visualstudio.com/docs/cpp/config-mingw "MinGW installation instructions for Windows").  You only need to follow instruction through the "Prerequisites" section.  Make sure that when you install, you install for your specific architecture (i.e. x86_64 (AMD64)). 

### Building the Shared Libraries
To build the Goland shared-C libraries, you must use the Makefile.  You can do this by navigating into `go/src/github.com/johncburns1/shared-c` and run the `make` command.
This will build the libraries and add them to `<ProjectRoot>/SharedC/bin/Debug/netcoreapp3.0`.  This means that you need to run the solution in `Debug` mode for `Any CPU`.
You can change where the libraries are built in the Golang `Makefile` by changing `BUILD_DIR`.  By default, the `Makefile` is configured to build only the Windows `.dll`, but this can be changed by specifying the `make` target command.  Once the libraries are built, just run the solution with the correct configurations!

