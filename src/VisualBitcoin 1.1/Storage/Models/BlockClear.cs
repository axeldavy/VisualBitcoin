namespace Storage.Models
{
    /*this class without transactions, just thier IDs*/
    public class BlockClear
    {
  		public BlockClear()
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
			TransactionsIDs = new string [0];
		}
		
		public BlockClear(string hash, string version, string previousBlock, string nextBlock,
			string merkleRoot, int time, int bits, int numberOnce, int numberOfTransactions, int size,
			int index, bool isInMainChain, int height, int receivedTime, string relayedBy, string[] transactionsIDs)
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
			TransactionsIDs = transactionsIDs;
		}

        public BlockClear(Block _block)
        {
			Hash = _block.Hash;
			Version = _block.Version;
			PreviousBlock = _block.PreviousBlock;
            NextBlock = _block.NextBlock;
			MerkleRoot = _block.MerkleRoot;
			Time = _block.Time;
			Bits = _block.Bits;
			NumberOnce = _block.NumberOnce;
			NumberOfTransactions = _block.NumberOfTransactions;
			Size = _block.Size;
			Index = _block.Index;
			IsInMainChain = _block.IsInMainChain;
			Height = _block.Height;
			ReceivedTime = _block.ReceivedTime;
			RelayedBy = _block.RelayedBy;
            TransactionsIDs = new string [NumberOfTransactions];
			//Getting transactions ID
            int i = 0;
            foreach (Transactions currentTr in _block.Transactions)
            {
                TransactionsIDs[i] = currentTr.Txid;
                ++i;
            }
        }

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
	    public string[] TransactionsIDs { get; set; }
	}

}
