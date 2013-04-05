using System.Diagnostics;
using Microsoft.WindowsAzure.Storage.Table;

namespace Storage
{
	public class Table
	{
		// Properties.
		public static CloudTableClient CloudTableClient { get; private set; }
		public static CloudTable CloudTable { get; private set; }


		// Configure and start the table storage, only one call make on application start.
		public static void Start(string tableName)
		{
			Trace.WriteLine("On start",
				"VisualBitcoin.Storage.Table Information");

			CloudTableClient = WindowsAzure.StorageAccount.CreateCloudTableClient();
			CloudTable = CloudTableClient.GetTableReference(tableName);
			CloudTable.CreateIfNotExists();
		}
	}
}
