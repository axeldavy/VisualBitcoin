using System.Diagnostics;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Storage
{
	class Blob
	{
		// Properties.
		public static CloudBlobClient CloudBlobClient { get; private set; }
		public static CloudBlobContainer CloudBlobContainer { get; private set; }


		// Configure and start the blob storage, only one call make on application start.
		public static void Start(string containerName)
		{
			Trace.WriteLine("Configure and start the blob storage");

			CloudBlobClient = WindowsAzure.StorageAccount.CreateCloudBlobClient();
			CloudBlobContainer = CloudBlobClient.GetContainerReference(containerName);
			CloudBlobContainer.CreateIfNotExists();
		}
	}
}
