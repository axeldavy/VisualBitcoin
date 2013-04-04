namespace Storage.Models
{
    public class Transactions // vin and vout not included
    {
        // Constructors
        public Transactions()
        {
            Txid = "";
            Amount = 0;
            Version = 0;
            Locktime = 0;
        }

        public Transactions(string txid, double amount, int version, int locktime)
        {
            Txid = txid;
            Amount = amount;
            Version = version;
            Locktime = locktime;
        }

        // Properties
        public string Txid { set; get; }
        public double Amount { get; set; } // TODO, check precision
        public int Version { get; set; }
        public int Locktime { get; set; } // TODO, check precision
    }
}
