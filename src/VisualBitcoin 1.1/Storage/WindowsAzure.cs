using System.Diagnostics;
using Microsoft.WindowsAzure.Storage;

namespace Storage
{
	public class WindowsAzure
	{
		// Storage parameters.
		private const string DefaultContainerName = "defaultcontainer";
		private const string BlocksContainerName = "blockscontainer";
		private const string TransactionsContainerName = "transactionscontainer";
		private const string StatContainerName = "statcontainer";
		private const string TableName = "visualbitcointable";
		private const string QueueName = "visualbitcoinqueue";

		private bool _isStopped = true;

        private CloudStorageAccount storageAccount;
        private Blob blob;
        private Queue queue;

		// Configure and start the storage, only one call make by application.
		public WindowsAzure(string connectionString, bool resetBlobBlocksEnable, bool resetQueueMessagesEnable)
		{
			if (_isStopped)
			{
				Trace.WriteLine("Start", "VisualBitcoin.Storage.WindowsAzure Information");
                storageAccount = CloudStorageAccount.Parse(connectionString);

                blob = new Blob(storageAccount, DefaultContainerName, BlocksContainerName, TransactionsContainerName, StatContainerName);
                queue = new Queue(storageAccount.CreateCloudQueueClient(), QueueName); 

				if (resetBlobBlocksEnable)
					// blob.Reset(); TODO: Find alternative way to handle this situation

				if (resetQueueMessagesEnable)
					//Queue.Reset(resetBlobBlocksEnable);

				_isStopped = false;
			}
			else
			{
				Trace.WriteLine("Already started", "VisualBitcoin.Storage.WindowsAzure Warning");
			}
		}
	}
}
