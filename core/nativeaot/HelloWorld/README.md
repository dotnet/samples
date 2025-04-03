# Building a Hello World console app with native AOT

Native AOT is an [optimized .NET runtime deployment model](https://learn.microsoft.com/dotnet/core/deploying/native-aot/). This document will guide you through compiling a .NET Console application with native AOT.

_Please ensure that [pre-requisites](https://learn.microsoft.com/dotnet/core/deploying/native-aot#prerequisites) are installed._

## Create .NET Console project

Open a new shell/command prompt window and run the following commands.

```bash
> dotnet new console -o HelloWorld --aot
> cd HelloWorld
```

This will create a simple Hello World console app in `Program.cs` and associated project files enabled for publishing as native AOT.

## Restore and Publish your app

Once the package has been successfully added it's time to compile and publish your app! In the shell/command prompt window, run the following command:

```bash
> dotnet publish
```

Once completed, you can find the native executable in the root folder of your project under `/bin/<Configuration>/net8.0/<RID>/publish/`. Navigate to `/bin/<Configuration>/net8.0/<RID>/publish/` in your project folder and run the produced native executable.

## Build using a docker container

This sample includes Dockerfiles that demonstrate installing NativeAOT build prerequisites and building in a container:

- Linux x64: `docker build -t hello . & docker run -t hello`
- Windows x64: `docker build -t hello -f Dockerfile.windowsservercore-x64 . & docker run -t hello`

More comprehensive containerized sample app built with native AOT can be found in [dotnet-docker repo](https://github.com/dotnet/dotnet-docker/tree/main/samples/releasesapi).
