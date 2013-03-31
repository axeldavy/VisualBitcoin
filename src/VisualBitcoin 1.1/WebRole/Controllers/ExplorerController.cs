using System.Diagnostics;
using System.Web.Mvc;
using Storage;
using WebRole.Models;

namespace WebRole.Controllers
{
	public class ExplorerController : Controller
	{
		public ExplorerController()
		{
			Trace.WriteLine("Reach Explorer Controller");
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
			var block = Storage.Storage.GetExampleBlock();
			var blockModel = BlockModelOfBlock(block);
			return View(blockModel);
		}
	}
}
