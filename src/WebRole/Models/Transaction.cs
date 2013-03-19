using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebRole.Models
{
    public class Transaction
    {
        public string RelayedBy { get; set; }
        public int Transactions { get; set; }
        public decimal Total { get; set; } //BTC
        public string Hash { get; set; }

        public Transaction(decimal total, string relayedby, int transactions, string hash)
        {
            Total = total;
            RelayedBy = relayedby;
            Transactions = transactions;
            Hash = hash;
        }
    }

    public class Example
    {
        public static Transaction[] Data = new Transaction[]
            {
                new Transaction(1337.42m,"BTCEntity",999,"000000000000029eec98795fb7a77828493aa8bc3c14b01f83f46539580e87ba"),
                new Transaction(12345.6m,"Unknown",789,"0000000000000000123456789abcdef0123456789abcdef0123456789abcdef"),
                new Transaction(33333.33m,"33.33.33.33",333,"000000000000000000000000000333333333333333333333333333333333333")
            };
    }
}