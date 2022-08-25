using System;
using System.Text;
using System.Threading.Tasks;
using Azure;
using Azure.Core;
using Azure.Identity;
using Azure.ResourceManager;
using Azure.ResourceManager.Resources;
using Azure.ResourceManager.Storage;
using Azure.ResourceManager.Storage.Models;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using AccessTier = Azure.ResourceManager.Storage.Models.StorageAccountAccessTier;

namespace AzureIdentityStorageExample;

class Program
{
    static readonly AzureLocation s_resourceRegion = new("West US");
    
    const string DotNetBotChillingPng = "dotnet-bot_chilling.png";
    const string DotNetBotGrillingPng = "dotnet-bot_grilling.png";
    const string BlobContainerName = "images";

    static async Task Main()
    {
        var credential = new DefaultAzureCredential();
        var armClient = new ArmClient(credential);        
        var subscription = await armClient.GetDefaultSubscriptionAsync();

        try
        {
            // Create a Resource Group
            var resourceGroup = await CreateResourceGroupAsync(subscription);

            // Create a Storage account
            var storageAccount = await CreateStorageAccountAsync(subscription, resourceGroup);

            // Create a container and upload a blob using a storage connection string
            await UploadBlobUsingStorageConnectionStringAsync(storageAccount);

            // Upload a blob using Azure.Identity.DefaultAzureCredential
            await UploadBlobUsingDefaultAzureCredentialAsync(storageAccount, credential);


            Console.WriteLine("Press any key to continue and delete the resources...");
            Console.ReadKey(true);
            
            // Delete the resource group
            Console.WriteLine("Deleting the resources...");
            await resourceGroup.DeleteAsync(WaitUntil.Completed);
            Console.WriteLine("Done!");
        }
        catch (RequestFailedException ex)
        {
            Console.WriteLine($"Request failed! {ex.Message} {ex.StackTrace}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unexpected exception! {ex.Message} {ex.StackTrace}");
        }
    }

    /// <summary>
    /// Creates a new resource group with a random name.
    /// </summary>
    /// <param name="subscription">The subscription in which to create the resource group.</param>

    private static async Task<ResourceGroupResource> CreateResourceGroupAsync(
        SubscriptionResource subscription)
    {
        string resourceGroupName = RandomName("rg", 20);
        Console.WriteLine($"Creating resource group {resourceGroupName}...");
        ArmOperation<ResourceGroupResource> resourceGroupOperation = 
            await subscription.GetResourceGroups()
                .CreateOrUpdateAsync(
                    WaitUntil.Completed, 
                    resourceGroupName,
                    new ResourceGroupData(s_resourceRegion));
        
        Console.WriteLine("Done!");
        
        return resourceGroupOperation.Value;
    }

    /// <summary>
    /// Creates a new storage account with a random name.
    /// </summary>
    /// <param name="subscription">The subscription in which to create the storage account.</param>
    /// <param name="resourceGroup">The resource group in which to create the storage account.</param>
    /// <returns></returns>
    private static async Task<StorageAccountResource> CreateStorageAccountAsync(
        SubscriptionResource subscription, ResourceGroupResource resourceGroup)
    {
        Console.WriteLine("Creating a new storage account...");

        var storageAccountName = await GenerateStorageAccountNameAsync(subscription);

        var parameters =
            new StorageAccountCreateOrUpdateContent(
                new StorageSku(StorageSkuName.StandardLrs),
                StorageKind.BlobStorage,
                s_resourceRegion)
            {
                AccessTier = AccessTier.Hot
            };

        var storageAccounts = resourceGroup.GetStorageAccounts();
        ArmOperation<StorageAccountResource> createStorageAccountOperation = 
            await storageAccounts.CreateOrUpdateAsync(
                WaitUntil.Completed,
                storageAccountName, 
                parameters);   
                     
        Console.WriteLine($"Done creating account {storageAccountName}.");

        return createStorageAccountOperation.Value;
    }

