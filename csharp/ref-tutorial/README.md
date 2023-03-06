---
languages:
- csharp
products:
- dotnet
page_type: sample
name: "Reduce memory allocations with `ref` safety"
urlFragment: "performance-allocations"
description: ".NET Console applications that demonstrate performance tuning by reducing allocations and copies. Companion to /docs/dotnet/csharp/advanced-topics/performance/ref-tutorial"
---
# Tutorial: Reduce memory allocations with `ref` safety

Often, performance tuning for a .NET application involves two techniques. First, reduce the number and size of heap allocations. Second, reduce how often data is copied. Visual Studio provides great [tools](/visualstudio/profiling/dotnet-alloc-tool) that help analyze how your application is using memory. Once you've determined where your app makes unnecessary allocations, use can make use of the `ref` safety [features](index.md) to minimize allocations.

The application uses a simulation of an IoT sample with several sensors to determine if an intruder has entered a secret gallery with valuables. The IoT sensors are constantly sending data that measures the mix of Oxygen (O2) and Carbon Dioxide (CO2) in the air. They also report the temperature and relative humidity. Each of these values are fluctuating slightly all the time. However, when a person enters the room, they change a bit more, and always in the same direction: Oxygen decreases, Carbon Dioxide increases, temperature increases, as does relative humidity. When the sensors combine to show increases, the intruder alarm is triggered.

The starter application works correctly, but because it allocates a number of small objects with each measurement cycle, its performance slowly degrades as it runs over time. In this tutorial, you'll run the application, take measurements on memory allocations, then improve the performance by reducing the number of allocations.
