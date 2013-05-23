using System.Collections.Generic;
using System.Diagnostics;
using System.Web.Mvc;
using Storage;
using Storage.Models;
using WebRole.Models;


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
            /* TODO: Show blocks on Explorer page*/
			return View();
		}
	}
}
