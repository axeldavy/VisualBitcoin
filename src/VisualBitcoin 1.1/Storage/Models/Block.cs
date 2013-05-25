using System.Collections.Generic;

namespace Storage.Models
{
	public class Block
	{
		// Properties.
		public string Hash { get; set; }
		public string Version { get; set; }
		public string PreviousBlock { get; set; }
		public List<string> NextBlock { get; set; }
		public string MerkleRoot { get; set; }
		public int Time { get; set; }
		public long NumberOnce { get; set; }
		public int NumberOfTransactions { get; set; }
		public int Size { get; set; }
		public int Height { get; set; }
		public string[] TransactionIds { get; set; }
        public double Amount { get; set; }

		// Constructors.
		public Block()
		{
			Hash = "default";
			Version = "default";
			PreviousBlock = "default";
            NextBlock = new List<string>();
			MerkleRoot = "default";
			Time = 0;
			NumberOnce = 0;
			NumberOfTransactions = 0;
			Size = 0;
			Height = 0;
			TransactionIds = new string[0];
            Amount = 0;
		}

		public Block(string hash, string version, string previousBlock, List<string> nextBlock,
		             string merkleRoot, int time, long numberOnce, int numberOfTransactions, int size,
		             int height, string[] transactionIds, double amount)
		{
			Hash = hash;
			Version = version;
			PreviousBlock = previousBlock;
			NextBlock = nextBlock;
			MerkleRoot = merkleRoot;
			Time = time;
			NumberOnce = numberOnce;
			NumberOfTransactions = numberOfTransactions;
			Size = size;
			Height = height;
			TransactionIds = transactionIds;
            Amount = amount;
		}
	}
}
