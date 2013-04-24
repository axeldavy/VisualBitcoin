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

			UploadBlock(lastBlock);
		}

		public static void Initialisation(int maximumNumberOfBlocksInTheStorage, int numberOfBlocksInTheStorage,
			string firstBlockHash, string lastBlockHash)
		{
			Trace.WriteLine("Initialisation without backup", "VisualBitcoin.BitcoinWorkerRole.BitcoinClient Information");

			var user = CloudConfigurationManager.GetSetting("BitcoinUser");
			var password = CloudConfigurationManager.GetSetting("BitcoinPassword");
			var virtualMachineUri = CloudConfigurationManager.GetSetting("BitcoinVirtualMachineUri");
			var firstBlockBlobName = firstBlockHash;
			var lastBlockBlobName = lastBlockHash;

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

        // Use this method for testing changes made to Blocks class
        private static void UpdateBlocks()
        {
            foreach (string blockHash in Blob.GetBlockList())
            {
                var blockObject = Invoke("getblock", new object[] { blockHash }) as JObject;
                var block = GetBlockFromJObject(blockObject);
                Blob.UploadBlockBlob(block.Hash, block);
                UploadTransactionsFromBlock(block);
            }
        }

		public static void UploadNewBlocks()
		{
            // For testing purposes use UpdateBlocks() HERE --
            //UpdateBlocks();
			if (MaximumNumberOfBlocksInTheStorage <= NumberOfBlocksInTheStorage)
				return;

			Trace.WriteLine("Upload new blocks", "VisualBitcoin.BitcoinWorkerRole.BitcoinClient Information");

			var block = UpdateNextBlockHash(LastBlock);
			UpdateBlock(block);
			
			while (!string.IsNullOrEmpty(LastBlock.NextBlock))
			{
				Trace.WriteLine("\"\" != \"" + block.NextBlock + "\"", "VisualBitcoin.BitcoinWorkerRole.BitcoinClient Information");

				block = GetBlockByHash(block.NextBlock);
				UploadBlock(block);
                UploadTransactionsFromBlock(block);
			}
		}

        private static Block UpdateNextBlockHash(Block block)
        {
            var blockJObject = Invoke("getblock", new object[] { block.Hash }) as JObject;
	        Debug.Assert(blockJObject != null, "blockJObject != null");
	        block.NextBlock = (string) blockJObject["nextblockhash"];
            return block;
        }

        private static void UploadBlock(Block block)
        {
	        var blockBlobName = block.Hash;
            var blockReference = new BlockReference(block.Hash);

            Blob.UploadBlockBlob(blockBlobName, block);
            Queue.PushMessage(blockReference);

	        NumberOfBlocksInTheStorage = NumberOfBlocksInTheStorage + 1;
	        LastBlock = block;

	        var bitcoinWorkerRoleBackup = new BitcoinWorkerRoleBackup(MaximumNumberOfBlocksInTheStorage,
				NumberOfBlocksInTheStorage, FirstBlock.Hash, LastBlock.Hash);
			Blob.UploadBlockBlob("bitcoinworkerrolebackup", bitcoinWorkerRoleBackup);
        }

        private static void UploadTransactionsFromBlock(Block block)
        {
            Transactions[] trans = GetTransactionsFromBlock(block);
            foreach (Transactions t in trans)
            {
                Blob.UploadBlockBlob(t.Txid, t);
                Queue.PushMessage(t);
            }
        }

		private static void UpdateBlock(Block block)
		{
			var blockBlobName = block.Hash;
			Blob.UploadBlockBlob(blockBlobName, block);
			LastBlock = block;
		}

		private static Transactions[] GetTransactionsFromBlock(Block block)
		{
            string[] txidList = block.TransactionIds;
            string blockHash = block.Hash;
            Transactions[] transactionsFromBlock = new Transactions[txidList.Count()];

            int count = 0;
            foreach (string txid in txidList)
            {
                JObject transaction = DecodeTransaction(txid);
                transactionsFromBlock[count] = new Transactions(
                    (int)transaction["version"],
                    (int)transaction["vin"].Count(),
                    (int)transaction["vout"].Count(),
                    (ulong)transaction["locktime"],
                    (string)txid,
                    blockHash);

                int vout_current = 0;
                foreach (JObject v in transaction["vout"])
                {
                    transactionsFromBlock[count].Outputs[vout_current] = new Vout(
                        (ulong)v["value"],
                        (int)v["n"]
                    );
                    vout_current++;
                }

                int vin_current = 0;
                foreach (JObject v in transaction["vin"])
                {
                    JToken prev_out_new = v["prev_out"];
                    transactionsFromBlock[count].Inputs[vin_current] = new Vin(
                        (string)v["coinbase"],
                        (ulong)v["sequence"]
                    );
                    vin_current++;
                }
                count += 1;
			}

			return transactionsFromBlock;
		}

        private static string[] GetTransactionIds(JObject obj)
        {
            JToken txidList = obj["tx"];
            string[] transactionIds = new string[txidList.Count()];
            int count = 0;
            foreach (var txid in txidList) 
            {
                transactionIds[count] = (string)txid;
                count += 1;
            }
            return transactionIds;
        }

		private static JObject DecodeTransaction(string txid)
		{
			JToken txHash = Invoke("getrawtransaction", new object[] { txid });
			if (txHash == null) throw new Exception("null transaction hash value");
			return Invoke("decoderawtransaction", new object[] { txHash }) as JObject;
		}

		private static Block GetBlockByHash(JToken hashToken)
		{
			var block = Invoke("getblock", new object[] { hashToken }) as JObject;
			return GetBlockFromJObject(block);
		}

		private static Block GetBlockFromJObject(JObject obj)
		{
			string hash = (string)obj["hash"];
			string version = (string)obj["version"];
			string previousBlock = (string)obj["previousblockhash"];
			string nextBlock = (string)obj["nextblockhash"];
			string merkleRoot = (string)obj["merkleroot"];
			int time = (int)obj["time"];
			long numberOnce = (long)obj["nonce"]; 
            string[] transactionIds = GetTransactionIds(obj);
			int numberOfTransactions = transactionIds.Count();
			int size = (int)obj["size"];
			int height = (int)obj["height"];
			return new Block(hash, version, previousBlock, nextBlock, merkleRoot, time, numberOnce,
				numberOfTransactions, size, height, transactionIds);
		}

	}
}
