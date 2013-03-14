using System;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;

namespace Storage
{
	// easy way: we choose how to connect (mockstorage or our windows azure storage)
    // and then we return a blobClient we can use as we want.
    // pros: fast to implement
    // cons: we'll need to give a documentation to explain the useful blob functions, how to use them...
    // Moreover the blobClient class can be used to get the credentials which we want to hide.
    // Even from CloudBlob we can get CloudBlobClient and then the credentials.
    public interface IStorageFirstProposition
	{
        CloudBlobClient GetBlobClient();
	}
    // second way : We write our functions that use the windows azure api
    // pros: easier/faster to use
    // cons: need to implement Ireader and Iwriter. 
    public interface IStorageSecondProposition
    {
        bool Isconnected();
        void Connect();
        Ireader open_read_file(string directory, string name);
        Iwriter open_write_file(string directory, string name);
        bool delete_file(string directory, string name); // 0: didn't exist. 1: success
        bool rename_file(string directory, string oldname, string newname);
        bool copy_file(string directory, string oldfile, string newfile); // if newfile exists, overwrite
        bool new_file(string directory, string name, ulong size);
        // Do we need more functionalities?
        // Maybe more advanced functionalities to take into account azure issues: sync, ...
    }
    
    
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
