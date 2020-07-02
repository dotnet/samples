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
    /// <summary>
    /// Sample app
    /// </summary>
    class Program
    {
        const string ResourceRegion = "West US";
        const string DotNetBotChillingPng = "dotnet-bot_chilling.png";
        const string DotNetBotGrillingPng = "dotnet-bot_grilling.png";
        const string BlobContainerName = "images";

        /// <summary>
        /// Main program.
        /// </summary>
        static async Task Main()
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

        /// <summary>
        /// Creates a new resource group with a random name.
        /// </summary>
        /// <param name="resourcesManagementClient">A credentialed ResourcesManagementClient.</param>
        /// <returns></returns>
        private static async Task<string> CreateResourceGroupAsync(ResourcesManagementClient resourcesManagementClient)
        {
            string resourceGroupName = RandomName("rg", 20);
            Console.WriteLine($"Creating resource group {resourceGroupName}...");
            await resourcesManagementClient.ResourceGroups.CreateOrUpdateAsync(resourceGroupName, new ResourceGroup(ResourceRegion));
            Console.WriteLine("Done!");

            return resourceGroupName;
        }

        /// <summary>
        /// Creates a new storage account with a random name.
        /// </summary>
        /// <param name="storageManagementClient">A credentialed StorageManagementClient.</param>
        /// <param name="rgName">The name of the resource group in which to create the storage account.</param>
        /// <returns></returns>
        private static async Task<string> CreateStorageAccountAsync(StorageManagementClient storageManagementClient, string rgName)
        {
            Console.WriteLine("Creating a new storage account...");

            string storageAccountName = await GenerateStorageAccountNameAsync(storageManagementClient);
            StorageAccountCreateParameters parameters =
                new StorageAccountCreateParameters(new Sku("Standard_LRS"), Kind.BlobStorage, ResourceRegion)
                    { AccessTier = AccessTier.Hot };

            StorageAccountsCreateOperation createStorageAccountOperation =
                await storageManagementClient.StorageAccounts.StartCreateAsync(rgName, storageAccountName, parameters);
            await createStorageAccountOperation.WaitForCompletionAsync();
            Console.WriteLine($"Done creating account {storageAccountName}.");

            return storageAccountName;
        }

        /// <summary>
        /// Generates a random, unique storage account name.
        /// </summary>
        /// <param name="storageManagementClient">A credentialed StorageManagementClient.</param>
        /// <returns></returns>
        private static async Task<string> GenerateStorageAccountNameAsync(StorageManagementClient storageManagementClient)
        {
            string storageAccountName = RandomName("storage", 20);
            while (true)
            {
                Response<CheckNameAvailabilityResult> availabilityResponse =
                    await storageManagementClient.StorageAccounts.CheckNameAvailabilityAsync(
                        new StorageAccountCheckNameAvailabilityParameters(storageAccountName));

                if (availabilityResponse.Value.NameAvailable.GetValueOrDefault())
                {
                    return storageAccountName;
                }
                
                storageAccountName = RandomName("storage", 20);
            }
        }

        /// <summary>
        /// Uploads an image as a blob to a storage account using a BlobContainerClient and a storage connection string.
        /// </summary>
        /// <param name="storageManagementClient">A credentialed StorageManagementClient.</param>
        /// <param name="resourceGroupName">The name of the resource group containing the storage account.</param>
        /// <param name="storageAccountName">The name of the storage account in which to upload the blob.</param>
        /// <returns></returns>
        private static async Task UploadBlobUsingStorageConnectionStringAsync(StorageManagementClient storageManagementClient, string resourceGroupName, string storageAccountName)
        {
            Console.WriteLine($"Creating a container and uploading blob {DotNetBotChillingPng} using a storage connection string...");

            string connectionString = await GetStorageConnectionStringAsync(storageManagementClient, resourceGroupName, storageAccountName);

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
        /// <param name="storageManagementClient">A credentialed StorageManagementClient.</param>
        /// <param name="resourceGroupName">The name of the resource group containing the storage account.</param>
        /// <param name="storageAccountName">The name of the storage account in which to upload the blob.</param>
        /// <returns></returns>
        static async Task<string> GetStorageConnectionStringAsync(StorageManagementClient storageManagementClient, string resourceGroupName, string storageAccountName)
        {
            Response<StorageAccountListKeysResult> keysResponse = await storageManagementClient.StorageAccounts.ListKeysAsync(resourceGroupName, storageAccountName);
            StorageAccountKey storageKey = keysResponse.Value.Keys[0];
            return $"DefaultEndpointsProtocol=https;AccountName={storageAccountName};AccountKey={storageKey.Value};EndpointSuffix=core.windows.net;";
        }

        /// <summary>
        /// Uploads an image as a blob to a storage account using a BlobContainerClient and an Azure.Core.TokenCredential (like DefaultAzureCredential).
        /// </summary>
        /// <param name="storageManagementClient">A credentialed StorageManagementClient.</param>
        /// <param name="resourceGroupName">The name of the resource group containing the storage account.</param>
        /// <param name="storageAccountName">The name of the storage account in which to upload the blob.</param>
        /// <param name="credential">An Azure.Core.TokenCredential that contains an authenticated token.</param>
        /// <returns></returns>
        private static async Task UploadBlobUsingDefaultAzureCredentialAsync(StorageManagementClient storageManagementClient, string resourceGroupName, string storageAccountName, TokenCredential credential)
        {
            Console.WriteLine($"Uploading blob {DotNetBotGrillingPng} using DefaultAzureCredential...");

            string connectionString = await GetStorageConnectionStringAsync(storageManagementClient, resourceGroupName, storageAccountName);

            string blobEndpoint = storageManagementClient.StorageAccounts.GetProperties(resourceGroupName, storageAccountName).Value.PrimaryEndpoints.Blob;
            var containerClient = new BlobContainerClient(new Uri($"{blobEndpoint}{BlobContainerName}"), credential);

            BlobClient blobClient = containerClient.GetBlobClient(DotNetBotGrillingPng);
            await blobClient.UploadAsync(DotNetBotGrillingPng);

            Console.WriteLine($"Your blob uploaded with DefaultAzureCredential is at:");
            Console.WriteLine("");
            Console.WriteLine(blobClient.Uri);
            Console.WriteLine("");
        }

        /// <summary>
        /// Deletes the resource group (and all of its included resources).
        /// </summary>
        /// <param name="resourcesManagementClient">A credentialed ResourcesManagementClient.</param>
        /// <param name="resourceGroupName">The name of the resource group containing the storage account.</param>
        /// <returns></returns>
        private static async Task DeleteResourceGroupAsync(ResourcesManagementClient resourcesManagementClient, string resourceGroupName)
        {
            Console.WriteLine($"Deleting resource group {resourceGroupName}...");
            ResourceGroupsDeleteOperation deleteOperation = await resourcesManagementClient.ResourceGroups.StartDeleteAsync(resourceGroupName);
            await deleteOperation.WaitForCompletionAsync();
            Console.WriteLine("Done!");
        }

        /// <summary>
        /// Generates a random name.
        /// </summary>
        /// <param name="prefix">The text to include at the beginning of the name.</param>
        /// <param name="maxLen">The total length of the name.</param>
        /// <returns></returns>
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
