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
        public SearchModel BlockName = new SearchModel();
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
                                            block.Size, block.Height);
            return blockModel;
        }

        public ActionResult SearchResult()
        {
            var connectionString = CloudConfigurationManager.GetSetting("StorageConnectionString");
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
            Blob blob = new Blob(storageAccount);

            var blockModelList = new List<BlockModel>();
            var block = blob.GetBlock(BlockName.Name);
            blockModelList.Add(BlockModelOfBlock(block));
            return View(blockModelList);
        }

        [HttpPost]
        public ActionResult Index(SearchModel name)
        {
            BlockName = name;
            return RedirectToAction("SearchResult");
        }

    }
}
