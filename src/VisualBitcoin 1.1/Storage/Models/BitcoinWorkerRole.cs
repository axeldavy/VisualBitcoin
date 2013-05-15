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
        public int MinimalHeight { get; set; }

		// Constructors
		public BitcoinWorkerRoleBackup()
		{
			MaximumNumberOfBlocksInTheStorage = 0;
			NumberOfBlocksInTheStorage = 0;
			FirstBlockHash = "";
			LastBlockHash = "";
            MinimalHeight = 0;
		}

		public BitcoinWorkerRoleBackup(int maximumNumberOfBlocksInTheStorage,
									   int numberOfBlocksInTheStorage,
									   string firstBlockHash,
									   string lastBlockHash, 
                                       int minimalHeight)
		{
			MaximumNumberOfBlocksInTheStorage = maximumNumberOfBlocksInTheStorage;
			NumberOfBlocksInTheStorage = numberOfBlocksInTheStorage;
			FirstBlockHash = firstBlockHash;
			LastBlockHash = lastBlockHash;
            MinimalHeight = minimalHeight;
		}
	}
}