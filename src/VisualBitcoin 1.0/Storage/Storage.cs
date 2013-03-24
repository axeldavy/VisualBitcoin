﻿using System;
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
	    void CreateIfNotExistsTableBlobsContainerQueue();
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
		// Definition of all our table, blob container and queue.
		// Remember that containers must be a v valid DNS name:
		//   1. Container names must start with a letter or number, and can contain only
		//      letters, numbers, and the dash (-) character.
		//   2. Every dash (-) character must be immediately preceded and followed by a
		//      letter or number; consecutive dashes are not permitted in container names.
		//   3. All letters in a container name must be lowercase.
		//   4. Container names must be from 3 through 63 characters long.
		public const string Table = "table";
		
		public const string BlobsContainer = "blobscontainer";
		
		public const string Queue = "queue";

		// Choose true if you want to work with the Compute Emulator and Storage Emulator.
		// Don't forget to launch them before lauching Visual Studio.
	    public const bool Emulated = true;


		CloudStorageAccount SelectStorageAccount()
		{
			CloudStorageAccount storageAccount;
			if (true == Emulated)
			{
				storageAccount = CloudStorageAccount.DevelopmentStorageAccount;
			}
			else
			{
				var configurationString = ConfigurationManager.ConnectionStrings["StorageConnectionString"].ConnectionString;
				storageAccount = CloudStorageAccount.Parse(configurationString);	
			}
			return storageAccount;
		}

		// Retriving methods.
	    public CloudBlobClient RetrieveBlobClient()
	    {
		    var storageAccount = SelectStorageAccount();
		    return storageAccount.CreateCloudBlobClient();
	    }

		public CloudTableClient RetrieveTableClient()
		{
			var storageAccount = SelectStorageAccount();
			return storageAccount.CreateCloudTableClient();
		}
		
		public CloudQueueClient RetrieveQueueClient()
		{
			var storageAccount = SelectStorageAccount();
			return storageAccount.CreateCloudQueueClient();
		}
		
		// Next method check if all of the table, queue and blob containers used in this
		// application exist, and create any that don't already exist.
		public void CreateIfNotExistsTableBlobsContainerQueue()
		{
			var tableClient = RetrieveTableClient();
			var tableContainer = tableClient.GetTableReference(Table);
			tableContainer.CreateIfNotExists();
			
			var blobClient = RetrieveBlobClient();
			var blobContainer = blobClient.GetContainerReference(BlobsContainer);
			blobContainer.CreateIfNotExists();
			
			var queueClient = RetrieveQueueClient();
			var queueContainer = queueClient.GetQueueReference(Queue);
			queueContainer.CreateIfNotExists();
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
//            using (var memoryStream = new MemoryStream())
//            {
//                blockBlob.DownloadToStream(memoryStream);
//                text = System.Text.Encoding.UTF8.GetString(memoryStream.ToArray());
//            }

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
