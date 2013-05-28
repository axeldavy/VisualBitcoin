using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Storage;
using Storage.Models;
using WebRole.Models;

namespace WebRole.Controllers
{
    public class SearchController : Controller
    {
        //
        // GET: /Search/

        public ActionResult Index()
        {
            return View();
        }

        public static BlockModel BlockModelOfBlock(Block block)
        {
            var blockModel = new BlockModel(block.Hash, block.Version, block.PreviousBlock, block.MerkleRoot,
                                            block.Time, block.NumberOnce, block.NumberOfTransactions,
                                            block.Size, block.Height, block.Amount);
            return blockModel;
        }

        public ActionResult SearchResult()
        {
            var connectionString = CloudConfigurationManager.GetSetting("StorageConnectionString");
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
            Blob blob = new Blob(storageAccount);
            string search = (string)TempData["search"];
            if (search == null)
                return View(new Block());
            var block = blob.GetBlock(search);
            if (block == null)
                block = new Block();
            return View(block);
        }

        [HttpPost]
        public ActionResult Index(SearchModel search)
        {
            TempData["search"] = search.Name;
            return RedirectToAction("SearchResult");
        }

    }
}
