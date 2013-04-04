namespace Storage.Models
{
	public class BlockReference
	{
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
