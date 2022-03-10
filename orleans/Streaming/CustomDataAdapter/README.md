---
languages:
- csharp
products:
- dotnet
- dotnet-orleans
page_type: sample
name: "Orleans Streaming sample with custom data adapter"
urlFragment: "orleans-streaming-custom-data-adapter"
description: "An Orleans custom data adapter streaming sample."
---

# Orleans Streaming sample with custom data adapter

This sample demonstrates how to use Orleans Streams with a non-Orleans publisher. The external publishes pushes to a stream that is consumed by a grain with the help of a *custom data adapter* which tells Orleans how to interpret stream messages.

## Sample prerequisites

This sample is written in C# and targets .NET 6. It requires the [.NET 6.0 SDK](https://dotnet.microsoft.com/download/dotnet/6.0) or later.

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
