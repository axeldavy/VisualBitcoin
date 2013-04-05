namespace Storage.Models
{
	public class BlockReference
	{
		// BlockReference type-class for the queue, allow the BitnetWorkerRole to send news
		// to the StatWorkerRole.

		// Property.
		public string Hash { get; set; }


		// Constructors.
		public BlockReference()
		{
			Hash = "default";
		}

		public BlockReference(string hash)
		{
			Hash = hash;
		}
	}
}
