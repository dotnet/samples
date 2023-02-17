---
languages:
- csharp
products:
- dotnet
page_type: sample
name: "Worker Services in .NET"
urlFragment: "csharp-workers-fundamentals"
description: "Several .NET 6 worker service applications that contain sample source code for interacting with IHostedService, and BackgroundService."
---

# Worker Services in .NET sample source code

There are six sample source code projects in this collection of samples. The samples are written in C# and the content of the related docs is covered in [Worker Services in .NET][workers] articles. In addition to an overview, there are in-depth articles discussing queue service implementations, `BackgroundService` scenarios, custom `IHostedService` implementations, Windows Services interop with the `BackgroundService`, and even deploying a worker to Azure.

## Sample prerequisites

The samples are written in C# and target .NET 7. They require the [.NET 7.0 SDK](https://dotnet.microsoft.com/download/dotnet/7.0) or later.

## Building the sample

To download and run the sample, follow these steps:

1. Download and unzip the sample.
2. In Visual Studio (2022 or later):
    1. On the menu bar, choose **File** > **Open** > **Project/Solution**.
    2. Navigate to the folder that holds the unzipped sample code, and open the C# project (.csproj) file.
    3. Choose the <kbd>F5</kbd> key to run with debugging, or <kbd>Ctrl</kbd>+<kbd>F5</kbd> keys to run the project without debugging.
3. From the command line:
   1. Navigate to the folder that holds the unzipped sample code.
   2. At the command line, type [`dotnet run`](https://docs.microsoft.com/dotnet/core/tools/dotnet-run).

## More information

- [Worker Services in .NET][workers]
- [Create a Queue Service][queue]
- [Use scoped services within a `BackgroundService`][scoped-bgs]
- [Create a Windows Service using `BackgroundService`][win-bgs]
- [Create a Windows Service installer][win-inst]
- [Implement the `IHostedService` interface][timer-svc]
- [Deploy a Worker Service to Azure][cloud-svc]

[workers]: https://learn.microsoft.com/dotnet/core/extensions/workers
[queue]: https://learn.microsoft.com/dotnet/core/extensions/queue-service
[scoped-bgs]: https://learn.microsoft.com/dotnet/core/extensions/scoped-service
[win-bgs]: https://learn.microsoft.com/dotnet/core/extensions/windows-service
[win-inst]: https://learn.microsoft.com/dotnet/core/extensions/windows-service-with-installer
[timer-svc]: https://learn.microsoft.com/dotnet/core/extensions/timer-service
[cloud-svc]: https://learn.microsoft.com/dotnet/core/extensions/cloud-service
