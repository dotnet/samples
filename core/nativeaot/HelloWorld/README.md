# Building a Hello World console app with NativeAOT

NativeAOT is an AOT-optimized .NET runtime. This document will guide you through compiling a .NET Console application with NativeAOT.

_Please ensure that [pre-requisites](https://docs.microsoft.com/en-us/dotnet/core/deploying/native-aot#prerequisites) are installed._

## Create .NET Console project

Open a new shell/command prompt window and run the following commands.

```bash
> dotnet new console -o HelloWorld
> cd HelloWorld
```

This will create a simple Hello World console app in `Program.cs` and associated project files.

## Add NativeAOT to your project

Add `<PublishAot>true</PublishAot>` property to your project file. This will produce a native AOT app and show any potential compatibility warnings during the publish process.

## Restore and Publish your app

Once the package has been successfully added it's time to compile and publish your app! In the shell/command prompt window, run the following command:

```bash
> dotnet publish -r <RID> -c <Configuration>
```

where `<Configuration>` is your project configuration (such as Debug or Release) and `<RID>` is the runtime identifier (one of win-x64, linux-x64, osx-x64). For example, if you want to publish a release configuration of your app for a 64-bit version of Windows the command would look like:

```bash
> dotnet publish -r win-x64 -c release
```

Once completed, you can find the native executable in the root folder of your project under `/bin/<Configuration>/net7.0/<RID>/publish/`. Navigate to `/bin/<Configuration>/net7.0/<RID>/publish/` in your project folder and run the produced native executable.

## Build using a docker container

This sample includes Dockerfiles that demonstrate installing NativeAOT build prerequisites and building in a container:

- Linux x64: `docker build -t hello . & docker run -t hello`
- Windows x64: `docker build -t hello -f Dockerfile.windowsservercore-x64 . & docker run -t hello`
