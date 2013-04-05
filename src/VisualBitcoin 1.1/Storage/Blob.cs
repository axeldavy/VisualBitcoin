using System;
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
			Trace.WriteLine("Start",
				"VisualBitcoin.Storage.Blob Information");

			CloudBlobClient = WindowsAzure.StorageAccount.CreateCloudBlobClient();
			CloudBlobContainer = CloudBlobClient.GetContainerReference(containerName);
			CloudBlobContainer.CreateIfNotExists();
		}
		
		// Upload a block Blob in the storage. It could be a good thing to declare all the 
		// (data) models we need in the dedicated folder "Models". All our models in one  
		// place.
		public static void UploadBlockBlob<TModel>(string blockBlobName, TModel model)
		{
			Trace.WriteLine("Upload",
				"VisualBitcoin.Storage.Blob Information");

			var text = Serialization.ToXml(model);
			var content = Coding.Code(text);
			var blockBlob = CloudBlobContainer.GetBlockBlobReference(blockBlobName);
			var buffer = Encoding.UTF8.GetBytes(content);
			var stream = new MemoryStream(buffer);

			blockBlob.UploadFromStream(stream);
		}

		// Download a blockBlob.
		public static TModel DownloadBlockBlob<TModel>(string blockBlobName) where TModel : class
		{
			Trace.WriteLine("Download",
				"VisualBitcoin.Storage.Blob Information");

			if (null == CloudBlobContainer)
				throw new Exception("Test");

			var cloudBlockBlob = CloudBlobContainer.GetBlockBlobReference(blockBlobName);
			var stream = new MemoryStream();
			cloudBlockBlob.DownloadToStream(stream);
			var buffer = stream.ToArray();
			var content = Encoding.UTF8.GetString(buffer);
			var text = Coding.Decode(content);
			var model = Serialization.FromXml<TModel>(text);
            
			return model;
		}

        //Deleting the blob from container
        public static void DeleteBlockBlob(string blockBlobName)
        {
            Trace.WriteLine("Delete", "VisualBitcoin.Storage.Blob Information");

            if (null == CloudBlobContainer)
                throw new Exception("Test");

            var cloudBlockBlob = CloudBlobContainer.GetBlockBlobReference(blockBlobName);
            cloudBlockBlob.Delete();
        }

		// Retrieve the example block instance.
		public static Block GetExampleBlock()
		{
			Trace.WriteLine("Example block download",
				"VisualBitcoin.Storage.Blob Information");

			var block = DownloadBlockBlob<Block>("block");
			return block;
		}

		// Retrieve a block instance.
		public static Block GetBlock(string blockName)
		{
			Trace.WriteLine("Block download",
				"VisualBitcoin.Storage.Blob Information");

			var block = DownloadBlockBlob<Block>(blockName);
			return block;
		}

        //Retrieve the list of blocks (where the blocks' name begin by "block" : to be modified !).
        public static List<string> GetBlockList()
        {
			Trace.WriteLine("Block list download",
				"VisualBitcoin.Storage.Blob Information");

			var blockList = CloudBlobContainer.ListBlobs(prefix: "block");
            var nameList = new List<string>();
            foreach (IListBlobItem blob in blockList)
            {
                nameList.Add(Path.GetFileNameWithoutExtension(blob.Uri.ToString()));
            }

            return nameList;
        }
	}
}
