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
            JToken txidList = block["result"]["tx"];
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
            JToken txid = tx["result"]["txid"];
            JToken vout = tx["result"]["vout"];
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

        private JObject DecodeTransaction(JValue txid) 
        {
            JObject txObject = this.bitClient.InvokeMethod("getrawtransaction", new object[] { txid }) as JObject;
            JToken txHash = txObject["result"];

            if (txHash == null) return null;
            return this.bitClient.InvokeMethod("decoderawtransaction", new object[] { txHash }) as JObject;
        }

        public JObject GetLastBlock()
        {
            JObject lastBlock = this.bitClient.InvokeMethod("listsinceblock")["result"] as JObject;
            JToken lastBlockHash = lastBlock["lastblock"];

            return GetBlockByHash(lastBlockHash);
        }

        public JObject GetPreviousBlock(JObject obj)
        {
            JToken prevBlockHash = obj["previoussinceblock"];
            return GetBlockByHash(prevBlockHash);
        }

        private JObject GetBlockByHash(JToken hashToken)
        {
            return this.bitClient.InvokeMethod("getblock", new object[] { hashToken }) as JObject;
        }
    }
}
