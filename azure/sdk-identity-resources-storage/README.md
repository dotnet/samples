---
languages:
- csharp
products:
- dotnet
- azure-sdks
page_type: sample
name: "Azure Identity, Resource Management, and Storage sample"
urlFragment: "azure-identity-resource-management-storage"
description: "A sample console application that shows how to use the Azure SDK for .NET to authenticate an app using a service principal, create a resource group, create a storage account, and upload a blob."
---

# Azure Identity, Resource Management, and Storage sample

This sample console application accomplishes the following tasks using the Azure SDK for .NET:

* Creates a `DefaultAzureCredential` to represent the app's credentials.
* Creates a resource group.
* Creates a storage account in the resource group.
* Uploads an image to the storage account using a storage connection string.
* Uploads an image to the storage account using `DefaultAzureCredential`.
* Deletes the resource group and all of its contents.

> [!NOTE]
> This sample uses preview packages for *Azure.ResourceManagement.Storage*. *Azure.ResourceManager.Resource*, *Azure.Identity*, and *Azure.Storage.Blobs* are generally available.

## Prerequisites

* An [Azure subscription](https://azure.microsoft.com/free/dotnet/).

## Prepare to run the sample

Use the [Azure Cloud Shell](https://shell.azure.com) to create and get client secret credentials:

1. Create a service principal and configure its access to Azure resources:

    ```azurecli
    az ad sp create-for-rbac -n http://<your-application-name>.<your-organization-domain>

    # Example: az ad sp create-for-rbac -n http://this-sample-app.contoso.com
    ```

    Output:

    ```json
    {
        "appId": "<generated-app-ID>",
        "displayName": "http://this-sample-app.contoso.com",
        "password": "<random-password>",
        "tenant": "<tenant-ID>"
    }
    ```

    This creates a service principal. This is an identity for your app to use to perform Azure operations.

1. Assign the *Contributor* role to the service principal so that it can create the necessary resources

```azurecli
az role assignment create --role "Contributor" --assignee <appId>
```

1. Assign the *Storage Blob Data Contributor* role to service principal. There are two possible ways to do this (either way is acceptable):

    * Use the `appId` property from the output in the first step.

    ```azurecli
    az role assignment create --role "Storage Blob Data Contributor" --assignee <appId>
    ```

    * Use the `objectId` of the Service Principal account

    ```azurecli
    az ad sp show <appid>
    az az role assignment create --role 'Storage Blob Data Contributor' --assignee-object-id <objectid> --assignee-principal-type ServicePrincipal
    ```

    This will allow the service principal to perform blob data operations using Azure.Identity (as opposed to a connection string).

1. Use the returned credentials from the first step to set the following environment variables.

    |Variable name|Description|Value|
    |-|-|-|
    |`AZURE_CLIENT_ID`|Service principal's app identifier|`appId`|
    |`AZURE_TENANT_ID`|Identifier of the principal's Azure Active Directory tenant|`tenant`|
    |`AZURE_CLIENT_SECRET`|Client app secret|`password`|

    Azure.Identity reads these values from the environment at runtime to create a `DefaultAzureCredential` object.

1. Get the account details of the subscription you want to use for this sample.

    ```azurecli
    az account show
    ```

    Output:

    ```json
    {
      "environmentName": "AzureCloud",
      "homeTenantId": "tenant-id",
      "id": "subscription-id",
      "isDefault": true,
      "managedByTenants": [],
      "name": "subscriptionName",
      "state": "Enabled",
      "tenantId": "tenant-id",
      "user": {
        "cloudShellID": true,
        "name": "user@contoso.com",
        "type": "user"
      }
    }
    ```

1. Set an environment variable named `AZURE_SUBSCRIPTION_ID` using the `id` property of the information retrieved in the previous step.

> [!NOTE]
> Environment variables can be set in your operating system, or you can use a [*launchSettings.json* file](https://docs.microsoft.com/aspnet/core/fundamentals/environments?view=aspnetcore-3.1#lsj).

## Run the sample

```dotnetcli
dotnet build
dotnet run
```

## Clean up the service principal

You should remove unused service principals. Use the URL in the `name` property from the output in the first step, including `http://`.

```azurecli
az ad sp delete --id <sample-app-name-url>
```
