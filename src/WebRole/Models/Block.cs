using System.Collections.Generic;
using System.ComponentModel;

namespace WebRole.Models
{
    public class Block
    {
        private static int _fresh = 1;

        public Block(int size, decimal total, string relayedby, int transactions, string hash)
        {
            Id = _fresh++;
            Size = size;
            Total = total;
            RelayedBy = relayedby;
            Transactions = transactions;
            Hash = hash;
        }

        [DisplayName("ID")]
        public int Id { get; set; }

        public int Size { get; set; }

        [DisplayName("Relayed By")]
        public string RelayedBy { get; set; }

        public int Transactions { get; set; }
        public decimal Total { get; set; } //BTC
        public string Hash { get; set; }
    }

    public class Example
    {
        public static List<Block> Data = new List<Block>
            {
                new Block(178004, 1337.42m, "BTC Entity", 999,
                          "000000000000029eec98795fb7a77828493aa8bc3c14b01f83f46539580e87ba"),
                new Block(345678, 12345.6m, "Unknown", 789,
                          "0000000000000000123456789abcdef0123456789abcdef0123456789abcdef"),
                new Block(333333, 33333.33m, "33.33.33.33", 333,
                          "00000000000000333333333333333333333333333333333333333333333"),
                new Block(4048, 221.35m, "BTC Entity", 14,
                          "00000000000003240a030a6a5b81c3cef05ab2ebaa6a5569e92db8120bd4ef6e"),
                new Block(3164, 31990m, "Umbrella", 229,
                          " 	00000000000002df1fa55099508ab1bd877356ea829b32ef558820b303a66cef")
            };

        public static Block Find(int id)
        {
            return Data.Find(e => e.Id == id);
        }
    }
}