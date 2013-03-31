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
            BitcoinClient trans = new BitcoinClient();
            //Console.WriteLine(trans.GetTransactionsFromBlock(trans.GetLastBlock()));
            BlockandTransactionTransfer b = new BlockandTransactionTransfer( trans.GetLastBlock());
            b.toTest();
            Console.ReadKey();
        }
    }
}
