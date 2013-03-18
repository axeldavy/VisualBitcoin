using System;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Configuration;

namespace Storage
{
    public interface IStorage
    {
        CloudBlobClient RetrieveBlobClient();
        CloudBlobClient RetrieveEmulatedBlobClient();
        void UploadContainer(CloudBlobClient blobClient, string containerName);
        void UploadBlob(CloudBlobClient blobClient, string containerName, string blobName, string fileName);
    }
    
    public class WindowsAzureStorage : IStorage
	{
        public CloudBlobClient RetrieveBlobClient() {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConfigurationManager.ConnectionStrings["StorageConnectionString"].ConnectionString);
            return storageAccount.CreateCloudBlobClient();
        }

        public CloudBlobClient RetrieveEmulatedBlobClient()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.DevelopmentStorageAccount;
            return storageAccount.CreateCloudBlobClient();
        }

        public void UploadContainer(CloudBlobClient blobClient, string containerName)
		{
			// Variables for the cloud storage objects.
			CloudBlobContainer blobContainer;
			//BlobContainerPermissions blobContainerPermissions;

			// Get the container reference.
			blobContainer = blobClient.GetContainerReference(containerName);

			// Create the container if it does not exist.
            blobContainer.CreateIfNotExists();

			// Set permissions on the container.
			// blobContainerPermissions = new BlobContainerPermissions();
			//blobContainerPermissions.PublicAccess = BlobContainerPublicAccessType.Blob;
			//blobContainer.SetPermissions(blobContainerPermissions);

			Console.WriteLine("Upload complete: container \"" + containerName + "\"");
		}

        public void UploadBlob(CloudBlobClient blobClient, string containerName, string blobName, string fileName)
		{
			CloudBlobContainer blobContainer;
            CloudBlockBlob blockBlob;

			// Retrieve reference to a container.
			blobContainer = blobClient.GetContainerReference(containerName);
            
			// Retrieve reference to a blob.
			blockBlob = blobContainer.GetBlockBlobReference(blobName);

			// Create or overwrite the blob with contents from a local file.
            using (var fileStream = System.IO.File.OpenRead(@fileName))
            {
                blockBlob.UploadFromStream(fileStream);
            }

			Console.WriteLine("Upload Complete: \"" + blobName + "\" in container \"" + containerName + "\" from file \"" + fileName + "\"");
		}
	}
}
