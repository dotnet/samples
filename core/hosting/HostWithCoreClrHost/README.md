.NET Core Hosting Sample
========================

This sample demonstrates a simple .NET Core host using the hosting APIs from [CoreClrHost.h](https://github.com/dotnet/coreclr/blob/master/src/coreclr/hosts/inc/coreclrhost.h). The sample host loads and starts the .NET Core CLR (including starting the default AppDomain), loads managed code, calls into a managed method, and provides a function pointer for the managed code to call back into the host.

This sample is part of the [.NET Core hosting tutorial](https://docs.microsoft.com/dotnet/core/tutorials/netcore-hosting). Please see that topic for a more detailed explanation of this sample. There are also alternative hosting APIs (the `ICLRRuntimeHost2` interface in [mscoree.h](https://github.com/dotnet/coreclr/tree/master/src/pal/prebuilt/inc/mscoree.h)) that are demonstrated in the [HostWithMscoree folder](../HostWithMsCoree) of this repository. 

About .NET Core Hosts
---------------------

.NET Core applications are always run by a host. In most cases, the default dotnet.exe host is used.

It is possible to create your own host, though, to enable starting and running .NET Core code from a native application, or to enable a high degree of control over how the runtime operates. More complex, real-world hosts can be found in the [dotnet/coreclr](https://github.com/dotnet/coreclr/tree/master/src/coreclr/hosts) repository.

This host can be built either for Windows or Linux using the simple build scripts (build.bat and build.sh).

Build and Run
-------------

To build this sample, just use the included build scripts (build.bat or build.sh) to build both the managed target assembly (ManagedLibrary.dll) and the host (SampleHost.exe). The build scripts are just simple wrappers around two build calls (`dotnet publish` for the managed component of the sample and cl/g++ for the host), so it's also easy to build the two components directly if you prefer. Be sure that the bitness of the host and sample app match. By default, the build scripts build ManagedLibrary.dll for x64, so you will need to build the host for 64-bit.

To run the host, just execute SampleHost from the bin/{{OS}} directory.
