using System.Web.Mvc;

namespace WebRole.Controllers
{
	public class HomeController : Controller
	{
		//
		// GET: /Home/

		public ActionResult Index()
		{
			return View();
		}

		public ActionResult About()
		{
			return View();
		}

		public ActionResult DataDisplayTest()
		{
			return View();
		}
	}
}
