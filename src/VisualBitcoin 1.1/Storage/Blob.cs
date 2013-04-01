using System.Diagnostics;
using System.IO;
using System.Text;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Storage
{
	public class Blob
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

		// Retrieve a blockBlob.
		private static string DownloadBlockBlob(string blockBlobName)
		{
			Trace.WriteLine("Retrieve a blockBlob");

			var cloudBlockBlob = CloudBlobContainer.GetBlockBlobReference(blockBlobName);
			var memoryStream = new MemoryStream();
			cloudBlockBlob.DownloadToStream(memoryStream);
			var text = Encoding.UTF8.GetString(memoryStream.ToArray());

			return text;
		}

		// Retrieve the example block instance.
		public static Block GetExampleBlock()
		{
			Trace.WriteLine("Get example block instance.");

			var blob = DownloadBlockBlob("block");
			var block = Serialization.FromXml<Block>(blob);
			return block;
		}

		// Retrieve a block instance.
		public static Block GetBlock(string blockName)
		{
			Trace.WriteLine("Get a block instance.");

			var blockBlob = DownloadBlockBlob(blockName);
			var block = Serialization.FromXml<Block>(blockBlob);
			return block;
		}
	}
}
