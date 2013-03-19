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
            return View();
        }

        //
        // GET: /Explore/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Explore/Create

        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Explore/Edit/5

        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /Explore/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Explore/Delete/5

        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /Explore/Delete/5

        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
