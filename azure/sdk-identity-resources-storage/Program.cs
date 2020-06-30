using System;
using System.Threading.Tasks;

using Azure;
using Azure.Identity;
using Azure.ResourceManager.Resources;
using Azure.ResourceManager.Resources.Models;
using Azure.ResourceManager.Storage;
using Azure.ResourceManager.Storage.Models;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

using AccessTier = Azure.ResourceManager.Storage.Models.AccessTier;
using Sku = Azure.ResourceManager.Storage.Models.Sku;

namespace AzureIdentityStorageExample
{
    class Program
    {
        static async Task Main(string[] args)
        {
            string subscriptionId = Environment.GetEnvironmentVariable("AZURE_SUBSCRIPTION_ID");

            var credential = new DefaultAzureCredential();

            using var resourcesManagementClient = new ResourcesManagementClient(subscriptionId, credential);
            using var storageManagementClient = new StorageManagementClient(subscriptionId, credential);

            // Create a Resource Group
            string resourceGroupName = await CreateResourceGroup(resourcesManagementClient);

            // Create a Storage account
            string storageName = await CreateStorageAccount(storageManagementClient, resourceGroupName);

            // Create a container and upload a blob
            await UploadBlobToStorageAccountUsingClientConnectionString(storageManagementClient, resourceGroupName, storageName);

            Console.WriteLine("Press any key to continue and delete the resources...");
            Console.ReadKey(true);

            await DeleteResourceGroup(resourcesManagementClient, resourceGroupName);

        }

        private static async Task DeleteResourceGroup(ResourcesManagementClient resourcesManagementClient, string resourceGroupName)
        {
            Console.WriteLine($"Deleting resource group {resourceGroupName}...");
            ResourceGroupsDeleteOperation deleteOperation = await resourcesManagementClient.ResourceGroups.StartDeleteAsync(resourceGroupName);
            await deleteOperation.WaitForCompletionAsync();
            Console.WriteLine("Done!");
        }

        private static async Task<string> CreateStorageAccount(StorageManagementClient storageManagementClient, string rgName)
        {
            Console.WriteLine("Creating a new storage account...");

            string storageAccountName = await GetStorageAccountName(storageManagementClient);
            StorageAccountCreateParameters parms = new StorageAccountCreateParameters(
                        new Sku("Standard_LRS"),
                        Kind.BlobStorage,
                        "West US");
            parms.AccessTier = AccessTier.Hot;

            StorageAccountsCreateOperation createStorageAccount =
                await storageManagementClient.StorageAccounts.StartCreateAsync(rgName, storageAccountName, parms);
                        await createStorageAccount.WaitForCompletionAsync();
            Console.WriteLine($"Done creating account {storageAccountName}.");

            return storageAccountName;
        }

        private static async Task<string> GetStorageAccountName(StorageManagementClient storageManagementClient)
        {
            bool foundAvailableName = false;
            string storageAccountName = string.Empty;

            while (!foundAvailableName)
            {
                storageAccountName = RandomName("storage", 20);
                Response<CheckNameAvailabilityResult> availability =
                    await storageManagementClient.StorageAccounts.CheckNameAvailabilityAsync(
                        new StorageAccountCheckNameAvailabilityParameters(storageAccountName));

                if (availability.Value.NameAvailable.GetValueOrDefault())
                {
                    foundAvailableName = true;
                }
            }
            return storageAccountName;
        }

        private static async Task UploadBlobToStorageAccountUsingClientConnectionString(StorageManagementClient storageManagementClient, string resourceGroupName, string storageName)
        {
            Console.WriteLine("Creating a container and uploading a blob...");

            const string uploadFileName = "dotnet-bot.png";
            const string containerName = "images";
            
            string connectionString = await GetStorageConnectionStringAsync(storageManagementClient, resourceGroupName, storageName);

            var containerClient = new BlobContainerClient(connectionString, containerName);
            await containerClient.CreateIfNotExistsAsync(publicAccessType: PublicAccessType.Blob);

            BlobClient blobClient = containerClient.GetBlobClient(uploadFileName);
            await blobClient.UploadAsync(uploadFileName);

            Console.WriteLine($"Your blob is at {blobClient.Uri}");
        }


        private static async Task<string> GetStorageConnectionString(StorageManagementClient storageManagementClient, string resourceGroupName, string storageAccountName)
        {
            Response<StorageAccountListKeysResult> keysResponse = await storageManagementClient.StorageAccounts.ListKeysAsync(resourceGroupName, storageAccountName);
            StorageAccountKey storageKey = keysResponse.Value.Keys[0];
            
            return $"DefaultEndpointsProtocol=https;AccountName={storageAccountName};AccountKey={storageKey.Value};EndpointSuffix=core.windows.net;";
        }

        private static async Task<string> CreateResourceGroup(ResourcesManagementClient resourcesManagementClient)
        {
            string resourceGroupName = RandomName("rg", 20);
            Console.WriteLine($"Creating resource group {resourceGroupName}...");
            await resourcesManagementClient.ResourceGroups.CreateOrUpdateAsync(resourceGroupName, new ResourceGroup("West US"));
            Console.WriteLine("Done!");

            return resourceGroupName;
        }

        static string RandomName(string prefix, int maxLen)
        {
            var random = new Random();
            string s = prefix;
            for (int i = 0; i < (maxLen - prefix.Length); i++)
                s = String.Concat(s, random.Next(10).ToString());
            return s;            
        }
    }
}
