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

        public JObject GetTransaction(String txid) 
        {
            return null;
        }

        public JObject GetLastBlock()
        {
            return this.bitClient.InvokeMethod("listsinceblock")["result"] as JObject;
        }

        public JObject GetPreviousBlock(JObject obj)
        {
            return null;
        }
    }
}
