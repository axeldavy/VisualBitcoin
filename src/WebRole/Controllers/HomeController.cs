using System.Collections.Generic;
using System.Web.Mvc;
using Data;

namespace WebRole.Controllers
{
	public class HomeController : Controller
	{
		//
		// GET: /Home/

		public Data.Data DataService = new Data.Data() ;

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
			// WARNING :
			// 
			// You must set up your storage emulator before.
			// 1. Launch it outside Visual Studio, click start, search for Storage Emulator and launch it.
			// 2. Set it up outside Visual Studio, click start, search for Azure Storage Explorer and launch it.
			// 3. In Azure Storage Explorer add a container and some blobs.
			// 4. Change the containerName below.

			string containerName = "container";
			List<string> blobsList = DataService.RetrieveBlobsList(containerName);
			return View(blobsList);
		}
	}
}
