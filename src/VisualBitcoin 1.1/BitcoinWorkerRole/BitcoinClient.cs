using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Net;
using System.Configuration;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Storage;
using Storage.Models;

namespace BitcoinWorkerRole
{
	public class BitcoinClient
	{
		Block _listSinceBlock;

		private readonly Uri _url;
		private readonly ICredentials _credentials;

		public BitcoinClient()
		{
			var user = ConfigurationManager.AppSettings["bitcoinuser"];
			var password = ConfigurationManager.AppSettings["bitcoinpassword"];
			_credentials = new NetworkCredential(user, password);
			_url = new Uri("http://127.0.0.1:8332");

			_listSinceBlock = GetLastBlock();
		}

		public JObject InvokeMethod(string aSMethod, params object[] aParams) //adapted from bitnet, 04/2013, bitnet: COPYRIGHT 2011 Konstantin Ineshin, Irkutsk, Russia.
		{
			var webRequest = (HttpWebRequest)WebRequest.Create(_url);
			webRequest.Credentials = _credentials;

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
							return JsonConvert.DeserializeObject<JObject>(sr.ReadToEnd());
						}
					}
				}
				throw;
			}
		}

		public JToken Invoke(string asMethod, params object[] aParams)
		{
			JObject received = InvokeMethod(asMethod, aParams);
			JToken result = received["result"];
			JToken error = received["error"]; // bitcoind always sends an error field
			bool hasError = error.ToString() != ""; // we have to test if the error field contain an error message.
			if (!hasError) return result; // may be null
			throw new Exception("Invoke:" + error);
		}

		public void UploadNewBlocks(int max = 1000)
		{
			// Retrieve information from the BitnetWorkerRole backup.
			var backup = Blob.DownloadBlockBlob<BitnetBackup>("bitnetbackup");
			var block = Blob.DownloadBlockBlob<Block>(backup.Hash);
			var count = backup.Count;

			while (count < max && block.Hash != _listSinceBlock.Hash)
			{
				block = GetNextBlock(block);
				var blockReference = new BlockReference(block.Hash);

				Blob.UploadBlockBlob(block.Hash, block);
				Queue.PushMessage(blockReference);
				count += 1;

				backup = new BitnetBackup(block.Hash, count);
				Blob.UploadBlockBlob("bitnetbackup", backup);
			}
		}

		public Transactions[] GetTransactionsFromBlock(JObject block)
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

		public JObject DecodeTransaction(JValue txid)
		{
			JToken txHash = Invoke("getrawtransaction", new object[] { txid });
			if (txHash == null) throw new Exception("null transaction hash value");
			return Invoke("decoderawtransaction", new object[] { txHash }) as JObject;
		}

		public Block GetLastBlock()
		{
			var lastBlock = Invoke("listsinceblock") as JObject;
			Debug.Assert(lastBlock != null, "lastBlock != null");
			JToken lastBlockHash = lastBlock["lastblock"];

			_listSinceBlock = GetBlockByHash(lastBlockHash);
			return _listSinceBlock;
		}

		public Block GetPrevBlock(Block block)
		{
			return GetBlockByHash(block.PreviousBlock);
		}

		public Block GetNextBlock(Block block)
		{
			return GetBlockByHash(block.NextBlock);
		}

		private Block GetBlockByHash(JToken hashToken)
		{
			var block = Invoke("getblock", new object[] { hashToken }) as JObject;
			return GetBlockModel(block);
		}

		private Block GetBlockModel(JObject block)
		{
			var hash = (string)block["hash"];
			var version = (string)block["version"];
			var previousBlock = (string)block["previousblockhash"]; // TODO, throw error if not exists
			var nextBlock = (string)block["nextblockhash"]; // TODO, throw error if not exists
			var merkleRoot = (string)block["merkleroot"];
			var time = (int)block["time"];
			var bits = (int)block["bits"];
			const int numberOnce = 0; // default
			var transactions = GetTransactionsFromBlock(block);
			var numberOfTransactions = transactions.Count();
			var size = (int)block["size"];
			const int index = 0; // default
			const bool isInMainChain = false; // default
			var height = (int)block["height"];
			var receivedTime = 0;
			try
			{
				receivedTime = int.Parse(DateTime.Now.ToString("HH:mm:ss tt"));
			}
			catch (Exception)
			{
				Trace.WriteLine("Exception occured in creating Block Model");
			}
			const string relayedBy = ""; // default
			return new Block(hash, version, previousBlock, nextBlock, merkleRoot, time, bits, numberOnce,
				numberOfTransactions, size, index, isInMainChain, height, receivedTime, relayedBy, transactions);
		}

	}
}
