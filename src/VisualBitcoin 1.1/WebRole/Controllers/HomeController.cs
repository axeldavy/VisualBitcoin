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
            var transactionList = Blob.GetTransactionList();
            var tranModelList = new List<Transactions>();
            foreach (string trans in transactionList)
            {
                var t = Blob.GetTransaction(trans);
                tranModelList.Add(t);
            }
			return View(tranModelList);
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
