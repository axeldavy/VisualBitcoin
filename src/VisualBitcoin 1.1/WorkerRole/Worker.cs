using System;
using System.Net;
using Bitnet.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WorkerRole
{
    public class Worker
    {
        public void GetBitcoinTransactions() 
        {
            BitnetClient bc = new BitnetClient("http://127.0.0.1:8332");
            bc.Credentials = new NetworkCredential("user", "pass");

            var transactions = bc.InvokeMethod("listtransactions")["result"] as JArray;
            Console.WriteLine(transactions);

        }
    }
}