    /// <summary>
    /// Generates a random, unique storage account name.
    /// </summary>
    /// <param name="subscription">The subscription in which to create the storage account.</param>
    /// <returns></returns>
    private static async Task<string> GenerateStorageAccountNameAsync(SubscriptionResource subscription)
    {
        while (true)
        {
            var storageAccountName = RandomName("storage", 20);
            var availabilityResponse =
                await subscription.CheckStorageAccountNameAvailabilityAsync(
                    new StorageAccountNameAvailabilityContent(storageAccountName));

            if (availabilityResponse.Value.IsNameAvailable.GetValueOrDefault())
            {
                return storageAccountName;
            }
        }
    }

    /// <summary>
    /// Uploads an image as a blob to a storage account using a BlobContainerClient and a storage connection string.
    /// </summary>
    /// <param name="storageAccount">The storage account in which to upload the blob.</param>
    /// <returns></returns>
    private static async Task UploadBlobUsingStorageConnectionStringAsync(
        StorageAccountResource storageAccount)
    {
        Console.WriteLine(
            $"Creating a container and uploading blob {DotNetBotChillingPng} using a storage connection string...");

        var connectionString = await GetStorageConnectionStringAsync(storageAccount);

        var containerClient = new BlobContainerClient(connectionString, BlobContainerName);
        await containerClient.CreateIfNotExistsAsync(publicAccessType: PublicAccessType.Blob);

        BlobClient blobClient = containerClient.GetBlobClient(DotNetBotChillingPng);
        await blobClient.UploadAsync(DotNetBotChillingPng);

        Console.WriteLine($"Your blob uploaded with a connection string is at:");
        Console.WriteLine("");
        Console.WriteLine(blobClient.Uri);
        Console.WriteLine("");
    }

    /// <summary>
    /// Retrieves the storage account key and uses it to return a storage connection string.
    /// </summary>
    /// <param name="storageAccount">The storage account in which to upload the blob.</param>
    /// <returns></returns>
    static async Task<string> GetStorageConnectionStringAsync(StorageAccountResource storageAccount)
    {
        var keysResponse = storageAccount.GetKeysAsync().GetAsyncEnumerator();
        var storageKey = await keysResponse.MoveNextAsync()
            ? keysResponse.Current.Value
            : throw new Exception("No keys found for storage account.");

        var connectionStringParts = new string[]
        {
            "DefaultEndpointsProtocol=https",
            $"AccountName={storageAccount.Data.Name}",
            $"AccountKey={storageKey}",
            "EndpointSuffix=core.windows.net"
        };

        return string.Join(";", connectionStringParts);
    }

    /// <summary>
    /// Uploads an image as a blob to a storage account using a BlobContainerClient and an Azure.Core.TokenCredential (like DefaultAzureCredential).
    /// </summary>
    /// <param name="storageAccount">The storage account in which to upload the blob.</param>
    /// <param name="credential">An Azure.Core.TokenCredential that contains an authenticated token.</param>
    /// <returns></returns>
    private static async Task UploadBlobUsingDefaultAzureCredentialAsync(
        StorageAccountResource storageAccount, TokenCredential credential)
    {
        Console.WriteLine($"Uploading blob {DotNetBotGrillingPng} using DefaultAzureCredential...");

        var containerClient = new BlobContainerClient(
            new Uri($"{storageAccount.Data.PrimaryEndpoints.BlobUri}{BlobContainerName}"), credential);

        BlobClient blobClient = containerClient.GetBlobClient(DotNetBotGrillingPng);
        await blobClient.UploadAsync(DotNetBotGrillingPng);

        Console.WriteLine($"Your blob uploaded with DefaultAzureCredential is at:");
        Console.WriteLine("");
        Console.WriteLine(blobClient.Uri);
        Console.WriteLine("");
    }

    /// <summary>
    /// Generates a random name.
    /// </summary>
    /// <param name="prefix">The text to include at the beginning of the name.</param>
    /// <param name="maxLen">The total length of the name.</param>
    static string RandomName(string prefix, int maxLen)
    {
        var random = Random.Shared;
        var sb = new StringBuilder(prefix);
        for (int i = 0; i < maxLen - prefix.Length; i++)
            sb.Append(random.Next(10));
        return sb.ToString();
    }
}
