using System.Linq;

namespace Data
{
	class Context : Microsoft.WindowsAzure.StorageClient.TableServiceContext
	{
		public Context(string baseAddress, Microsoft.WindowsAzure.StorageCredentials credentials)
			: base(baseAddress, credentials)
		{

		}

		public IQueryable<Block> Block
		{
			get { return CreateQuery<Block>("Block"); }
		}
	}
}
