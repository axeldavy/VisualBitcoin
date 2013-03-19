using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebRole.Models
{
    public class Block
    {
        public int Id { get; set; }
        public int Size { get; set; }
        public string RelayedBy { get; set; }
        public int Blocks { get; set; }
        public decimal Total { get; set; } //BTC
        public string Hash { get; set; }

        public Block(int id, decimal total, string relayedby, int blocks, string hash)
        {
            Id = id;
            Total = total;
            RelayedBy = relayedby;
            Blocks = blocks;
            Hash = hash;
            Size = 0;
        }
    }

    public class Example
    {
        public static Block[] Data = new[]
            {
                new Block(0,1337.42m,"BTCEntity",999,"000000000000029eec98795fb7a77828493aa8bc3c14b01f83f46539580e87ba"),
                new Block(1,12345.6m,"Unknown",789,"0000000000000000123456789abcdef0123456789abcdef0123456789abcdef"),
                new Block(2,33333.33m,"33.33.33.33",333,"00000000000000333333333333333333333333333333333333333333333")
            };
    }
}