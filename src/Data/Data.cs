using System.Collections.Generic;
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
				blobsList.Add(blobItem.Uri.ToString());
			}

			return blobsList;
		}
	}
}
