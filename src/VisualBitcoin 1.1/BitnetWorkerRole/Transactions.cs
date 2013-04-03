using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Net;
using System.Configuration;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BitnetWorkerRole
{
    public class BitcoinClient
    {
        //JObject lastBlockSent;
        JObject _listSinceBlock;

        private readonly Uri _url;

        private readonly ICredentials _credentials;

        public BitcoinClient()
        {
            var user = ConfigurationManager.AppSettings["bitcoinuser"];
            var password = ConfigurationManager.AppSettings["bitcoinpassword"];
            _credentials = new NetworkCredential(user, password);
            _url = new Uri("http://127.0.0.1:8332");

            //JToken lastBlockHash = new JObject(this.storage.DownloadBlobToString("LastBlockSent"));
            //this.lastBlockSent = GetBlockByHash(lastBlockHash);

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
            // TODO, complete this method using the defined helper methods
        }

        public Transaction[] GetTransactionsFromBlock(JObject block)
        {
            JToken txidList = block["tx"];
            var transactionsFromBlock = new Transaction[txidList.Count()];

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
                transactionsFromBlock[count] = new Transaction
	                {
		                Amount = amount,
		                Txid = (string) txid,
		                Version = (short) transaction["version"],
		                Locktime = (long) transaction["locktime"]
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

        public JObject GetLastBlock()
        {
            var lastBlock = Invoke("listsinceblock") as JObject;
	        Debug.Assert(lastBlock != null, "lastBlock != null");
	        JToken lastBlockHash = lastBlock["lastblock"];

            _listSinceBlock = GetBlockByHash(lastBlockHash);
            return _listSinceBlock;
        }

        public JObject GetPrevBlock(JObject obj)
        {
            JToken prevBlockHash = obj["prevblockhash"];
            return GetBlockByHash(prevBlockHash);
        }
        public JObject GetNextBlock(JObject obj)
        {
            JToken nextBlockHash = obj["nextblockhash"];
            return GetBlockByHash(nextBlockHash);
        }

        private JObject GetBlockByHash(JToken hashToken)
        {
            return Invoke("getblock", new object[] { hashToken }) as JObject;
        }
    }

    public class Transaction // vin, vout missing
    {
        public string Txid { set; get; }
        public double Amount { get; set; } // double may be not precise enough
        public short Version { get; set; }
        public long Locktime { get; set; } // nut sure for 'long'
    }

    public class BlockandTransactionTransfer
    {
        public string Hashblock { get; set; }
        public short Version { get; set; }
        public long Size { get; set; }
        public long Height { get; set; }
        public string Merkleroot { get; set; }
        public long Time { get; set; }
        public string Bits { get; set; }
        public double Difficulty { get; set; }
        public string PreviousBlockHash { get; set; }
        public Transaction[] TransactionArray { get; set; }
        public BitcoinClient BitcoinClient;

        public BlockandTransactionTransfer(JObject block)
        {
            BitcoinClient = new BitcoinClient();
            TransactionArray = BitcoinClient.GetTransactionsFromBlock(block);
            Hashblock = (string)block["hash"];
            Version = (short)block["version"];
            Size = (long)block["size"];
            Height = (long)block["height"];
            Merkleroot = (string)block["merkleroot"];
            Time = (long)block["time"];
            PreviousBlockHash = (string)block["previousblockhash"];
            Bits = (string)block["bits"];
            Difficulty = (double)block["difficulty"];
        }
    }
}
