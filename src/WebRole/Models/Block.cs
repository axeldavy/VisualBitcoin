using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebRole.Models
{
    public class Block
    {
        [DisplayName("ID")]
        public int Id { get; set; }
        public int Size { get; set; }
        [DisplayName("Relayed By")]
        public string RelayedBy { get; set; }
        public int Blocks { get; set; }
        public decimal Total { get; set; } //BTC
        public string Hash { get; set; }

        public Block(int id, int size, decimal total, string relayedby, int blocks, string hash)
        {
            Id = id;
            Size = size;
            Total = total;
            RelayedBy = relayedby;
            Blocks = blocks;
            Hash = hash;
        }
    }

    public class Example
    {
        public static List<Block> Data = new List<Block>()
            {
                new Block(1,178004,1337.42m,"BTCEntity",999,"000000000000029eec98795fb7a77828493aa8bc3c14b01f83f46539580e87ba"),
                new Block(2,345678,12345.6m,"Unknown",789,"0000000000000000123456789abcdef0123456789abcdef0123456789abcdef"),
                new Block(3,333333,33333.33m,"33.33.33.33",333,"00000000000000333333333333333333333333333333333333333333333")
            };

        public static Block Find(int id)
        {
            return Data.Find(e => e.Id == id);
        }
    }
}