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
        private readonly IEnumerable<Block> _blockList = Example.Data;
        //
        // GET: /Explore/Details/5

        public ActionResult Details(int id)
        {
            Block block = _blockList.FirstOrDefault(b => b.Id == id);

            if (block == null)
            {
<<<<<<< HEAD
                throw new HttpException(404, "Not found");
=======
                return HttpNotFound();
>>>>>>> 2e6ad6e99bd40c78ebd232a830a25600767ee897
            }

            return View(block);
        }

        public ActionResult Search(string hashSub = "", string relay = "")
        {
            IEnumerable<Block> blocks = from b in _blockList
                                        select b;

            if (!String.IsNullOrEmpty(hashSub))
            {
                blocks = blocks.Where(b => b.Hash.Contains(hashSub.ToLower()));
            }

            if (!String.IsNullOrEmpty(relay))
            {
                blocks = blocks.Where(b => b.RelayedBy.ToLower().Contains(relay.ToLower()));
            }

<<<<<<< HEAD
=======
            ViewBag.NoResult = (!blocks.Any());

>>>>>>> 2e6ad6e99bd40c78ebd232a830a25600767ee897
            return View(blocks);
        }
    }
}