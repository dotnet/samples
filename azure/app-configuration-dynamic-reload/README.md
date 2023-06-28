---
languages:
- csharp
products:
- aspnet-core
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

### Add a sentinel key

A *sentinel key* is a special key that you update after you complete the change of all other keys. Your application monitors the sentinel key. When a change is detected, your application refreshes all configuration values. This approach helps to ensure the consistency of configuration in your application and reduces the overall number of requests made to App Configuration, compared to monitoring all keys for changes.

1. In the Azure portal, select **Configuration Explorer > Create > Key-value**.
1. For **Key**, enter *TestApp:Settings:Sentinel*. For **Value**, enter 1. Leave **Label** and **Content type** blank.
1. Select **Apply**.

## Run the sample

```dotnetcli
dotnet build
dotnet run
```

Change the value stored in the sentinel key in the Azure Portal. Within 10 seconds, the new config value will be available to your app.
