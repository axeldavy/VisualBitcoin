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
		public ExplorerController()
		{
			Trace.WriteLine("Reach Explorer Controller");
		}

		private Block FindRow(string partitionKey, string rowKey)
		{
			var blockTable = WindowsAzureStorage.GetTableReference();
			var retrieveOperation = TableOperation.Retrieve<Block>(partitionKey, rowKey);
			var retrievedResult = blockTable.Execute(retrieveOperation);
			var blockList = retrievedResult.Result as Block;
			if (blockList == null)
			{
				throw new Exception("Explorer controller: No block found for: " + partitionKey + ", " + rowKey);
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
				var blockTable = WindowsAzureStorage.GetTableReference();
				var query = new TableQuery<Block>().Where(TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, "block0"));
				lists = blockTable.ExecuteQuery(query, reqOptions).ToList();
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
