using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebRole.Controllers
{
    public class ExploreController : Controller
    {
        //
        // GET: /Explore/

        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: /Explore/Details/5

        public ActionResult Details(int id)
        {
            return View(Models.Example.Find(id));
        }
    }
}
