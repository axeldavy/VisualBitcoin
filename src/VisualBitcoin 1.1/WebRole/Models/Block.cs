using System.ComponentModel.DataAnnotations;
using System.Globalization;
using Microsoft.WindowsAzure.Storage.Table;

namespace WebRole.Models
{
	// We model the schema of the blocks stored by our application for the table
	// storage.
	public class Block : TableEntity
	{
		// Default constructor for generic uses.
		public Block()
		{
			Hash = "default";
			Version = "default";
			PreviousBlock = "default";
			MerkleRoot = "default";
			Time = 0;
			Bits = 0;
			NumberOnce = 0;
			Size = 0;
			Index = 0;
			IsInMainChain = false;
			Height = 0;
			ReceivedTime = 0;
			RelayedBy = "";

			RowKey = "";
			PartitionKey = "";
		}

		// Main constructor.
		public Block(string hash, string version, string previousBlock,
			string merkleRoot, int time, int bits, int numberOnce, int size,
			int index, bool isInMainChain, int height, int receivedTime, string relayedBy)
		{
			Hash = hash;
			Version = version;
			PreviousBlock = previousBlock;
			MerkleRoot = merkleRoot;
			Time = time;
			Bits = bits;
			NumberOnce = numberOnce;
			Size = size;
			Index = index;
			IsInMainChain = isInMainChain;
			Height = height;
			ReceivedTime = receivedTime;
			RelayedBy = relayedBy;

			// In general, you choose the value of the PartitionKey to ensure load
			// balancing of the data across the storage nodes. I chose to associate
			// PartitionKey with time (30 days).
			PartitionKey = "partition" + (time / (60 * 60 * 24 * 30)).ToString(CultureInfo.InvariantCulture);

			// Tables within partitions are sorted in RowKey order.
			RowKey = "block" + index.ToString(CultureInfo.InvariantCulture);
		}

		// Properties to hold information about block.
		[Display(Name = "Hash")]
		public string Hash { get; set; }
		[Display(Name = "Version")]
		public string Version { get; set; }
		[Display(Name = "Previous block")]
		public string PreviousBlock { get; set; }
		[Display(Name = "Merkle root")]
		public string MerkleRoot { get; set; }
		[Display(Name = "Time")]
		public int Time { get; set; }
		[Display(Name = "Bits")]
		public int Bits { get; set; }
		[Display(Name = "Number once")]
		public int NumberOnce { get; set; }
		[Display(Name = "Number of transactions")]
		public int NumberOfTransactions { get; set; }
		[Display(Name = "Size")]
		public int Size { get; set; }
		[Display(Name = "Index")]
		public int Index { get; set; }
		[Display(Name = "Is in main chain")]
		public bool IsInMainChain { get; set; }
		[Display(Name = "Height")]
		public int Height { get; set; }
		[Display(Name = "Received time")]
		public int ReceivedTime { get; set; }
		[Display(Name = "Relayed by")]
		public string RelayedBy { get; set; }
	}
}
