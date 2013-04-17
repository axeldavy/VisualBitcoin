using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.WindowsAzure;
using Storage;
using Storage.Models;

namespace BitcoinWorkerRole
{
	public class BitcoinClient
	{
		public static Block ListSinceBlock { get; private set; }
        public static Block FirstBlock { get; private set; }
		public static Block LastBlock { get; private set; }
		public static int MaximumNumberOfBlocksInTheStorage;
		public static int NumberOfBlocksInTheStorage { get; private set; }
        public static Uri Uri { get; private set; }
        public static ICredentials Credentials { get; private set; }

		public static void Init()
		{
			Trace.WriteLine("Init", "VisualBitcoin.BitcoinWorkerRole.BitcoinClient Information");

			var user = CloudConfigurationManager.GetSetting("BitcoinUser");
			var password = CloudConfigurationManager.GetSetting("BitcoinPassword");
			var uri = CloudConfigurationManager.GetSetting("BitcoinVirtualMachineUri");
			
			Credentials = new NetworkCredential(user, password);
			Uri = new Uri(uri);
			
			SetListSinceBlock();
            SetFirstBlock();
        }

		public static void Initialisation()
		{
			Trace.WriteLine("Initialisation without backup", "VisualBitcoin.BitcoinWorkerRole.BitcoinClient Information");

			var user = CloudConfigurationManager.GetSetting("BitcoinUser");
			var password = CloudConfigurationManager.GetSetting("BitcoinPassword");
			var virtualMachineUri = CloudConfigurationManager.GetSetting("BitcoinVirtualMachineUri");

			Credentials = new NetworkCredential(user, password);
			Uri = new Uri(virtualMachineUri);

			var listSinceBlock = Invoke("listsinceblock") as JObject;
			Debug.Assert(listSinceBlock != null, "lastBlock != null");
			var lastBlockHash = listSinceBlock["lastblock"];
			var lastBlock = GetBlockByHash(lastBlockHash);

			MaximumNumberOfBlocksInTheStorage = 30;
			NumberOfBlocksInTheStorage = 0;
			FirstBlock = lastBlock;
			LastBlock = lastBlock;

			UploadBlock(lastBlock, 0);
		}

		public static void Initialisation(int maximumNumberOfBlocksInTheStorage, int numberOfBlocksInTheStorage,
			string firstBlockHash, string lastBlockHash)
		{
			Trace.WriteLine("Initialisation without backup", "VisualBitcoin.BitcoinWorkerRole.BitcoinClient Information");

			var user = CloudConfigurationManager.GetSetting("BitcoinUser");
			var password = CloudConfigurationManager.GetSetting("BitcoinPassword");
			var virtualMachineUri = CloudConfigurationManager.GetSetting("BitcoinVirtualMachineUri");
			var firstBlockBlobName = GetBlockBlobName(firstBlockHash);
			var lastBlockBlobName = GetBlockBlobName(lastBlockHash);

			Credentials = new NetworkCredential(user, password);
			Uri = new Uri(virtualMachineUri);
			MaximumNumberOfBlocksInTheStorage = maximumNumberOfBlocksInTheStorage;
			NumberOfBlocksInTheStorage = numberOfBlocksInTheStorage;
			FirstBlock = Blob.DownloadBlockBlob<Block>(firstBlockBlobName);
			LastBlock = Blob.DownloadBlockBlob<Block>(lastBlockBlobName);
		}

        // The following method was adapted from bitnet on 04/2013
        // bitnet: COPYRIGHT 2011 Konstantin Ineshin, Irkutsk, Russia.
		private static JObject InvokeMethod(string aSMethod, params object[] aParams) 
		{
			var webRequest = (HttpWebRequest)WebRequest.Create(Uri);
			webRequest.Credentials = Credentials;

			webRequest.ContentType = "application/json-rpc";
			webRequest.Method = "POST";

			var joe = new JObject();
			joe["jsonrpc"] = "1.0";
			joe["id"] = "1";
			joe["method"] = aSMethod;

			if (aParams != null)
			{
				if (aParams.Length > 0)
				{
					var props = new JArray();
					foreach (var p in aParams)
					{
						props.Add(p);
					}
					joe.Add(new JProperty("params", props));
				}
			}

			string s = JsonConvert.SerializeObject(joe);
			// serialize json for the request
			byte[] byteArray = Encoding.UTF8.GetBytes(s);
			webRequest.ContentLength = byteArray.Length;

			using (Stream dataStream = webRequest.GetRequestStream())
			{
				dataStream.Write(byteArray, 0, byteArray.Length);
			}
			try
			{
				using (WebResponse webResponse = webRequest.GetResponse())
				{
					using (Stream str = webResponse.GetResponseStream())
					{
						Debug.Assert(str != null, "str != null");
						using (var sr = new StreamReader(str))
						{                       
							return JsonConvert.DeserializeObject<JObject>(sr.ReadToEnd());
						}
					}
				}
			}
			catch (WebException e)
			{
				if (e.Status == WebExceptionStatus.ProtocolError)
				{
					using (Stream str = e.Response.GetResponseStream())
					{
						Debug.Assert(str != null, "str != null");
						using (var sr = new StreamReader(str))
						{
                            Console.WriteLine(sr.ReadToEnd());
							return JsonConvert.DeserializeObject<JObject>(sr.ReadToEnd());
						}
					}
				}
				throw;
			}
		}

