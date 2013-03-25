using System.ComponentModel.DataAnnotations;

namespace WebRole.Models
{
	public class BlockModel
	{
		// Constructor.
		public BlockModel(string hash, string version, string previousBlock,
			string merkleRoot, int time, int bits, int numberOnce, int numberOfTransactions,
			int size, int index, bool isInMainChain, int height, int receivedTime, string relayedBy)
		{
			Hash = hash;
			Version = version;
			PreviousBlock = previousBlock;
			MerkleRoot = merkleRoot;
			Time = time;
			Bits = bits;
			NumberOnce = numberOnce;
			NumberOfTransactions = numberOfTransactions;
			Size = size;
			Index = index;
			IsInMainChain = isInMainChain;
			Height = height;
			ReceivedTime = receivedTime;
			RelayedBy = relayedBy;
			// Transactions = ?;
		}

		// Properties.
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
		// [Display(Name = "Transactions")]
		// public ? Transactions { get; set; }
	}
}
