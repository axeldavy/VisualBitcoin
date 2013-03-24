using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.RetryPolicies;
using Microsoft.WindowsAzure.Storage.Table;
using WebRole.Models;

namespace WebRole.Controllers
{
	public class ExplorerController : Controller
	{
		// Reference to our table in WindowsAzureStorage.
		private CloudTable blockTable;

		public ExplorerController()
		{
			var storageAccount = CloudStorageAccount.DevelopmentStorageAccount;
			var tableClient = storageAccount.CreateCloudTableClient();
			blockTable = tableClient.GetTableReference();
		}

		public ActionResult Index()
		{
			var requestOptions = new TableRequestOptions()
			{
				MaximumExecutionTime = TimeSpan.FromSeconds(1.5),
				RetryPolicy = new LinearRetry(TimeSpan.FromSeconds(3), 3)
			};
			List<Block> lists;
			try
			{
				var query = new TableQuery<Block>();
				lists = blockList.ExecuteQuery(query, requestOptions).ToList();
			}
			catch (StorageException se)
			{
				ViewBag.errorMessage = "Explorer controller: Timeout error, try again. ";
				Trace.TraceError(se.Message);
				return View("Error");
			}


			return View(lists);
		}
	}
}
