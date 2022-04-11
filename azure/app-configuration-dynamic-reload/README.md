---
languages:
- csharp
products:
- dotnet-core
- azure
page_type: sample
name: "Azure App Configuration dynamic settings sample"
urlFragment: "azure-app-config-dynamic-settings"
description: "A sample web application that demonstrates reading settings dynamically from Azure App Configuration using ASP.NET Core."
---

# Azure App Configuration

This app demonstrates reading settings dynamically from Azure App Configuration.

## Prerequisites

* An [Azure subscription](https://azure.microsoft.com/free/dotnet/).

## Prepare to run the sample

1. Create a new Azure App Configuration resource as in [this quickstart](https://docs.microsoft.com/azure/azure-app-configuration/quickstart-aspnet-core-app). **Complete only the section labeled *Create an App Configuration store*.**
1. Add your connection string to *appsettings.json*.

## Run the sample

```dotnetcli
dotnet build
dotnet run
```
