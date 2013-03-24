using System;
using Microsoft.WindowsAzure.StorageClient;

namespace Data
{
	// In Data project, we model the schema of the entities stored by our application
	// and create a context class to use WCF Data Services to access the information in
	// table storage. To complete the project, we create an object that can be data
	// bounds to data controls in ASP.NET and implements the basic data access
	// operations : read, update and delete.

	// /!\ I don't know if sealed is really required. (Baptiste)
	public sealed class Entry : Microsoft.WindowsAzure.StorageClient.TableServiceEntity
	{
		public Entry()
		{
			PartitionKey = DateTime.UtcNow.ToString("YYYYMMDD");
			// We use the date of the entry as the PartitionKey, which means that there
			// will be a separate partition for each day of blockchain entries. In
			// general, you choose the value of the partition key to ensure load
			// balancing of the data across the storage nodes.
			
			RowKey = string.Format("{0:10}_{1}", DateTime.MaxValue.Ticks - DateTime.Now.Ticks, Guid.NewGuid());
			// Tables within partitions are sorted in RowKey order, so this will sort
			// the tables with the newest entry shown at the top.
		}

		// Properties to hold information about entry

		public string Hash { get; set; }
		public string Version { get; set; }
		public string PreviousBlock { get; set; }
		public string MerkleRoot { get; set; }
		public int Time { get; set; }
		public int Bits { get; set; }
		public int NumberOnce { get; set; }
		public int NumberOfTransactions { get; set; }
		public int Size { get; set; }
		public int Index { get; set; }
		public bool IsInMainChain { get; set; }
		public int Height { get; set; }
		public int ReceivedTime { get; set; }
		public string RelayedBy { get; set; }

		// We must change the schema of the entities stored in our application to support
		// transaction entities.
		//
		// public Transaction[] Transactions { get; set; }
	}
}
