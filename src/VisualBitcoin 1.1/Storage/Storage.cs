using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Configuration;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Table;

namespace Storage
{
	public interface IStorage
	{
		CloudBlobClient RetrieveBlobClient();
		CloudTableClient RetrieveTableClient();
		CloudQueueClient RetrieveQueueClient();
		void UploadContainer(CloudBlobClient blobClient, string containerName);
		void UploadBlob(CloudBlobClient blobClient, string containerName, string blobName, string fileName);
		void DownloadBlobToFile(CloudBlobClient blobClient, string containerName, string blobName, string destFileName);
		String DownloadBlobToString(CloudBlobClient blobClient, string containerName, string blobName);
		void DeleteBlob(CloudBlobClient blobClient, string containerName, string blobName);
		void DeleteContainer(CloudBlobClient blobClient, string containerName);
		CloudBlobContainer RetrieveContainer(CloudBlobClient blobClient, string containerName);
	}

	public class WindowsAzureStorage : IStorage
	{
		private static CloudStorageAccount _storageAccount;
		private static CloudBlobClient _blobClient;
		private static CloudTableClient _tableClient;
		private static CloudQueueClient _queueClient;


		// It will be better if the next constants are store in a configuration file.

		// Definition of all our table, container and queue.
		// Remember that names must be a v valid DNS name:
		// 1. Names must start with a letter or number, and can contain only
		// letters, numbers, and the dash (-) character.
		// 2. Every dash (-) character must be immediately preceded and followed by a
		// letter or number; consecutive dashes are not permitted in names.
		// 3. All letters in a name must be lowercase.
		// 4. Names must be from 3 through 63 characters long.
		private const string Table = "table";
		private const string Container = "container";
		private const string Queue = "queue";

		// Windows Azure Storage Emulator is used if true.
		private const bool UseDevelopmentStorage = true;


		// Set up the storage, only one call while application start.
		public static void SetUp()
		{
			if (null != _storageAccount)
			{
				throw new Exception("Windows Azure Storage Account can not be initialize twice.");
			}

			if (true == UseDevelopmentStorage)
			{
				_storageAccount = CloudStorageAccount.DevelopmentStorageAccount;
			}
			else
			{
				var configurationString = ConfigurationManager.ConnectionStrings["StorageConnectionString"].ConnectionString;
				_storageAccount = CloudStorageAccount.Parse(configurationString);
			}

			_blobClient = _storageAccount.CreateCloudBlobClient();
			var containerReference = _blobClient.GetContainerReference(Container);
			containerReference.CreateIfNotExists();

			_tableClient = _storageAccount.CreateCloudTableClient();
			var tableReference = _tableClient.GetTableReference(Table);
			tableReference.CreateIfNotExists();

			_queueClient = _storageAccount.CreateCloudQueueClient();
			var queueReference = _queueClient.GetQueueReference(Queue);
			queueReference.CreateIfNotExists();
		}

		// Retriving methods.
		public CloudBlobClient RetrieveBlobClient()
		{
			return _storageAccount.CreateCloudBlobClient();
		}

		public CloudTableClient RetrieveTableClient()
		{
			return _storageAccount.CreateCloudTableClient();
		}

		public CloudQueueClient RetrieveQueueClient()
		{
			return _storageAccount.CreateCloudQueueClient();
		}

		public static CloudTable GetTableReference()
		{
			var tableReference = _tableClient.GetTableReference(Table);
			return tableReference;
		}

		public void UploadContainer(CloudBlobClient blobClient, string containerName)
		{
			// Variables for the cloud storage objects.
			CloudBlobContainer blobContainer;

			// Get the container reference.
			blobContainer = blobClient.GetContainerReference(containerName);

			// Create the container if it does not exist.
			blobContainer.CreateIfNotExists();

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

		public void DownloadBlobToFile(CloudBlobClient blobClient, string containerName, string blobName, string destFileName)
		{
			CloudBlobContainer blobContainer;

			// Retrieve reference to a container.
			blobContainer = blobClient.GetContainerReference(containerName);

			// Retrieve reference to blobName.
			CloudBlockBlob blockBlob = blobContainer.GetBlockBlobReference(blobName);

			// Save blob contents to a file.
			using (var fileStream = System.IO.File.OpenWrite(@destFileName))
			{
				blockBlob.DownloadToStream(fileStream);
			}
		}

		public String DownloadBlobToString(CloudBlobClient blobClient, string containerName, string blobName)
		{
			// Retrieve reference to a previously created container.
			CloudBlobContainer container = blobClient.GetContainerReference(containerName);

			// Retrieve reference to a blob.
			CloudBlockBlob blockBlob = container.GetBlockBlobReference(blobName);

			string text = "";
			// using (var memoryStream = new MemoryStream())
			// {
			// blockBlob.DownloadToStream(memoryStream);
			// text = System.Text.Encoding.UTF8.GetString(memoryStream.ToArray());
			// }

			return text;
		}

		public void DeleteBlob(CloudBlobClient blobClient, string containerName, string blobName)
		{
			// Retrieve reference to a previously created container.
			CloudBlobContainer blobContainer = blobClient.GetContainerReference(containerName);

			// Retrieve reference to a blob.
			CloudBlockBlob blockBlob = blobContainer.GetBlockBlobReference(blobName);

			// Delete the blob.
			blockBlob.Delete();
		}

		public void DeleteContainer(CloudBlobClient blobClient, string containerName)
		{
			// Retrieve reference to a previously created container.
			CloudBlobContainer blobContainer = blobClient.GetContainerReference(containerName);

			// Delete the container
			blobContainer.Delete();
		}

		public CloudBlobContainer RetrieveContainer(CloudBlobClient blobClient, string containerName)
		{
			// Retrieve reference to a previously created container.
			return blobClient.GetContainerReference(containerName);
		}

		public List<string> RetrieveBlobsList(CloudBlobClient blobClient, string containerName)
		{
			// Retrieve reference to a previously created container.
			CloudBlobContainer blobContainer = blobClient.GetContainerReference(containerName);

			var blobsList = new List<string>();

			// Loop over blobs within the container and output the URI to each of them.
			foreach (var blobItem in blobContainer.ListBlobs())
			{
				char[] separator = new char[] { '/' };
				string blobUri = blobItem.Uri.ToString();
				string[] splitedBlobUri = blobUri.Split(separator);
				string blobName = splitedBlobUri[splitedBlobUri.Length - 1];
				CloudBlockBlob blockBlob = blobContainer.GetBlockBlobReference(blobName);
				string text;
				using (var memoryStream = new MemoryStream())
				{
					blockBlob.DownloadToStream(memoryStream);
					text = System.Text.Encoding.UTF8.GetString(memoryStream.ToArray());

				}
				blobsList.Add(text);
			}

			return blobsList;
		}
	}
}