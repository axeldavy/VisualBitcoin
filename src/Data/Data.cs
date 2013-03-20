using System.Collections.Generic;
using System.IO;
using Microsoft.WindowsAzure.Storage.Blob;
using Storage;

namespace Data
{
	public interface IData
	{
		List<string> RetrieveBlobsList(string containerName);
	}

	public class Data : IData
	{
		public WindowsAzureStorage BlobStorage = new WindowsAzureStorage();
		
		public List<string> RetrieveBlobsList(string containerName)
		{
			// Retrieve reference to a previously created container.
			CloudBlobClient emulatedBlobClient = BlobStorage.RetrieveEmulatedBlobClient();
			CloudBlobContainer blobContainer = BlobStorage.RetrieveContainer(emulatedBlobClient, containerName);

			var blobsList = new List<string>();
			
			// Loop over blobs within the container and output the URI to each of them.
			foreach (var blobItem in blobContainer.ListBlobs())
			{
				char[] separator = new char[]{'/'};
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
