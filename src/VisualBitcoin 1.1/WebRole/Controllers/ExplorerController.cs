using System.Collections.Generic;
using System.Diagnostics;
using System.Web.Mvc;
using Storage;
using Storage.Models;
using WebRole.Models;


namespace WebRole.Controllers
{
	public class ExplorerController : Controller
	{
		public ExplorerController()
		{
			Trace.WriteLine("Entry point called",
				"VisualBitcoin.WebRole.Controllers.ExplorerController Information");
		}


		public static BlockModel BlockModelOfBlock(Block block)
		{
			var blockModel = new BlockModel(block.Hash, block.Version, block.PreviousBlock, block.MerkleRoot,
			                                block.Time, block.Bits, block.NumberOnce, block.NumberOfTransactions,
											block.Size, block.Index, block.IsInMainChain, block.Height,
											block.ReceivedTime, block.RelayedBy);
			return blockModel;
		}

		public ActionResult Index()
		{
		    var blockList = Blob.GetBlockList();
		    var blockModelList = new List<BlockModel>();
            foreach (string name in blockList)
            {
                var block = Blob.GetBlock(name);
                blockModelList.Add(BlockModelOfBlock(block));
            }
			return View(blockModelList);
		}
	}
}
