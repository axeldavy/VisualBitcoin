using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Storage;
using Storage.Models;

namespace BitcoinWorkerRole
{
	public class BitcoinClient
	{
        private Block firstBlock;
        private Block lastBlock;
		private int maximumNumberOfBlocks;
        private int numberOfBlocks;

        // Give public access to the following Bitcoin client data
        public Block FirstBlock { get { return firstBlock; } }
        public Block LastBlock { get { return lastBlock; } }
        public int MaximumNumberOfBlocks { get { return maximumNumberOfBlocks; } }
        public int NumberOfBlocks { get { return numberOfBlocks; } }

        private Uri uri;
        private ICredentials credentials;
        private bool blockLimit;
        private int minimalHeight;
        
        private Blob blob;
        private Queue queue;

		public BitcoinClient(Blob blob, Queue queue, string firstBlockHash)
		{
			Trace.WriteLine("Initialisation without backup", "VisualBitcoin.BitcoinWorkerRole.BitcoinClient Information");

            InitializeConfigurations(blob, queue);

			Block block;
			if (String.IsNullOrEmpty(firstBlockHash))
			{
				var listSinceBlock = Invoke("listsinceblock") as JObject;
				Debug.Assert(listSinceBlock != null, "lastBlock != null");
				var lastBlockHash = listSinceBlock["lastblock"];
				block = GetBlockByHash(lastBlockHash);
			}
			else
			{
				block = GetBlockByHash(firstBlockHash);
			}

			maximumNumberOfBlocks = 0;
			numberOfBlocks = 0;
            minimalHeight = block.Height;
			blockLimit = true;
			firstBlock = block;
			lastBlock = block;

			UploadNewBlock(block);
		}

		public BitcoinClient(Blob blob, Queue queue, int maximumNumberOfBlocksInTheStorage, int numberOfBlocksInTheStorage,
			string firstBlockHash, string lastBlockHash, int minimalHeight)
		{
			Trace.WriteLine("Initialisation with backup", "VisualBitcoin.BitcoinWorkerRole.BitcoinClient Information");

            InitializeConfigurations(blob, queue);

			this.maximumNumberOfBlocks = maximumNumberOfBlocksInTheStorage;
			this.numberOfBlocks = numberOfBlocksInTheStorage;
            this.minimalHeight = minimalHeight;
			blockLimit = (maximumNumberOfBlocksInTheStorage != 0);
			firstBlock = blob.GetBlock(firstBlockHash);
			lastBlock = blob.GetBlock(lastBlockHash);
		}

        public void InitializeConfigurations(Blob blob, Queue queue)
        {
            this.blob = blob;
            this.queue = queue; 
            var user = CloudConfigurationManager.GetSetting("BitcoinUser");
            var password = CloudConfigurationManager.GetSetting("BitcoinPassword");
            var virtualMachineUri = CloudConfigurationManager.GetSetting("BitcoinVirtualMachineUri");

            credentials = new NetworkCredential(user, password);
            uri = new Uri(virtualMachineUri);
        }

