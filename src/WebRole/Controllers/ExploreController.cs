using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebRole.Models;

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
            Block block = Example.Find(id);

            if (block == null)
            {
                throw new HttpException(404, "Not found");
            }

            return View(block);
        }
    }
}
