namespace Storage.Models
{
	public class BitnetBackup
	{
		// Backup type-class for the BitnetWorkerRole to remember the last block sent.

		// Property
		public string Hash { get; set; }
		public int Count { get; set; }


		// Constructors
		public BitnetBackup()
		{
			Hash = "default";
			Count = 0;
		}

		public BitnetBackup(string hash, int count)
		{
			Hash = hash;
			Count = count;
		}
	}
}
