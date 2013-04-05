using System.Diagnostics;
using System.Web.Mvc;

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
	}
}
