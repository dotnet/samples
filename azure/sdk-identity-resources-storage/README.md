## Prerequisites

* An [Azure subscription]().

## Preparing to run the sample

Use the [Azure Cloud Shell](https://shell.azure.com) to create/get client secret credentials.

* Create a service principal and configure its access to Azure resources:

    ```bash
    az ad sp create-for-rbac -n <your-application-name> --skip-assignment
    ```

    Output:

    ```json
    {
        "appId": "generated-app-ID",
        "displayName": "dummy-app-name",
        "name": "http://dummy-app-name",
        "password": "random-password",
        "tenant": "tenant-ID"
    }
    ```

* Use the returned credentials above to set the following environment variables:

    |variable name|value
    |-|-
    |`AZURE_CLIENT_ID`|service principal's app id
    |`AZURE_TENANT_ID`|id of the principal's Azure Active Directory tenant
    |`AZURE_CLIENT_SECRET`|one of the service principal's client secrets

* Get the account details of the subscription you want to use for this sample.

    ```bash
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

* Set the `AZURE_SUBSCRIPTION_ID` environment variable using the `id` property of the information retrieved in the previous step.

    |variable name|value
    |-|-
    |`AZURE_SUBSCRIPTION_ID`|subscription id for resources

## Run the sample

```bash
dotnet run
```
