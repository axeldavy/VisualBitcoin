using System.Diagnostics;
using Microsoft.WindowsAzure.Storage;

namespace Storage
{
	public static class WindowsAzure
	{
		// Storage parameters.
		private const string DefaultContainerName = "defaultcontainer";
		private const string BlocksContainerName = "blockscontainer";
		private const string TableName = "visualbitcointable";
		private const string QueueName = "visualbitcoinqueue";
		private const bool ResetBlobBlocksEnable = false;
		private const bool ResetQueueMessagesEnable = false;

		// Already start flag.
		private static bool _isNotAlreadyStarted = true;

		// Property.
		public static CloudStorageAccount StorageAccount { get; private set; }



		// Configure and start the storage, only one call make by application.
		public static void Start(string connectionString)
		{
			if (_isNotAlreadyStarted)
			{
				Trace.WriteLine("Start", "VisualBitcoin.Storage.WindowsAzure Information");

				StorageAccount = CloudStorageAccount.Parse(connectionString);

				Blob.Start(DefaultContainerName, BlocksContainerName);
				Table.Start(TableName);
				Queue.Start(QueueName);

				if (ResetBlobBlocksEnable)
				{
					Blob.Reset();
				}

				if (ResetQueueMessagesEnable)
				{
					Queue.Reset();
				}

				_isNotAlreadyStarted = false;
			}
			else
			{
				Trace.WriteLine("Already started", "VisualBitcoin.Storage.WindowsAzure Warning");
			}
		}
	}
}
