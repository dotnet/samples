using System;
using System.Text;
using System.Threading.Tasks;

using Azure;
using Azure.Core;
using Azure.Identity;
using Azure.ResourceManager;
using Azure.ResourceManager.Resources;
using Azure.ResourceManager.Resources.Models;
using Azure.ResourceManager.Storage;
using Azure.ResourceManager.Storage.Models;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

using AccessTier = Azure.ResourceManager.Storage.Models.AccessTier;


namespace AzureIdentityStorageExample
{
    /// <summary>
    /// Sample app
    /// </summary>
    class Program
    {
        static AzureLocation _resourceRegion = new AzureLocation("West US");
        const string _dotNetBotChillingPng = "dotnet-bot_chilling.png";
        const string _dotNetBotGrillingPng = "dotnet-bot_grilling.png";
        const string _blobContainerName = "images";

        /// <summary>
        /// Main program.
        /// </summary>
        static async Task Main()
        {
            var credential = new DefaultAzureCredential();

            var armClient = new ArmClient(credential);
            
            SubscriptionResource subscription = await armClient.GetDefaultSubscriptionAsync();

            try
            {
                // Create a Resource Group
                ResourceGroupResource resourceGroup = await CreateResourceGroupAsync(subscription);

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
        /// <returns></returns>
        private static async Task<ResourceGroupResource> CreateResourceGroupAsync(SubscriptionResource subscription)
        {
            string resourceGroupName = RandomName("rg", 20);
            Console.WriteLine($"Creating resource group {resourceGroupName}...");
            ArmOperation<ResourceGroupResource> resourceGroupOperation = 
                await subscription.GetResourceGroups().CreateOrUpdateAsync(WaitUntil.Completed, 
                                                                        resourceGroupName, 
                                                                        new ResourceGroupData(_resourceRegion));
            Console.WriteLine("Done!");
            
            return resourceGroupOperation.Value;
        }

        /// <summary>
        /// Creates a new storage account with a random name.
        /// </summary>
        /// <param name="subscription">The subscription in which to create the storage account.</param>
        /// <param name="resourceGroup">The resource group in which to create the storage account.</param>
        /// <returns></returns>
        private static async Task<StorageAccountResource> CreateStorageAccountAsync(SubscriptionResource subscription, ResourceGroupResource resourceGroup)
        {
            Console.WriteLine("Creating a new storage account...");

            string storageAccountName = await GenerateStorageAccountNameAsync(subscription);
            var parameters =
                new StorageAccountCreateOrUpdateContent(new StorageSku(StorageSkuName.StandardLRS), 
                                                            StorageKind.BlobStorage, 
                                                            _resourceRegion);
            
            parameters.AccessTier = AccessTier.Hot;
          
            var storageAccounts = resourceGroup.GetStorageAccounts();
            ArmOperation<StorageAccountResource> createStorageAccountOperation = 
                await storageAccounts.CreateOrUpdateAsync(WaitUntil.Completed,
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
                string storageAccountName = RandomName("storage", 20);
                Response<CheckNameAvailabilityResult> availabilityResponse =
                    await subscription.CheckStorageAccountNameAvailabilityAsync(
                        new StorageAccountNameAvailabilityContent(storageAccountName));

                if (availabilityResponse.Value.NameAvailable.GetValueOrDefault())
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
        private static async Task UploadBlobUsingStorageConnectionStringAsync(StorageAccountResource storageAccount)
        {
            Console.WriteLine($"Creating a container and uploading blob {_dotNetBotChillingPng} using a storage connection string...");

            string connectionString = await GetStorageConnectionStringAsync(storageAccount);

            var containerClient = new BlobContainerClient(connectionString, _blobContainerName);
            await containerClient.CreateIfNotExistsAsync(publicAccessType: PublicAccessType.Blob);

            BlobClient blobClient = containerClient.GetBlobClient(_dotNetBotChillingPng);
            await blobClient.UploadAsync(_dotNetBotChillingPng);

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
            Response<StorageAccountListKeysResult> keysResponse = await storageAccount.GetKeysAsync();
            StorageAccountKey storageKey = keysResponse.Value.Keys[0];
            return $"DefaultEndpointsProtocol=https;AccountName={storageAccount.Data.Name};AccountKey={storageKey.Value};EndpointSuffix=core.windows.net;";
        }

        /// <summary>
        /// Uploads an image as a blob to a storage account using a BlobContainerClient and an Azure.Core.TokenCredential (like DefaultAzureCredential).
        /// </summary>
        /// <param name="storageAccount">The storage account in which to upload the blob.</param>
        /// <param name="credential">An Azure.Core.TokenCredential that contains an authenticated token.</param>
        /// <returns></returns>
        private static async Task UploadBlobUsingDefaultAzureCredentialAsync(StorageAccountResource storageAccount, TokenCredential credential)
        {
            Console.WriteLine($"Uploading blob {_dotNetBotGrillingPng} using DefaultAzureCredential...");

            var containerClient = new BlobContainerClient(new Uri($"{storageAccount.Data.PrimaryEndpoints.Blob}{_blobContainerName}"), credential);

            BlobClient blobClient = containerClient.GetBlobClient(_dotNetBotGrillingPng);
            await blobClient.UploadAsync(_dotNetBotGrillingPng);

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
