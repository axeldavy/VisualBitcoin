using System;
using System.Diagnostics;
using Microsoft.WindowsAzure.Storage;

namespace Storage
{
	public static class WindowsAzure
	{
		// Property.
		public static CloudStorageAccount StorageAccount { get; private set; }


		// Configure and start the storage, only one call make on application start.
		public static void Start(bool useDevelopmentStorage, string connectionString,
			string containerName, string tableName, string queueName)
		{
			Trace.WriteLine("Configure and start the storage");

			if (null != StorageAccount)
				throw new Exception("VisualBitcoin storage can not be initialize twice.");

			StorageAccount = useDevelopmentStorage ?
				CloudStorageAccount.DevelopmentStorageAccount : CloudStorageAccount.Parse(connectionString);

			Blob.Start(containerName);
			Table.Start(tableName);
			Queue.Start(queueName);
		}
	}
}