		// The following method was adapted from bitnet on 04/2013
		// bitnet: COPYRIGHT 2011 Konstantin Ineshin, Irkutsk, Russia.
		private JObject InvokeMethod(string aSMethod, params object[] aParams)
		{
			var webRequest = (HttpWebRequest)WebRequest.Create(uri);
			webRequest.Credentials = credentials;

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
							//Console.WriteLine(sr.ReadToEnd()); Useless since WebException => Invoke will get a non null error field
							return JsonConvert.DeserializeObject<JObject>(sr.ReadToEnd());
						}
					}
				}
				throw;
			}
		}

		private JToken Invoke(string asMethod, params object[] aParams)
		{
			JObject received = InvokeMethod(asMethod, aParams);
			JToken result = received["result"];
			JToken error = received["error"]; // bitcoind always sends an error field
			bool hasError = error.ToString() != ""; // we have to test if the error field contain an error message.
			if (!hasError) return result; // may be null
			throw new Exception("Invoke:" + error);
		}

		public void UploadNewBlocks()
		{   
            if (blockLimit && maximumNumberOfBlocks <= numberOfBlocks)
				return;

			Trace.WriteLine("Upload new blocks", "VisualBitcoin.BitcoinWorkerRole.BitcoinClient Information");

            int lastHeight = (int)Invoke("getblockcount");
            while (lastHeight > lastBlock.Height && !(blockLimit && maximumNumberOfBlocks <= numberOfBlocks))
            {
                JObject nextBlockJObject = Invoke("listsinceblock", new object[] { lastBlock.Hash, lastHeight - lastBlock.Height }) as JObject;
                Block nextBlock = GetBlockByHash((string) nextBlockJObject["lastblock"]);
                lastBlock = UpdateNextBlockHash(lastBlock);               
                
                Trace.WriteLine("\"\" != \"" + nextBlock.Hash + "\"", "VisualBitcoin.BitcoinWorkerRole.BitcoinClient Information");
                
                //UploadTransactionsFromBlock(nextBlock); // Upload Transactions first because the message in the queue must be sent after everything is done.
				UploadNewBlock(nextBlock);
                lastBlock = nextBlock;
                if (lastBlock.NextBlock == null) // Need to retrieve blocks in the main chain if LastBlock is an orphan.
                {
                    UploadOrphanBlocks(nextBlock.Hash);
                }
            }
		}

		private Block UpdateNextBlockHash(Block block)
		{
			var blockJObject = Invoke("getblock", new object[] { block.Hash }) as JObject;
			Debug.Assert(blockJObject != null, "blockJObject != null");
		    block.NextBlock = new List<string> ();
            foreach (var hash in blockJObject["nextblockhash"])
            {
                block.NextBlock.Add((string)hash);
            }
			return block;
		}

		private void UploadNewBlock(Block block)
		{
            block.Amount = UploadTransactionsFromBlock(block); // Upload Transactions first because the message in the queue must be sent after everything is done.
			blob.UploadBlock(block.Hash, block);
			queue.PushMessage(new BlockReference(block.Hash));

			numberOfBlocks += 1;

			var bitcoinWorkerRoleBackup = 
                new BitcoinWorkerRoleBackup(maximumNumberOfBlocks, numberOfBlocks, firstBlock.Hash, lastBlock.Hash, firstBlock.Height);
			blob.UploadBackup(bitcoinWorkerRoleBackup);

		}

        // Returns the amount of bit coins transferred in that block
		private double UploadTransactionsFromBlock(Block block)
		{
			IEnumerable<Transaction> trans = GetTransactionsFromBlock(block);
            double amount = 0;
			foreach (Transaction t in trans)
			{
                amount += t.Amount;
				blob.UploadTransaction(t.TransactionId, t);
			}
            return amount;
		}

        private void UploadOrphanBlocks(string blockHash) 
        {
            Block block = GetBlockByHash(blockHash);
            while (blob.GetBlock(blockHash) == null && block.Height != minimalHeight)
            {
                UploadNewBlock(block);
                blockHash = block.PreviousBlock;
                block = GetBlockByHash(blockHash);
            }
        }

		private IEnumerable<Transaction> GetTransactionsFromBlock(Block block)
		{
			string[] idList = block.TransactionIds;
			Transaction[] transactionsFromBlock = new Transaction[idList.Count()];

			int count = 0;
			foreach (string id in idList)
			{
				JObject transaction = DecodeTransaction(id);
                double amount = 0;
                foreach (JObject v in transaction["vout"])
                {
                    if (v["value"].Type == JTokenType.Float)
                        amount = amount + (double)v["value"]; // assert > 0
                    else throw new Exception("type error in BlockandTransactionTransfer");
                }

                transactionsFromBlock[count] = 
                    new Transaction((int)transaction["version"], (ulong)transaction["locktime"], id, amount);

                count += 1;
			}

			return transactionsFromBlock;
		}

		private string[] GetTransactionIds(JObject obj)
		{
			JToken idList = obj["tx"];
			var transactionIds = new string[idList.Count()];
			int count = 0;
			foreach (var id in idList)
			{
				transactionIds[count] = (string)id;
				count += 1;
			}
			return transactionIds;
		}

		private JObject DecodeTransaction(string txid)
		{
			JToken txHash = Invoke("getrawtransaction", new object[] { txid });
            if (txHash == null)
            {
                throw new Exception("Null transaction hash value");
            }
			return Invoke("decoderawtransaction", new object[] { txHash }) as JObject;
		}

		private Block GetBlockByHash(JToken hashToken)
		{
			var block = Invoke("getblock", new object[] { hashToken }) as JObject;
			return GetBlockFromJObject(block);
		}

		private Block GetBlockFromJObject(JObject obj)
		{
			var hash = (string)obj["hash"];
			var version = (string)obj["version"];
			var previousBlock = (string)obj["previousblockhash"];
            var hashList = new List<string>();
            foreach (var h in obj["nextblockhash"])
            {
                hashList.Add((string)h);
            }
			var merkleRoot = (string)obj["merkleroot"];
			var time = (int)obj["time"];
			var numberOnce = (long)obj["nonce"];
			var transactionIds = GetTransactionIds(obj);
			var numberOfTransactions = transactionIds.Count();
			var size = (int)obj["size"];
			var height = (int)obj["height"];
			return new Block(hash, version, previousBlock, hashList, merkleRoot, time, numberOnce,
				numberOfTransactions, size, height, transactionIds, 0);
		}


	}
}
