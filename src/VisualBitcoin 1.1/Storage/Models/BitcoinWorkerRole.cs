namespace Storage.Models
{
	public class BitcoinWorkerRoleBackup
	{
		// Backup type-class for the BitnetWorkerRole.

		// Properties
		public int NumberOfBlocksInTheStorage { get; set; }
		public string FirstBlockHash { get; set; }
		public string LastBlockHash { get; set; }

		// Constructors
		public BitcoinWorkerRoleBackup()
		{
			NumberOfBlocksInTheStorage = 0;
			FirstBlockHash = "";
			LastBlockHash = "";
		}
		
		public BitcoinWorkerRoleBackup(int numberOfBlocksInTheStorage, string firstBlockHash, string lastBlockHash)
		{
			NumberOfBlocksInTheStorage = numberOfBlocksInTheStorage;
			FirstBlockHash = firstBlockHash;
			LastBlockHash = lastBlockHash;
		}
	}
}