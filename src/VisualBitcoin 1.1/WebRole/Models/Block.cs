using System.ComponentModel.DataAnnotations;
using Microsoft.WindowsAzure.Storage.Table;

namespace WebRole.Models
{
	public class Block : TableEntity
	{
		private static int _identifier = 0;

        public Block()
        {
	        var id = _identifier++;
	        this.RowKey = "block" + id.ToString();
        }

		[Display(Name = "Hash")]
		public string Hash { get; set; }
		[Display(Name = "Version")]
		public string Version { get; set; }
		[Display(Name = "Previous block")]
		public string PreviousBlock { get; set; }
		[Display(Name = "MerkleRoot")]
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
