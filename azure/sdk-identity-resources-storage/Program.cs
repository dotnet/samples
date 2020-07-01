using System;
using System.Text;
using System.Threading.Tasks;

using Azure;
using Azure.Core;
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
        const string ResourceRegion = "West US";
        const string UploadFileName1 = "dotnet-bot_chilling.png";
        const string UploadFileName2 = "dotnet-bot_grilling.png";
        const string BlobContainerName = "images";

        static async Task Main(string[] args)
        {
            string subscriptionId = Environment.GetEnvironmentVariable("AZURE_SUBSCRIPTION_ID");

            var credential = new DefaultAzureCredential();

            // Azure.ResourceManager.Resources is currently in preview.
            var resourcesManagementClient = new ResourcesManagementClient(subscriptionId, credential);

            // Azure.ResourceManager.Storage is currently in preview.
            var storageManagementClient = new StorageManagementClient(subscriptionId, credential);

            try
            {
                // Create a Resource Group
                string resourceGroupName = await CreateResourceGroupAsync(resourcesManagementClient);

                // Create a Storage account
                string storageName = await CreateStorageAccountAsync(storageManagementClient, resourceGroupName);

                // Create a container and upload a blob using a storage connection string
                await UploadBlobUsingStorageConnectionStringAsync(storageManagementClient, resourceGroupName, storageName);

                // Upload a blob using Azure.Identity.DefaultAzureCredential
                await UploadBlobUsingDefaultAzureCredentialAsync(storageManagementClient, resourceGroupName, storageName, credential);


                Console.WriteLine("Press any key to continue and delete the resources...");
                Console.ReadKey(true);

                // Delete the resource group
                await DeleteResourceGroupAsync(resourcesManagementClient, resourceGroupName);
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

        private static async Task<string> CreateResourceGroupAsync(ResourcesManagementClient resourcesManagementClient)
        {
            string resourceGroupName = RandomName("rg", 20);
            Console.WriteLine($"Creating resource group {resourceGroupName}...");
            await resourcesManagementClient.ResourceGroups.CreateOrUpdateAsync(resourceGroupName, new ResourceGroup(ResourceRegion));
            Console.WriteLine("Done!");

            return resourceGroupName;
        }

        private static async Task<string> CreateStorageAccountAsync(StorageManagementClient storageManagementClient, string rgName)
        {
            Console.WriteLine("Creating a new storage account...");

            string storageAccountName = await GetStorageAccountName(storageManagementClient);
            StorageAccountCreateParameters parms = new StorageAccountCreateParameters(
                                                    new Sku("Standard_LRS"),
                                                    Kind.BlobStorage,
                                                    ResourceRegion);
            parms.AccessTier = AccessTier.Hot;

            StorageAccountsCreateOperation createStorageAccount =
                await storageManagementClient.StorageAccounts.StartCreateAsync(rgName, storageAccountName, parms);
            await createStorageAccount.WaitForCompletionAsync();
            Console.WriteLine($"Done creating account {storageAccountName}.");

            return storageAccountName;
        }

        private static async Task<string> GetStorageAccountName(StorageManagementClient storageManagementClient)
        {
            string storageAccountName = RandomName("storage", 20);
            while (true)
            {
                Response<CheckNameAvailabilityResult> availability =
                    await storageManagementClient.StorageAccounts.CheckNameAvailabilityAsync(
                        new StorageAccountCheckNameAvailabilityParameters(storageAccountName));

                if (availability.Value.NameAvailable.GetValueOrDefault())
                {
                    return storageAccountName;
                }
                
                storageAccountName = RandomName("storage", 20);
            }
        }

        private static async Task UploadBlobUsingStorageConnectionStringAsync(StorageManagementClient storageManagementClient, string resourceGroupName, string storageName)
        {
            Console.WriteLine("Creating a container and uploading a blob using a storage connection string...");

            string connectionString = await GetStorageConnectionStringAsync(storageManagementClient, resourceGroupName, storageName);

            var containerClient = new BlobContainerClient(connectionString, BlobContainerName);
            await containerClient.CreateIfNotExistsAsync(publicAccessType: PublicAccessType.Blob);

            BlobClient blobClient = containerClient.GetBlobClient(UploadFileName1);
            await blobClient.UploadAsync(UploadFileName1);

            Console.WriteLine($"Your blob uploaded with a connection string is at:");
            Console.WriteLine("\r\n");
            Console.WriteLine(blobClient.Uri);
            Console.WriteLine("\r\n");
        }

        private static async Task UploadBlobUsingDefaultAzureCredentialAsync(StorageManagementClient storageManagementClient, string resourceGroupName, string storageName, TokenCredential credential)
        {
            Console.WriteLine("Uploading a blob using DefaultAzureCredential...");

            string connectionString = await GetStorageConnectionStringAsync(storageManagementClient, resourceGroupName, storageName);

            string blobEndpoint = storageManagementClient.StorageAccounts.GetProperties(resourceGroupName, storageName).Value.PrimaryEndpoints.Blob;
            var containerClient = new BlobContainerClient(new Uri($"{blobEndpoint}{BlobContainerName}"), credential);

            BlobClient blobClient = containerClient.GetBlobClient(UploadFileName2);
            await blobClient.UploadAsync(UploadFileName2);

            Console.WriteLine($"Your blob uploaded with DefaultAzureCredential is at:");
            Console.WriteLine("\r\n");
            Console.WriteLine(blobClient.Uri);
            Console.WriteLine("\r\n");
        }


        static async Task<string> GetStorageConnectionStringAsync(StorageManagementClient storageManagementClient, string resourceGroupName, string storageAccountName)
        {
            Response<StorageAccountListKeysResult> keysResponse = await storageManagementClient.StorageAccounts.ListKeysAsync(resourceGroupName, storageAccountName);
            StorageAccountKey storageKey = keysResponse.Value.Keys[0];

            return $"DefaultEndpointsProtocol=https;AccountName={storageAccountName};AccountKey={storageKey.Value};EndpointSuffix=core.windows.net;";
        }

        private static async Task DeleteResourceGroupAsync(ResourcesManagementClient resourcesManagementClient, string resourceGroupName)
        {
            Console.WriteLine($"Deleting resource group {resourceGroupName}...");
            ResourceGroupsDeleteOperation deleteOperation = await resourcesManagementClient.ResourceGroups.StartDeleteAsync(resourceGroupName);
            await deleteOperation.WaitForCompletionAsync();
            Console.WriteLine("Done!");
        }



        static string RandomName(string prefix, int maxLen)
        {
            var random = new Random();
            var sb = new StringBuilder(prefix);
            for (int i = 0; i < (maxLen - prefix.Length); i++)
                sb.Append(random.Next(10).ToString());
            return sb.ToString();
        }
    }
}
