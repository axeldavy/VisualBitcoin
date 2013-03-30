using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using Bitnet.Client;
using Newtonsoft.Json.Linq;
using System.Configuration;

namespace TransactionLibrary
{
    public class Transactions
    {
        BitnetClient bitClient;
        JObject lastBlockStored;

        public Transactions()
        {
            var user = ConfigurationManager.AppSettings["bitcoinuser"];
            var password = ConfigurationManager.AppSettings["bitcoinpassword"];
            this.bitClient = new BitnetClient("http://127.0.0.1:8332");
            this.bitClient.Credentials = new NetworkCredential(user, password);

            //TODO: get last block stored from Storage
        }

        public void PutBlocks(int max = 1000)
        {

        }

        public Boolean HasNewBlocks()
        {
            return false;
        }

        public void UploadNewBlocks(JArray arr) 
        { 

        }

        public JArray GetTransactionsFromBlock(JObject block) 
        {
            JArray transactionsFromBlock = new JArray();
            JToken txidList = block["tx"];
            foreach (JValue txid in txidList)
            {
                JObject transaction = DecodeTransaction(txid);
                if (transaction != null)
                {
                    transaction = ParseTransaction(transaction); // Parse relevant data from JObject
                    transactionsFromBlock.Add(transaction);
                }
            }

            return transactionsFromBlock;
                
        }

        private JObject ParseTransaction(JObject tx)
        {
            JObject result = new JObject();
            JArray transactionList = new JArray();
            JToken txid = tx["txid"];
            JToken vout = tx["vout"];
            foreach (JObject v in vout)
            {
                JObject arr = new JObject();
                arr.Add(new JProperty("txNumber", v["n"]));
                arr.Add(new JProperty("amountPaid", v["value"]));
                arr.Add(new JProperty("receivedByAddresses", v["scriptPubKey"]["addresses"]));

                transactionList.Add(arr);
            }

            result.Add(txid.ToString(), transactionList);
            return result;

        }

        public JObject DecodeTransaction(JValue txid) 
        {
            JObject txObject = this.bitClient.InvokeMethod("getrawtransaction", new object[] { txid }) as JObject;
            JToken txHash = txObject["result"];

            if (txHash == null) throw new Exception("null txHash value");
            return this.bitClient.InvokeMethod("decoderawtransaction", new object[] { txHash })["result"] as JObject;
        }

        public JObject GetLastBlock()
        {
            JObject lastBlock = this.bitClient.InvokeMethod("listsinceblock")["result"] as JObject;
            JToken lastBlockHash = lastBlock["lastblock"];
            //Console.WriteLine(lastBlockHash);
            //Console.ReadKey();
            
            return GetBlockByHash(lastBlockHash);
        }

        public JObject GetPreviousBlock(JObject obj)
        {
            JToken prevBlockHash = obj["previoussinceblock"];
            return GetBlockByHash(prevBlockHash) ;
        }

        private JObject GetBlockByHash(JToken hashToken)
        {
            return this.bitClient.InvokeMethod("getblock", new object[] { hashToken })["result"]  as JObject;
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
        public BlockandTransactionTransfer(JObject Block)
        {
            var temp = new Transactions();
            this.hashblock = (string)Block["hash"];
            JToken txidList = Block["tx"];
            int count = 0;
            foreach (JValue txid in txidList)
            {
                count = count + 1;
            }
            int sizeArray = count;
            var transactionsFromBlock = new Transaction[sizeArray];
            count = 0;
            foreach (JValue txid in txidList)
            {
                //Console.WriteLine(txid);
                //Console.ReadKey();
                JObject transaction = temp.DecodeTransaction(txid);
                double amountCalculate = 0;
                foreach (JObject v in transaction["vout"])
                {
                    if (v["value"].Type == JTokenType.Float) // à tester
                        amountCalculate = amountCalculate + (double)v["value"]; // assert >0
                    else throw new Exception("type error in BlockandTransactionTransfer");
                }
                transactionsFromBlock[count] = new Transaction();
                transactionsFromBlock[count].amount = amountCalculate;
                transactionsFromBlock[count].txid = (string)txid;
                transactionsFromBlock[count].version = (short)transaction["version"];
                transactionsFromBlock[count].locktime = (long)transaction["locktime"];
                count = count + 1;
            }
            this.transactionArray = transactionsFromBlock;
            this.version = (short)Block["version"];
            this.size = (long)Block["size"];
            this.height = (long)Block["height"];
            this.merkleroot = (string)Block["merkleroot"];
            this.time = (long)Block["time"];
            this.previousBlockHash = (string)Block["previousblockhash"];
            this.bits = (string)Block["bits"];
            this.difficulty = (double)Block["difficulty"];
        }
        public void toTest()
        {
            Console.WriteLine("hash: {0}",hashblock);
            Console.WriteLine("version: {0}", version);
            Console.WriteLine("size: {0}", size);
            Console.WriteLine("height: {0}", height);
            Console.WriteLine("time: {0}", time);
            Console.WriteLine("difficulty: {0}", difficulty);
            Console.WriteLine("merkleroot: {0}", merkleroot);
            Console.WriteLine("bits: {0}", bits);
            Console.WriteLine("previousblockhash: {0}", previousBlockHash);
            Console.WriteLine("transactions: ");
            foreach (Transaction t in transactionArray)
            {
                Console.WriteLine("txid: {0}", t.txid);
                Console.WriteLine("amount: {0}", t.amount);
                Console.WriteLine("locktime: {0}", t.locktime);
                Console.WriteLine("version: {0}", t.version);
            }
        }
    }
}
