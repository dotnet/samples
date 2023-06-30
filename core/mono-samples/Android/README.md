---
languages:
- csharp
products:
- dotnet
page_type: sample
name: "Android Sample: Simple greeting and counter (C#)"
description: "An Android application that contains an example of embedding the mono runtime to invoke unmanaged code with C#."
urlFragment: "mono-android-csharp"
---

# Android Sample: Simple greeting and counter (C#)

In this sample, the mono runtime is used to invoke C unmanaged code (native-lib.c) from the C# managed side (AndroidSampleApp.cs) and vice versa. Then through the Java Native Interface (JNI), the java code (MainActivity.java) can call and be called by the C unamanged code. With the sample running, you can enter your name and click the corresponding button to modify the greeting message as well as clicking a button to increment a counter.

[!NOTE]
The purpose of this sample is to demonstrate the concept of building an Android application on top of the mono runtime. The mono runtime headers should be supplied through the build process.
If you wish to use your own activity file, set `NativeMainSource` in `AndroidSampleApp.csproj` to point at your activity file, and the class is currently restricted to be called `MainActivity`.
When running an activity in a non-UI thread, `FindClass` in `native-lib.c` will not work.

## Sample Prerequisites

- ANDROID_NDK & ANDROID_SDK (<https://github.com/dotnet/runtime/blob/main/docs/workflow/testing/libraries/testing-android.md> right under `Testing Libraries on Android`).
- Android simulator API 21 or greater.
- Dotnet sdk 6.0.100-alpha.1.20628.2.

To install a specific version of the dotnet sdk, download the latest stable version of the dotnet-install script:

- Bash: <https://dot.net/v1/dotnet-install.sh>

Install version .NET version **6.0.100-alpha.1.20628.2**:

```bash
./dotnet-install.sh --version 6.0.100-alpha.1.20628.2
```

## Building the sample

The source code includes an MSBuild project file for C# (a _.csproj_ file) that targets .NET 6.0. After downloading the _.zip_ file, be sure to have the Android simulator open. Mdify the `TargetArchitecture` property in `AndroidSampleApp.csproj` to your simulator's architecture (x64, x86, arm, arm64). To run the sample, open the command line, navigate to the downloaded folder, and run `dotnet publish`.

[!NOTE]
To view the application's logs, use `<YOUR_ANDROID_SDK_ROOT>\platform-tools\adb logcat -d` after publishing.
