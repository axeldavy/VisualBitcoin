namespace Storage.Models
{
    public class BitcoinWorkerRoleBackup
    {
        // Backup type-class for the BitnetWorkerRole.

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

        // Properties
        public int MaximumNumberOfBlocksInTheStorage;
        public int NumberOfBlocksInTheStorage;
        public string FirstBlockHash;
        public string LastBlockHash;
        public int MinimalHeight;
    }
}
