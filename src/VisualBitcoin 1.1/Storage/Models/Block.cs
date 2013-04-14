namespace Storage.Models
{
	public class Block
	{
		// Properties.
		public string Hash { get; set; }
		public string Version { get; set; }
		public string PreviousBlock { get; set; }
		public string NextBlock { get; set; }
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
		public Transactions[] Transactions { get; set; }



		// Constructors.
		public Block()
		{
			Hash = "default";
			Version = "default";
			PreviousBlock = "default";
            NextBlock = "default";
			MerkleRoot = "default";
			Time = 0;
			Bits = 0;
			NumberOnce = 0;
			NumberOfTransactions = 0;
			Size = 0;
			Index = 0;
			IsInMainChain = false;
			Height = 0;
			ReceivedTime = 0;
			RelayedBy = "";
			Transactions = new Transactions[0];
		}

		public Block(string hash, string version, string previousBlock, string nextBlock,
		             string merkleRoot, int time, int bits, int numberOnce, int numberOfTransactions, int size,
		             int index, bool isInMainChain, int height, int receivedTime, string relayedBy,
		             Transactions[] transactions)
		{
			Hash = hash;
			Version = version;
			PreviousBlock = previousBlock;
			NextBlock = nextBlock;
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
			Transactions = transactions;
		}
	}
}
