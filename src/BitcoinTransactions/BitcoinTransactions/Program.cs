using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TransactionLibrary;

namespace BitcoinTransactions
{
    class Program
    {
        static void Main(string[] args)
        {
            Transactions trans = new Transactions();
            Console.WriteLine(trans.GetTransactionsFromBlock(trans.GetLastBlock()));
            Console.ReadKey();
        }
    }
}