		private static JToken Invoke(string asMethod, params object[] aParams)
		{
			JObject received = InvokeMethod(asMethod, aParams);
			JToken result = received["result"];
			JToken error = received["error"]; // bitcoind always sends an error field
			bool hasError = error.ToString() != ""; // we have to test if the error field contain an error message.
			if (!hasError) return result; // may be null
			throw new Exception("Invoke:" + error);
		}

		public static void UploadNewBlocks()
		{
			if (MaximumNumberOfBlocksInTheStorage <= NumberOfBlocksInTheStorage)
				return;

			Trace.WriteLine("Upload new blocks", "VisualBitcoin.BitcoinWorkerRole.BitcoinClient Information");

			var block = UpdateNextBlockHash(LastBlock);
			UpdateBlock(block);
			
			while (!string.IsNullOrEmpty(LastBlock.NextBlock))
			{
				Trace.WriteLine("\"\" != \"" + block.NextBlock + "\"", "VisualBitcoin.BitcoinWorkerRole.BitcoinClient Information");

				block = GetNextBlock(block);
				UploadBlock(block, 0);
			}
		}

		public static void UploadNewBlocks(int max = 1000)
		{
			Trace.WriteLine("Upload new blocks", "VisualBitcoin.BitcoinWorkerRole.BitcoinClient Information");

			// Retrieve information from the BitnetWorkerRole backup.
			var backup = Blob.DownloadBlockBlob<BitnetBackup>("bitnetbackup");
            Block block;
            int count;
            if (backup == null)
            {
                block = ListSinceBlock;
                count = 1;
            }
            else
            {
                String backupBlobName = GetBlockBlobName(backup.Hash);
                block = UpdateNextBlockHash(Blob.DownloadBlockBlob<Block>(backupBlobName));
                count = backup.Count;
            }

			while (block.Hash != ListSinceBlock.Hash)
			{
                // Keep a max number of blocks in WindowsAzureStorage by deleting older blocks first
                if (count == max)
                {
                    Blob.DeleteBlockBlob<Block>(GetBlockBlobName(FirstBlock.Hash));
	                Debug.Assert(FirstBlock != null, "FirstBlock != null");
	                String nextBlockBlobName = GetBlockBlobName(FirstBlock.NextBlock);
                    FirstBlock = Blob.DownloadBlockBlob<Block>(nextBlockBlobName);
                    Blob.UploadBlockBlob("headblock", FirstBlock);
                    count -= 1;
                }
                UploadBlock(block, count);
                count += 1;
                block = GetNextBlock(block);
			}
            UploadBlock(block, count);
		}

        private static String GetBlockBlobName(String hash)
        {
            return hash;
        }

        private static Block UpdateNextBlockHash(Block block)
        {
            var blockJObject = Invoke("getblock", new object[] { block.Hash }) as JObject;
	        Debug.Assert(blockJObject != null, "blockJObject != null");
	        block.NextBlock = (string) blockJObject["nextblockhash"];
            return block;
        }

        private static void UploadBlock(Block block, int count)
        {
	        var blockBlobName = GetBlockBlobName(block.Hash);
            var blockReference = new BlockReference(block.Hash);

            Blob.UploadBlockBlob(blockBlobName, block);
            Queue.PushMessage(blockReference);

            var bitnetBackup = new BitnetBackup(block.Hash, count);
            Blob.UploadBlockBlob("bitnetbackup", bitnetBackup);

	        NumberOfBlocksInTheStorage = NumberOfBlocksInTheStorage + 1;
	        LastBlock = block;

	        var bitcoinWorkerRoleBackup = new BitcoinWorkerRoleBackup(MaximumNumberOfBlocksInTheStorage,
				NumberOfBlocksInTheStorage, FirstBlock.Hash, LastBlock.Hash);
			Blob.UploadBlockBlob("bitcoinworkerrolebackup", bitcoinWorkerRoleBackup);
        }

