using System.Collections.Generic;
using System.Web.Mvc;
using WebRole.Models;

namespace WebRole.Controllers
{
	public class HomeController : Controller
    {
        private readonly IEnumerable<Block> _blockList = Example.Data;

        public ActionResult Index()
        {
            return View(_blockList);
        }

		public ActionResult About()
		{
			return View();
		}

        private readonly Data.Data _dataService = new Data.Data();

		public ActionResult DataDisplayTest()
		{
			// WARNING :
			// 
			// You must set up your storage emulator before.
			// 1. Launch it outside Visual Studio, click start, search for Storage Emulator and launch it.
			// 2. Set it up outside Visual Studio, click start, search for Azure Storage Explorer and launch it.
			// 3. In Azure Storage Explorer add a container and some blobs.
			// 4. Change the containerName below.

			const string containerName = "container";
			List<string> blobsList = _dataService.RetrieveBlobsList(containerName);
			return View(blobsList);
		}
	}
}