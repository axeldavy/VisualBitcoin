using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using Microsoft.WindowsAzure.Storage.Blob;
using Storage.Models;

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
		
		// Upload a block Blob in the storage. It could be a good thing to declare all the 
		// (data) models we need in the dedicated folder "Models". All our models in one  
		// place.
		public static void UploadBlockBlob<TModel>(string blockBlobName, TModel model) where TModel : class
		{
			Trace.WriteLine("Upload blockBlob");

			var text = Serialization.ToXml(model);
			var content = Coding.Code(text);
			var blockBlob = CloudBlobContainer.GetBlockBlobReference(blockBlobName);
			var buffer = Encoding.UTF8.GetBytes(content);
			var stream = new MemoryStream(buffer);

			blockBlob.UploadFromStream(stream);
		}

		// Download a blockBlob.
		private static TModel DownloadBlockBlob<TModel>(string blockBlobName) where TModel : class
		{
			Trace.WriteLine("Retrieve a blockBlob");

			var cloudBlockBlob = CloudBlobContainer.GetBlockBlobReference(blockBlobName);
			var stream = new MemoryStream();
			cloudBlockBlob.DownloadToStream(stream);
			var buffer = stream.ToArray();
			var content = Encoding.UTF8.GetString(buffer);
			var text = Coding.Decode(content);
			var model = Serialization.FromXml<TModel>(text);

			return model;
		}

		// Retrieve the example block instance.
		public static Block GetExampleBlock()
		{
			Trace.WriteLine("Get example block instance.");

			var block = DownloadBlockBlob<Block>("block");
			return block;
		}

		// Retrieve a block instance.
		public static Block GetBlock(string blockName)
		{
			Trace.WriteLine("Get a block instance.");

			var block = DownloadBlockBlob<Block>(blockName);
			return block;
		}

        //Retrieve the list of blocks (where the blocks' name begin by "block" : to be modified !).
        public static List<string> GetBlockList()
        {
            var blockList = CloudBlobContainer.ListBlobs(prefix: "block");
            var nameList = new List<string>();
            foreach (IListBlobItem blob in blockList)
            {
                Trace.WriteLine("Here is one of the blocks :");
                // Trace.WriteLine(Path.GetFileNameWithoutExtension(blob.Uri.ToString()));
                nameList.Add(Path.GetFileNameWithoutExtension(blob.Uri.ToString()));
            }
            return nameList;
        }
	}
}
