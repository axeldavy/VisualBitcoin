using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage;
using Storage.Models;

namespace Storage
{
	public class Blob
	{
		// Properties.
        private CloudBlobClient CloudBlobClient;
        private CloudBlobContainer DefaultContainer;
        private CloudBlobContainer BlockContainer;
        private CloudBlobContainer TransactionContainer;
        private CloudBlobContainer StatContainer;

        // Storage parameters.
        private const string defaultContainerName = "defaultcontainer";
        private const string blockContainerName = "blockscontainer";
        private const string transactionContainerName = "transactionscontainer";
        private const string statContainerName = "statcontainer";
        private const string tableName = "visualbitcointable";

		public Blob(CloudStorageAccount storageAccount)
		{
			Trace.WriteLine("Start", "VisualBitcoin.Storage.Blob Information");

			CloudBlobClient = storageAccount.CreateCloudBlobClient();
			DefaultContainer = CloudBlobClient.GetContainerReference(defaultContainerName);
			DefaultContainer.CreateIfNotExists();
			BlockContainer = CloudBlobClient.GetContainerReference(blockContainerName);
			BlockContainer.CreateIfNotExists();
			TransactionContainer = CloudBlobClient.GetContainerReference(transactionContainerName);
			TransactionContainer.CreateIfNotExists();
			StatContainer = CloudBlobClient.GetContainerReference(statContainerName);
			StatContainer.CreateIfNotExists();
		}



		public void UploadBlock<TModel>(string name, TModel model)
		{
            CloudBlockBlob blockBlob = BlockContainer.GetBlockBlobReference(name);
            UploadFromBlockBlob(blockBlob, model);
        }			

        public void UploadStatistics<TModel>(string name, TModel model)
        {
		    CloudBlockBlob blockBlob = StatContainer.GetBlockBlobReference(name);
            UploadFromBlockBlob(blockBlob, model);
		}

        public void UploadTransaction<TModel>(string name, TModel model)
        {
            CloudBlockBlob blockBlob = TransactionContainer.GetBlockBlobReference(name);
            UploadFromBlockBlob(blockBlob, model);
        }

        public void UploadBackup<TModel>(TModel model)
        { 
            CloudBlockBlob blockBlob = DefaultContainer.GetBlockBlobReference("bitcoinworkerrolebackup");
            UploadFromBlockBlob(blockBlob, model);
        }

        public void UploadFromBlockBlob<TModel>(CloudBlockBlob blockBlob, TModel model)
        {
            var text = Serialization.ToXml(model);
            var content = Coding.Code(text);
            var buffer = Encoding.UTF8.GetBytes(content);
            blockBlob.UploadFromStream(new MemoryStream(buffer));
        }

        public TModel DownloadFromBlockBlob<TModel>(CloudBlockBlob blockBlob) where TModel : class
        {
            var stream = new MemoryStream();
            try
            {
                blockBlob.DownloadToStream(stream);
            }
            catch (Exception e)
            {
                Trace.WriteLine("Warning: Blob not found ", e.Message);
                return null;
            }
            var buffer = stream.ToArray();
            var content = Encoding.UTF8.GetString(buffer);
            var text = Coding.Decode(content);
            return Serialization.FromXml<TModel>(text);
        }

		public Block GetBlock(string name) 
		{
			CloudBlockBlob blockBlob = BlockContainer.GetBlockBlobReference(name);
            return DownloadFromBlockBlob<Block>(blockBlob);
        }
        
        public TModel GetStatistics<TModel>(string name) where TModel : class
        {
            CloudBlockBlob blockBlob = StatContainer.GetBlockBlobReference(name);
            return DownloadFromBlockBlob<TModel>(blockBlob);
        }

        public BitcoinWorkerRoleBackup GetBackup()
        {
            CloudBlockBlob blockBlob = DefaultContainer.GetBlockBlobReference("bitcoinworkerrolebackup");
            return DownloadFromBlockBlob<BitcoinWorkerRoleBackup>(blockBlob);
        }

        public Transaction GetTransaction(string name)
	    {
	        CloudBlockBlob blockBlob = TransactionContainer.GetBlockBlobReference(name);
	        return DownloadFromBlockBlob<Transaction>(blockBlob);
	    }

        public List<Transaction> GetTransactionList(int max)
	    {
            var blobList = TransactionContainer.ListBlobs();
            List<string> stringlist = blobList.Select(blob => blob.Uri.ToString()).ToList();

	        List<Transaction> translist = new List<Transaction>();
	
	        for(int i = 0; i < max; i++)
	        {
	            translist.Add(GetTransaction(stringlist.ElementAt(i)));
	        }
	        return translist;
	    }


	}
}
