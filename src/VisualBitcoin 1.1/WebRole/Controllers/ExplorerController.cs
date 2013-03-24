using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.RetryPolicies;
using Microsoft.WindowsAzure.Storage.Table;
using Storage;
using WebRole.Models;

namespace WebRole.Controllers
{
	public class ExplorerController : Controller
	{
		// Reference to our table in WindowsAzureStorage.
		private readonly CloudTable _blockTable;

		public ExplorerController()
		{
			var storage = new WindowsAzureStorage();
			var tableClient = storage.RetrieveTableClient();
			_blockTable = tableClient.GetTableReference(WindowsAzureStorage.Table);
		}

		private Block FindRow(string partitionKey, string rowKey)
		{
			var retrieveOperation = TableOperation.Retrieve<Block>(partitionKey, rowKey);
			var retrievedResult = _blockTable.Execute(retrieveOperation);
			var blockList = retrievedResult.Result as Block;
			if (blockList == null)
			{
				throw new Exception("WebRole/Explorer controller: No block found for: " + partitionKey + ", " + rowKey);
			}
			return blockList;
		}

		public ActionResult Index()
		{
			var reqOptions = new TableRequestOptions
			{
				MaximumExecutionTime = TimeSpan.FromSeconds(1.5),
				RetryPolicy = new LinearRetry(TimeSpan.FromSeconds(3), 3)
			};
			List<Block> lists;
			try
			{
				var query = new TableQuery<Block>().Where(TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, "mailinglist"));
				lists = _blockTable.ExecuteQuery(query, reqOptions).ToList();
			}
			catch (StorageException se)
			{
				ViewBag.errorMessage = "Timeout error, try again. ";
				Trace.TraceError(se.Message);
				return View("Error");
			}


			return View(lists);


		}
	}
}
