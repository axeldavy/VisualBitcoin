namespace Storage.Models
{
    public class Transactions // vin and vout not included
    {
        // Constructors
        public Transactions()
        {
            Hash = "";
            Version = 0;
            Vin_size = 0;
            Vout_size = 0;
            Locktime = 0;
            Size = 0;
            Relayed_by = "127.0.0.1";
            Height = 0;
            Txid = "";
            BlockHash = "";
            Inputs = new Vin[Vin_size];
            Outputs = new Vout[Vout_size];
        }

        public Transactions(string hash, int version,  int vin_size, int vout_size, ulong locktime, int size, string relayed_by, int height, 
                            string txid, string blockhash, Vin[] inputs, Vout[] outputs)
        {
            Hash = hash;
            Txid = txid;
            Version = version;
            Locktime = locktime;
            Vin_size = vin_size;
            Vout_size = vout_size;
            Size = size;
            Relayed_by = relayed_by;
            Height = height;
            BlockHash = blockhash;
            Inputs = inputs;
            Outputs = outputs;
        }

        public Transactions(string hash, int version, int vin_size, int vout_size, ulong locktime, int size, string relayed_by, int height,
                    string txid, string blockhash)
        {
            Hash = hash;
            Txid = txid;
            Version = version;
            Locktime = locktime;
            Vin_size = vin_size;
            Vout_size = vout_size;
            Size = size;
            Relayed_by = relayed_by;
            Height = height;
            BlockHash = blockhash;
            Inputs = new Vin[Vin_size];
            Outputs = new Vout[Vout_size];
        }

        // Properties
        public string Hash { get; set; } 
        public int Version { get; set; }
        public int Vin_size { get; set; }
        public int Vout_size { get; set; }
        public ulong Locktime { get; set; } // 0 if Unavailable
        public int Size { get; set; }
        public string Relayed_by { get; set; } // IP of the person who relays this transaction
        public int Height { get; set; }
        public string Txid { set; get; }
        public Vin[] Inputs { get; set; }
        public Vout[] Outputs { get; set; }
        public string BlockHash { get; set; }
    }
}
