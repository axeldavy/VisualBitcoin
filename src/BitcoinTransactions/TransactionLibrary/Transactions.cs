using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bitnet.Client;
using Newtonsoft.Json.Linq;
using System.Configuration;

namespace TransactionLibrary
{
    public class Transactions
    {
        public void GetTransactions() 
        {
            var configurationString = ConfigurationManager.AppSettings["bitcoinuser"];


        }

    }
}
