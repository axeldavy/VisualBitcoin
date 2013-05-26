using System.Collections.Generic;
using System.Diagnostics;
using System.Web.Mvc;
using Storage;
using Storage.Models;
using WebRole.Models;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure;

namespace WebRole.Controllers
{
	public class ExplorerController : Controller
	{
		public ExplorerController()
		{
			Trace.WriteLine("Entry point called",
				"VisualBitcoin.WebRole.Controllers.ExplorerController Information");
		}

		public ActionResult Index()
		{
            var connectionString = CloudConfigurationManager.GetSetting("StorageConnectionString");
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
            Blob blob = new Blob(storageAccount);

            List<Block> blocklist = blob.GetBlockList(10);
            return View(blocklist);
		}
	}
}