		private static void UpdateBlock(Block block)
		{
			var blockBlobName = GetBlockBlobName(block.Hash);

			Blob.UploadBlockBlob(blockBlobName, block);

			LastBlock = block;
		}

		private static Transactions[] GetTransactionsFromJObject(JObject block)
		{
			JToken txidList = block["tx"];
            string blockHash = (string) block["hash"];
			var transactionsFromBlock = new Transactions[txidList.Count()];

			int count = 0;
			foreach (JValue txid in txidList)
			{
				JObject transaction = DecodeTransaction(txid);
				double amount = 0;
				foreach (JObject v in transaction["vout"])
				{
					if (v["value"].Type == JTokenType.Float)
						amount = amount + (double)v["value"]; // assert > 0
					else throw new Exception("type error in BlockandTransactionTransfer");
				}

                //TODO: add vin and vout
				transactionsFromBlock[count] = new Transactions
				{
				
                    Hash = (string) transaction["hash"],                    
					Version = (int)transaction["ver"],
					Locktime = (ulong)transaction["locktime"],
                    BlockHash = blockHash,
                    Vin_size = (int) transaction["vin_sz"],
                    Vout_size = (int) transaction["vout_sz"],
                    Size = (int) transaction["size"],
                    Relayed_by = (string)transaction ["relayed_by"],
                    
                    Txid = (string)txid,
				};
				count += 1;
			}

			return transactionsFromBlock;

		}

        private static string[] GetTransactionIds(JObject block)
        {
            JToken txidList = block["tx"];
            string[] transactionIds = new string[txidList.Count()];
            int count = 0;
            foreach (var txid in txidList) 
            {
                transactionIds[count] = (string)txid;
                count += 1;
            }
            return transactionIds;
        }

		private static JObject DecodeTransaction(JValue txid)
		{
			JToken txHash = Invoke("getrawtransaction", new object[] { txid });
			if (txHash == null) throw new Exception("null transaction hash value");
			return Invoke("decoderawtransaction", new object[] { txHash }) as JObject;
		}

		public static void SetListSinceBlock()
		{
			Trace.WriteLine("Set ListSinceBlock", "VisualBitcoin.BitcoinWorkerRole.BitcoinClient Information");

			var lastBlock = Invoke("listsinceblock") as JObject;
			Debug.Assert(lastBlock != null, "lastBlock != null");
			JToken lastBlockHash = lastBlock["lastblock"];

			ListSinceBlock = GetBlockByHash(lastBlockHash);
		}

        public static void SetFirstBlock()
        {
			Trace.WriteLine("Set FirstBlock", "VisualBitcoin.BitcoinWorkerRole.BitcoinClient Information");

            var block = Blob.DownloadBlockBlob<Block>("headblock");
            if (block == null)
            {
                FirstBlock = ListSinceBlock;
                Blob.UploadBlockBlob("headblock", FirstBlock);
            }
            else
            {
                FirstBlock = block;
            }
        }

		private static Block GetPrevBlock(Block block)
		{
			return GetBlockByHash(block.PreviousBlock);
		}

		private static Block GetNextBlock(Block block)
		{
			return GetBlockByHash(block.NextBlock);
		}

		private static Block GetBlockByHash(JToken hashToken)
		{
			var block = Invoke("getblock", new object[] { hashToken }) as JObject;
			return GetBlockFromJObject(block);
		}

		private static Block GetBlockFromJObject(JObject block)
		{
			var hash = (string)block["hash"];
			var version = (string)block["version"];
			var previousBlock = (string)block["previousblockhash"]; // TODO, throw error if not exists
			var nextBlock = (string)block["nextblockhash"]; // TODO, throw error if not exists
			var merkleRoot = (string)block["merkleroot"];
			var time = (int)block["time"];
			var bits = 0; // default
			const int numberOnce = 0; // default 
            string[] transactionIds = GetTransactionIds(block);
			var numberOfTransactions = transactionIds.Count();
			var size = (int)block["size"];
			const int index = 0; // default
			const bool isInMainChain = false; // default
			var height = (int)block["height"];
			var receivedTime = 0; // Parse DateTime.Now
			const string relayedBy = ""; // default
			return new Block(hash, version, previousBlock, nextBlock, merkleRoot, time, bits, numberOnce,
				numberOfTransactions, size, index, isInMainChain, height, receivedTime, relayedBy, transactionIds);
		}

	}
}
