---
languages:
- csharp
products:
- dotnet
page_type: sample
name: "iOS Sample: Simple greeting and counter (C#)"
description: "An iOS application that contains an example of embedding the mono runtime to invoke unmanaged code with C#."
urlFragment: "mono-ios-csharp"
---

# iOS Sample: Simple greeting and counter (C#)

In this sample, the mono runtime is used to invoke Objective-c unmanaged code (main.m) from the C# managed side (iOSSampleApp.cs) and vice versa. With the sample running, you can enter your name and click the corresponding button to modify the greeting message as well as clicking a button to increment a counter.

> [!NOTE]
> The purpose of this sample is to demonstrate the concept of building an iOS application on top of the mono runtime. The mono runtime headers should be supplied through the build process.

## Sample Prerequisites

This sample will only run on macOS as it requires Xcode and an iOS simulator.

- Xcode: Any version should work with this sample (download Xcode at <https://developer.apple.com/xcode/>).
- iOS simulator 8.0 or greater.
- .NET sdk 6.0.100-alpha.1.20628.2.

To install a specific version of the dotnet sdk, download the latest stable version of the dotnet-install script:

- Bash: <https://dot.net/v1/dotnet-install.sh>

Install version .NET version **6.0.100-alpha.1.20628.2**:

```bash
./dotnet-install.sh --version 6.0.100-alpha.1.20628.2
```

> [!NOTE]
> Modify `IosSimulator` under target `BuildAppBundle` from `iPhone 11` to your simulator's device name.

## Building the sample

The source code includes an MSBuild project file for C# (a _.csproj_ file) that targets .NET 6.0. After downloading the _.zip_ file, be sure to have the iOS simulator open and modify `iPhone 11` in `iOSSampleApp.csproj` to the simulator's name. To run the sample, open the command line, navigate to the downloaded folder, and run `dotnet publish`.

> [!NOTE]
> The on screen keyboard for the device can be minimized by enabling `Connect Hardware Keyboard` under `Keyboard` in the `I/O` section of the simulator's toolbar and toggling `Toggle Software Keyboard` in that same location.
