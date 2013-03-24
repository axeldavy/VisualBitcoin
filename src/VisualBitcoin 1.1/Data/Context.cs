using System.Linq;

namespace Data
{
	class Context : Microsoft.WindowsAzure.StorageClient.TableServiceContext
	{
		public Context(string baseAddress, Microsoft.WindowsAzure.StorageCredentials credentials)
			: base(baseAddress, credentials)
		{

		}

		public IQueryable<Entry> Entry
		{
			get { return this.CreateQuery<Entry>("Entry"); }
		}
	}
}
