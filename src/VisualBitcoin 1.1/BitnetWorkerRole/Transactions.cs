using System;
using System.Collections.Generic;
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
        JObject listSinceBlock;

        private Uri Url;

        private ICredentials Credentials;

        public BitcoinClient()
        {
            var user = ConfigurationManager.AppSettings["bitcoinuser"];
            var password = ConfigurationManager.AppSettings["bitcoinpassword"];
            this.Credentials = new NetworkCredential(user, password);
            this.Url = new Uri("http://127.0.0.1:8332");

            //JToken lastBlockHash = new JObject(this.storage.DownloadBlobToString("LastBlockSent"));
            //this.lastBlockSent = GetBlockByHash(lastBlockHash);

            this.listSinceBlock = GetLastBlock();

        }

        public JObject InvokeMethod(string a_sMethod, params object[] a_params) //adapted from bitnet, 04/2013, bitnet: COPYRIGHT 2011 Konstantin Ineshin, Irkutsk, Russia.
        {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(Url);
            webRequest.Credentials = Credentials;

            webRequest.ContentType = "application/json-rpc";
            webRequest.Method = "POST";

            JObject joe = new JObject();
            joe["jsonrpc"] = "1.0";
            joe["id"] = "1";
            joe["method"] = a_sMethod;

            if (a_params != null)
            {
                if (a_params.Length > 0)
                {
                    JArray props = new JArray();
                    foreach (var p in a_params)
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
                        using (StreamReader sr = new StreamReader(str))
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
                        using (StreamReader sr = new StreamReader(str))
                        {
                            return JsonConvert.DeserializeObject<JObject>(sr.ReadToEnd());
                        }
                    }
                }
                else throw e;
            }

        }

        public JToken Invoke(string asMethod, params object[] aParams)
        {
            JObject received = InvokeMethod(asMethod, aParams);
            JToken result = received["result"];
            JToken error = received["error"]; // bitcoind always sends an error field
            bool hasError = error.ToString() != ""; // we have to test if the error field contain an error message.
            if (!hasError) return result; // may be null
            else throw new Exception("Invoke:" + error.ToString());
        }

        public void UploadNewBlocks(int max = 1000)
        {
            // TODO, complete this method using the defined helper methods
        }

        public Transaction[] GetTransactionsFromBlock(JObject block)
        {
            JToken txidList = block["tx"];
            Transaction[] transactionsFromBlock = new Transaction[txidList.Count()];

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
                transactionsFromBlock[count] = new Transaction();
                transactionsFromBlock[count].amount = amount;
                transactionsFromBlock[count].txid = (string)txid;
                transactionsFromBlock[count].version = (short)transaction["version"];
                transactionsFromBlock[count].locktime = (long)transaction["locktime"];
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
            JObject lastBlock = Invoke("listsinceblock") as JObject;
            JToken lastBlockHash = lastBlock["lastblock"];

            this.listSinceBlock = GetBlockByHash(lastBlockHash);
            return this.listSinceBlock;
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
        public string txid { set; get; }
        public double amount { get; set; } // double may be not precise enough
        public short version { get; set; }
        public long locktime { get; set; } // nut sure for 'long'
    }

    public class BlockandTransactionTransfer
    {
        public string hashblock { get; set; }
        public short version { get; set; }
        public long size { get; set; }
        public long height { get; set; }
        public string merkleroot { get; set; }
        public long time { get; set; }
        public string bits { get; set; }
        public double difficulty { get; set; }
        public string previousBlockHash { get; set; }
        public Transaction[] transactionArray { get; set; }
        public BitcoinClient bitcoinClient;

        public BlockandTransactionTransfer(JObject block)
        {
            this.bitcoinClient = new BitcoinClient();
            this.transactionArray = this.bitcoinClient.GetTransactionsFromBlock(block);
            this.hashblock = (string)block["hash"];
            this.version = (short)block["version"];
            this.size = (long)block["size"];
            this.height = (long)block["height"];
            this.merkleroot = (string)block["merkleroot"];
            this.time = (long)block["time"];
            this.previousBlockHash = (string)block["previousblockhash"];
            this.bits = (string)block["bits"];
            this.difficulty = (double)block["difficulty"];
        }
    }
}
