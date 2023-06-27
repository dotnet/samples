---
languages:
- csharp
products:
- aspnet-core
- azure
page_type: sample
name: "Azure Monitor Application Insights Quickstart for ASP.NET Core"
urlFragment: "azure-monitor-application-insights-quickstart"
description: "A sample ASP.NET Core app that demonstrates basic instrumentation for Azure Monitor Application Insights."
---

<!-- 

This very simple sample is used to provide code snippets with highlighting for https://docs.microsoft.com/azure/azure-monitor/app/dotnet-quickstart

Please DO NOT update this sample unless you're making matching changes to that article!

Thanks!

 - The Management

-->

# Azure Monitor Application Insights Quickstart for ASP.NET Core

This sample ASP.NET Core application was created with `dotnet new razor`. After that, the following changes were made:

* The Application Insights SDK was added to the project with `dotnet add package Microsoft.ApplicationInsights.AspNetCore --version 2.17.0`.
* A configuration value for `ApplicationInsights:InstrumentationKey` was added to *appsettings.json*.
* `services.AddApplicationInsightsTelemetry();` was added to the `ConfigureServices` method of *Startup.cs*.
* `@inject Microsoft.ApplicationInsights.AspNetCore.JavaScriptSnippet JavaScriptSnippet` was added to *Pages/_ViewImports.cshtml*.
* `@Html.Raw(JavaScriptSnippet.FullScript)` was added to the `<head>` element in *Pages/Shared/_Layout.cshtml*.

## Prerequisites

* An [Azure subscription](https://azure.microsoft.com/free/dotnet/).
* An active [Azure Monitor Application Insights resource](https://docs.microsoft.com/azure/azure-monitor/app/create-new-resource).

## Prepare to run the sample

Replace the placeholder `ApplicationInsights:InstrumentationKey` value in *appsettings.json* with your actual instrumentation key.

## Run the sample

```dotnetcli
dotnet run
```

As you use the app, telemetry will be ingested to Application Insights within a few minutes. You can verify the connection using Live Metrics.
