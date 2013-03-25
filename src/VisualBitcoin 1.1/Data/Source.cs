using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;

namespace Data
{
	public class Source
	{
		// We create an object that implements the basic data access operations : read,
		// update and delete.

		// /!\ I don't know if readonly is required. (Baptiste)
		private static readonly CloudStorageAccount StorageAccount;
		private readonly Context _context;

		// The static constructor initializes the storage account by reading its settings
		// from the configuration and then create the tables used by the application from
		// the model defined by the Context class
		static Source()
		{
			var connectionString = ConfigurationManager.ConnectionStrings["StorageConnectionString"].ConnectionString;
			StorageAccount = CloudStorageAccount.Parse(connectionString);
			CloudTableClient.CreateTablesFromModel(
				typeof(Context),
				StorageAccount.TableEndpoint.AbsoluteUri,
				StorageAccount.Credentials);
		}

		// The public construstor initializes the data context used to access table
		// storage.
		public Source()
		{
			_context = new Context(StorageAccount.TableEndpoint.AbsoluteUri, StorageAccount.Credentials)
				{
					RetryPolicy = RetryPolicies.Retry(3, TimeSpan.FromSeconds(1))
				};
		}

		// This method returns the contents of Entry tables.
		public IEnumerable<Block> Entries()
		{
			// We retrieve today entries which is used by WebRole to bind a data grid and
			// display the blocks.
			var results =
				from g in _context.Block
				where g.PartitionKey == DateTime.UtcNow.ToString("YYYYMMDD")
				select g;
			return results;
		}

		// This method insert a new entry into the Entry table.
		public void AddEntry(Block entry)
		{
			_context.AddObject("Entry", entry);
			_context.SaveChanges();
		}
	}
}
