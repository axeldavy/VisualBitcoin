namespace Storage.Models
{
	public class BitcoinWorkerRoleBackup
	{
		// Backup type-class for the BitnetWorkerRole.

		// Properties
		public int MaximumNumberOfBlocksInTheStorage { get; set; }
		public int NumberOfBlocksInTheStorage { get; set; }
		public string FirstBlockHash { get; set; }
		public string LastBlockHash { get; set; }

		// Constructors
		public BitcoinWorkerRoleBackup()
		{
			MaximumNumberOfBlocksInTheStorage = 0;
			NumberOfBlocksInTheStorage = 0;
			FirstBlockHash = "";
			LastBlockHash = "";
		}
		
		public BitcoinWorkerRoleBackup(int maximumNumberOfBlocksInTheStorage,
			int numberOfBlocksInTheStorage, string firstBlockHash, string lastBlockHash)
		{
			MaximumNumberOfBlocksInTheStorage = maximumNumberOfBlocksInTheStorage;
			NumberOfBlocksInTheStorage = numberOfBlocksInTheStorage;
			FirstBlockHash = firstBlockHash;
			LastBlockHash = lastBlockHash;
		}
	}
}