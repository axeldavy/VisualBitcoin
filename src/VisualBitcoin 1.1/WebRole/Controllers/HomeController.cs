using System.Diagnostics;
using System.Collections.Generic;
using Storage.Models;
using System.Web.Mvc;
using Storage;

namespace WebRole.Controllers
{
	public class HomeController : Controller
	{
		public HomeController()
		{
			Trace.WriteLine("Entry point called",
				"VisualBitcoin.WebRole.Controllers.HomeController Information");
		}

		public ActionResult Index()
		{
            /* TODO: Show transactions on home page? */
			return View();
		}

		public ActionResult About()
		{
			return View();
		}

		public ActionResult Contact()
		{
			return View();
		}
        public ActionResult Transactions()
        {
            return View();
        }
	}
}
