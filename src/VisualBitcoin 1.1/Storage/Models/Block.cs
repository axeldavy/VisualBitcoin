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
		public long NumberOnce { get; set; }
		public int NumberOfTransactions { get; set; }
		public int Size { get; set; }
		public int Height { get; set; }
        public int ReceivedTime { get; set; }
		public string RelayedBy { get; set; }
		public string[] TransactionIds { get; set; }



		// Constructors.
		public Block()
		{
			Hash = "default";
			Version = "default";
			PreviousBlock = "default";
            NextBlock = "default";
			MerkleRoot = "default";
			Time = 0;
			NumberOnce = 0;
			NumberOfTransactions = 0;
			Size = 0;
			Height = 0;
            ReceivedTime = 0;
			RelayedBy = "";
			TransactionIds = new string[0];
		}

		public Block(string hash, string version, string previousBlock, string nextBlock,
		             string merkleRoot, int time, long numberOnce, int numberOfTransactions, int size,
		             int height, int receivedTime, string relayedBy, string[] transactionIds)
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
            ReceivedTime = receivedTime;
			RelayedBy = relayedBy;
			TransactionIds = transactionIds;
		}
	}
}
