namespace Storage.Models
{
    public class Prev_out 
    {
        // Constructors
        public Prev_out ()
        {
            Hash = "";
            Value = 0;
            TxId = "";
            N = 0;
        }

        public Prev_out(string hash, ulong value, string txId, int n)
        {
            Hash = hash;
            Value = value;
            TxId = txId;
            N = n;
        }

        // Properties
        public string Hash { set; get; }
        public ulong Value { get; set; } 
        public string TxId { set; get; }
        public int N { set; get; }      //don't know for what it serves
    }
}
