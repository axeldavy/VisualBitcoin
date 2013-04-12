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
        public static Uri Uri { get; private set; }
        public static ICredentials Credentials { get; private set; }

		public static void Init()
		{
			var user = CloudConfigurationManager.GetSetting("BitcoinUser");
			var password = CloudConfigurationManager.GetSetting("BitcoinPassword");
			var uri = CloudConfigurationManager.GetSetting("BitcoinVirtualMachineUri");
			
			Credentials = new NetworkCredential(user, password);
			Uri = new Uri(uri);
			
			ListSinceBlock = GetListSinceBlock();
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

		public static void UploadNewBlocks(int max = 1000)
		{
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
                block = UpdateNextBlockHash(Blob.DownloadBlockBlob<Block>(backup.Hash));
                count = backup.Count;
            }

			while (count < max && block.Hash != ListSinceBlock.Hash)
			{
                if (count == max)
                {
                    // TODO: Get oldest block, delete it.
                    count -= 1;
                }
                UploadBlock(block, count);
                count += 1;
                block = GetNextBlock(block);
			}
            UploadBlock(block, count);
		}

        private static Block UpdateNextBlockHash(Block block)
        {
            var blockJObject = Invoke("getblock", new object[] { block.Hash }) as JObject;
            block.NextBlock = (string) blockJObject["nextblockhash"];
            return block;
        }

        private static void UploadBlock(Block block, int count)
        {
            var blockReference = new BlockReference(block.Hash);

            Blob.UploadBlockBlob("block" + block.Hash, block);
            Queue.PushMessage(blockReference);

            BitnetBackup backup = new BitnetBackup(block.Hash, count);
            Blob.UploadBlockBlob("bitnetbackup", backup);
        }

		private static Transactions[] GetTransactionsFromBlock(JObject block)
		{
			JToken txidList = block["tx"];
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
				transactionsFromBlock[count] = new Transactions
				{
					Amount = amount,
					Txid = (string)txid,
					Version = (int)transaction["version"],
					Locktime = (int)transaction["locktime"]
				};
				count += 1;
			}

			return transactionsFromBlock;

		}

		private static JObject DecodeTransaction(JValue txid)
		{
			JToken txHash = Invoke("getrawtransaction", new object[] { txid });
			if (txHash == null) throw new Exception("null transaction hash value");
			return Invoke("decoderawtransaction", new object[] { txHash }) as JObject;
		}

		public static Block GetListSinceBlock()
		{
			var lastBlock = Invoke("listsinceblock") as JObject;
			Debug.Assert(lastBlock != null, "lastBlock != null");
			JToken lastBlockHash = lastBlock["lastblock"];

			ListSinceBlock = GetBlockByHash(lastBlockHash);
			return ListSinceBlock;
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
			var transactions = new Transactions[0];/* TODO, fix binding error GetTransactionsFromBlock(block);*/
			var numberOfTransactions = transactions.Count();
			var size = (int)block["size"];
			const int index = 0; // default
			const bool isInMainChain = false; // default
			var height = (int)block["height"];
			var receivedTime = 0; // Parse DateTime.Now
			const string relayedBy = ""; // default
			return new Block(hash, version, previousBlock, nextBlock, merkleRoot, time, bits, numberOnce,
				numberOfTransactions, size, index, isInMainChain, height, receivedTime, relayedBy, transactions);
		}

	}
}
