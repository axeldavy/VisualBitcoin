using System.ComponentModel.DataAnnotations;

namespace WebRole.Models
{
	public class BlockModel
	{
		// Constructor.
		public BlockModel(string hash, string version, string previousBlock,
			string merkleRoot, int time, long numberOnce, int numberOfTransactions,
			int size, int height, double amount)
		{
			Hash = hash;
			Version = version;
			PreviousBlock = previousBlock;
			MerkleRoot = merkleRoot;
			Time = time;
			NumberOnce = numberOnce;
			NumberOfTransactions = numberOfTransactions;
			Size = size;
			Height = height;
		    Amount = amount;
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
		[Display(Name = "Number once")]
		public long NumberOnce { get; set; }
		[Display(Name = "Number of transactions")]
		public int NumberOfTransactions { get; set; }
		[Display(Name = "Size")]
		public int Size { get; set; }
		[Display(Name = "Height")]
		public int Height { get; set; }
        [Display(Name = "Amount")]
        public double Amount { get; set; }
	}
}
