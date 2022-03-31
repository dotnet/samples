---
languages:
- csharp
products:
- dotnet
page_type: sample
name: "Logging in .NET"
urlFragment: "csharp-logging-fundamentals"
description: "Several .NET 5 console applications that contain sample source code for interacting with ILogger, and various logging providers."
---

# Logging in .NET sample source code

There are five sample source code projects in this collection of samples. The samples are written in C# and the content of the related docs is covered in [Logging in .NET][logging] articles. In addition to an overview, there are in-depth articles discussing logging providers, compile-time logging source generation, details for implementing a custom logging provider, information on high-performance logging, and console log formatting.

## Sample prerequisites

The samples are written in C# and targets .NET 6. It requires the [.NET 6.0 SDK](https://dotnet.microsoft.com/download/dotnet/6.0) or later.

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

- [Logging in .NET][logging]
- [Logging providers in .NET][logging-providers]
- [Compile-time logging source generation][logger-message-generator]
- [Implement a custom logging provider in .NET][custom-logging-provider]
- [High-performance logging in .NET][high-performance-logging]
- [Console log formatting][console-log-formatter]

[logging]: https://docs.microsoft.com/dotnet/core/extensions/logging
[logging-providers]: https://docs.microsoft.com/dotnet/core/extensions/logging-providers
[logger-message-generator]: https://docs.microsoft.com/dotnet/core/extensions/logger-message-generator
[custom-logging-provider]: https://docs.microsoft.com/dotnet/core/extensions/custom-logging-provider
[high-performance-logging]: https://docs.microsoft.com/dotnet/core/extensions/high-performance-logging
[console-log-formatter]: https://docs.microsoft.com/dotnet/core/extensions/console-log-formatter
