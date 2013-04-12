using System.Diagnostics;
using Microsoft.WindowsAzure.Storage;

namespace Storage
{
	public static class WindowsAzure
	{
		// Storage parameters.
		private const bool UseDevelopmentStorage = false;
		private const string ContainerName = "visualbitcoincontainer";
		private const string TableName = "visualbitcointable";
		private const string QueueName = "visualbitcoinqueue";

		// Already start flag.
		private static bool _isAlreadyStarted;

		// Property.
		public static CloudStorageAccount StorageAccount { get; private set; }



		// Configure and start the storage, only one call make by application.
		public static void Start(string connectionString)
		{
			if (false == _isAlreadyStarted)
			{
				Trace.WriteLine("Start", "VisualBitcoin.Storage.WindowsAzure Information");

				StorageAccount = UseDevelopmentStorage
									 ? CloudStorageAccount.DevelopmentStorageAccount
									 : CloudStorageAccount.Parse(connectionString);

				Blob.Start(ContainerName);
				Table.Start(TableName);
				Queue.Start(QueueName);

				_isAlreadyStarted = true;
			}
			else
			{
				Trace.WriteLine("Already started", "VisualBitcoin.Storage.WindowsAzure Warning");
			}
		}
	}
}
