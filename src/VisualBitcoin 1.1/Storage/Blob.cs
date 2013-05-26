using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.WindowsAzure.Storage.Blob;
using Storage.Models;

namespace Storage
{
	public class Blob
	{
		// Properties.
		public static CloudBlobClient CloudBlobClient { get; private set; }
		public static CloudBlobContainer DefaultContainer { get; private set; }
		public static CloudBlobContainer BlocksContainer { get; private set; }
		public static CloudBlobContainer TransactionsContainer { get; private set; }
		public static CloudBlobContainer StatContainer { get; private set; }


		// Configure and start the blob storage, only one call make on application start.
		public static void Start(string defaultContainerName, string blocksContainerName, string transactionsContainerName, string highContainerName)
		{
			Trace.WriteLine("Start", "VisualBitcoin.Storage.Blob Information");

			CloudBlobClient = WindowsAzure.StorageAccount.CreateCloudBlobClient();
			DefaultContainer = CloudBlobClient.GetContainerReference(defaultContainerName);
			DefaultContainer.CreateIfNotExists();
			BlocksContainer = CloudBlobClient.GetContainerReference(blocksContainerName);
			BlocksContainer.CreateIfNotExists();
			TransactionsContainer = CloudBlobClient.GetContainerReference(transactionsContainerName);
			TransactionsContainer.CreateIfNotExists();
			StatContainer = CloudBlobClient.GetContainerReference(highContainerName);
			StatContainer.CreateIfNotExists();
		}

		// Delete all blocks stored in the blocks container.
		public static void Reset()
		{
			Trace.WriteLine("Reset", "VisualBitcoin.Storage.Blob Information");

			var blockList = GetBlobBlocksList(BlocksContainer);
			foreach (var blockName in blockList)
				DeleteBlockBlob<Block>(blockName);

			var transactionList = GetBlobBlocksList(TransactionsContainer);
			foreach (var transactionName in transactionList)
				DeleteBlockBlob<Transaction>(transactionName);

			var highList = GetBlobBlocksList(StatContainer);
			foreach (var highName in highList)
				DeleteBlockBlob<Block>(highName);

			DeleteBlockBlob<BitcoinWorkerRoleBackup>("bitcoinworkerrolebackup");
		}

		// Upload a block Blob in the storage. It could be a good thing to declare all the 
		// (data) models we need in the dedicated folder "Models". All our models in one  
		// place.
		public static void UploadBlockBlob<TModel>(string blockBlobName, TModel model)
		{
			Trace.WriteLine("Upload", "VisualBitcoin.Storage.Blob Information");

			var text = Serialization.ToXml(model);
			var content = Coding.Code(text);

			CloudBlockBlob blockBlob;
			if (model is Block)
			{
				blockBlob = BlocksContainer.GetBlockBlobReference(blockBlobName);
			}
			else if (model is Transaction)
			{
				blockBlob = TransactionsContainer.GetBlockBlobReference(blockBlobName);
			}
            else if ((model is List<Block>) || (model is Statistics) || (model is List<int>) || (model is List<double>))
			{
				blockBlob = StatContainer.GetBlockBlobReference(blockBlobName);
			}
			else
			{
				blockBlob = DefaultContainer.GetBlockBlobReference(blockBlobName);
			}

			var buffer = Encoding.UTF8.GetBytes(content);
			var stream = new MemoryStream(buffer);

			blockBlob.UploadFromStream(stream);
		}

		// Download a blockBlob.
		public static TModel DownloadBlockBlob<TModel>(string blockBlobName) where TModel : class
		{
			Trace.WriteLine("Download", "VisualBitcoin.Storage.Blob Information");

			CloudBlockBlob cloudBlockBlob;

			if (typeof(TModel) == typeof(Block))
			{
				cloudBlockBlob = BlocksContainer.GetBlockBlobReference(blockBlobName);
			}
			else if (typeof(TModel) == typeof(Transaction))
			{
				cloudBlockBlob = TransactionsContainer.GetBlockBlobReference(blockBlobName);
			}
            else if ((typeof(TModel) == typeof(List<Block>)) || (typeof(TModel) == typeof(Statistics)) || (typeof(TModel) == typeof(List<int>)) || (typeof(TModel) == typeof(List<double>)))
			{
				cloudBlockBlob = StatContainer.GetBlockBlobReference(blockBlobName);
			}
			else
			{
				cloudBlockBlob = DefaultContainer.GetBlockBlobReference(blockBlobName);
			}

			var stream = new MemoryStream();
			try
			{
				cloudBlockBlob.DownloadToStream(stream);
			}
			catch (Exception e)
			{
				Trace.WriteLine("Warning", e.Message);
				return null;
			}
			var buffer = stream.ToArray();
			var content = Encoding.UTF8.GetString(buffer);
			var text = Coding.Decode(content);
			var model = Serialization.FromXml<TModel>(text);

			return model;
		}

		//Deleting the blob from container
		public static void DeleteBlockBlob<TModel>(string blockBlobName) where TModel : class
		{
			Trace.WriteLine("Delete", "VisualBitcoin.Storage.Blob Information");

			CloudBlockBlob cloudBlockBlob;

			if (typeof(TModel) == typeof(Block))
			{
				cloudBlockBlob = BlocksContainer.GetBlockBlobReference(blockBlobName);
			}
			else if (typeof(TModel) == typeof(Transaction))
			{
				cloudBlockBlob = TransactionsContainer.GetBlockBlobReference(blockBlobName);
			}
			else if (typeof(TModel) == typeof(List<Block>))
			{
				cloudBlockBlob = StatContainer.GetBlockBlobReference(blockBlobName);
			}
			else
			{
				cloudBlockBlob = DefaultContainer.GetBlockBlobReference(blockBlobName);
			}

			cloudBlockBlob.DeleteIfExists();
		}

		// Retrieve the example block instance.
		public static Block GetExampleBlock()
		{
			Trace.WriteLine("Example block download", "VisualBitcoin.Storage.Blob Information");

			var block = DownloadBlockBlob<Block>("block");
			return block;
		}

		// Retrieve a block instance.
		public static Block GetBlock(string blockName)
		{
			Trace.WriteLine("Block download", "VisualBitcoin.Storage.Blob Information");

			var block = DownloadBlockBlob<Block>(blockName);
			return block;
		}

        public static Transaction GetTransaction(string tranName)
        {
            Trace.WriteLine("Transaction download", "VisualBitcoin.Storage.Blob Information");

            var transaction = DownloadBlockBlob<Transaction>(tranName);
            return transaction;
        }


		//Retrieve the list of block blobs in a container.
		public static List<string> GetBlobBlocksList(CloudBlobContainer cloudBlobContainer)
		{
			Trace.WriteLine("Blob block list download", "VisualBitcoin.Storage.Blob Information");

			var blockList = cloudBlobContainer.ListBlobs();

			return blockList.Select(blob => blob.Uri.ToString()).ToList();
		}

        public static List<string> GetTransactionList()
        {
            Trace.WriteLine("Transaction list download", "VisualBitcoin.Storage.Blob Information");

            var blobTransactionList = GetBlobBlocksList(TransactionsContainer);
            return blobTransactionList;
        }

		public static List<string> GetBlockList()
		{
			Trace.WriteLine("Block list download", "VisualBitcoin.Storage.Blob Information");
			var blobBlocksList = GetBlobBlocksList(BlocksContainer);

			return blobBlocksList;
		}
	}
}
