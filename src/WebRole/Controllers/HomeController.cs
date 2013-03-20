using System.Collections.Generic;
using System.Web.Mvc;
using WebRole.Models;

namespace WebRole.Controllers
{
    public class HomeController : Controller
    {
        private readonly IEnumerable<Block> _blockList = Example.Data;

        //
        // GET: /Home/

        public ActionResult Index()
        {
            return View(_blockList);
        }

        public ActionResult About()
        {
            return View();
        }
    }
}