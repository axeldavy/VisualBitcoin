using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;


namespace Backend
{
	public class WindowsAzure
	{
		static void uploadContainer(string containerName)
		{
			// Variables for the cloud storage objects.
			CloudStorageAccount storageAccount;
			CloudBlobClient blobClient;
			CloudBlobContainer blobContainer;
			BlobContainerPermissions blobContainerPermissions;

			// Use the emulatedstorage account.
			storageAccount = CloudStorageAccount.DevelopmentStorageAccount;

			// Use the storage account.
			// cloudStorageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=http;AccountName=your_storage_account_name;AccountKey=your_storage_account_key");

			// Create the blob client.
			blobClient = storageAccount.CreateCloudBlobClient();

			// Get the container reference.
			blobContainer = blobClient.GetContainerReference(containerName);
			// Create the container if it does not exist.
			blobContainer.CreateIfNotExist();

			// Set permissions on the container.
			blobContainerPermissions = new BlobContainerPermissions();
			blobContainerPermissions.PublicAccess = BlobContainerPublicAccessType.Blob;
			blobContainer.SetPermissions(blobContainerPermissions);

			Console.WriteLine("Upload container \"" + containerName + "\" OK.");
		}

		static void uploadBlob(string containerName, string blobName, string fileName)
		{
			// Variables for the cloud storage objects.
			CloudStorageAccount storageAccount;
			CloudBlobClient blobClient;
			CloudBlobContainer blobContainer;
			CloudBlockBlob blockBlob;

			// Retrieve storage account from connection string.
			storageAccount = CloudStorageAccount.DevelopmentStorageAccount;

			// Create the blob client.
			blobClient = storageAccount.CreateCloudBlobClient();

			// Retrieve reference to a previously created container.
			blobContainer = blobClient.GetContainerReference(containerName);

			// Retrieve reference to a blob.
			blockBlob = blobContainer.GetBlockBlobReference(blobName);

			// Create or overwrite the blob with contents from a local file.
			blockBlob.UploadFile(fileName);

			Console.WriteLine("Upload blob \"" + blobName + "\" in container \"" + containerName + "\" from file \"" + fileName + "\"");
		}
	}
}
