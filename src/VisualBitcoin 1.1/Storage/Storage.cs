using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Storage
{
	public interface IStorage
	{
		void UploadContainer(CloudBlobClient blobClient, string containerName);
		void UploadBlob(CloudBlobClient blobClient, string containerName, string blobName, string fileName);
		void DownloadBlobToFile(CloudBlobClient blobClient, string containerName, string blobName, string destFileName);
		String DownloadBlobToString(string blobName);
		void DeleteBlob(CloudBlobClient blobClient, string containerName, string blobName);
		void DeleteContainer(CloudBlobClient blobClient, string containerName);
		CloudBlobContainer RetrieveContainer(CloudBlobClient blobClient, string containerName);
	}

	public class Storage : IStorage
	{
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
			using (var fileStream = File.OpenRead(@fileName))
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
			using (var fileStream = File.OpenWrite(@destFileName))
			{
				blockBlob.DownloadToStream(fileStream);
			}
		}

		public String DownloadBlobToString(string blobName)
		{
			// Retrieve reference to a blob.
			CloudBlockBlob blockBlob = Blob.CloudBlobContainer.GetBlockBlobReference(blobName);

			string text;
			using (var memoryStream = new MemoryStream())
			{
				blockBlob.DownloadToStream(memoryStream);
				text = System.Text.Encoding.UTF8.GetString(memoryStream.ToArray());
			}
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
				var separator = new[] { '/' };
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