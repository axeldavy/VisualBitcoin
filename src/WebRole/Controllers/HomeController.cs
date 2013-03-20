using System.Web.Mvc;

namespace WebRole.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        public ActionResult Index()
        {
            return View(Models.Example.Data);
        }

        public ActionResult About()
        {
            return View();
        }
    }
}
