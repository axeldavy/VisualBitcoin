using System.ComponentModel.DataAnnotations;

namespace Storage.Models
{
    public class Transactions // vin and vout not included
    {
        // Constructors
        public Transactions()
        {
            Version = 0;
            Vin_size = 0;
            Vout_size = 0;
            Locktime = 0;
            Txid = "";
            BlockHash = "";
            Inputs = new Vin[Vin_size];
            Outputs = new Vout[Vout_size];
        }

        public Transactions(int version,  int vin_size, int vout_size, ulong locktime,  
                            string txid, string blockhash, Vin[] inputs, Vout[] outputs)
        {
            Txid = txid;
            Version = version;
            Locktime = locktime;
            Vin_size = vin_size;
            Vout_size = vout_size;
            BlockHash = blockhash;
            Inputs = inputs;
            Outputs = outputs;
        }

        public Transactions(int version, int vin_size, int vout_size, ulong locktime,
                    string txid, string blockhash)
        {
            Txid = txid;
            Version = version;
            Locktime = locktime;
            Vin_size = vin_size;
            Vout_size = vout_size;
            BlockHash = blockhash;
            Inputs = new Vin[Vin_size];
            Outputs = new Vout[Vout_size];
        }

        // Properties
        public int Version { get; set; }
        public int Vin_size { get; set; }
        public int Vout_size { get; set; }
        [Display(Name = "Time")]
        public ulong Locktime { get; set; } // 0 if Unavailable
        [Display(Name = "Transaction ID")]
        public string Txid { set; get; }
        public Vin[] Inputs { get; set; }
        public Vout[] Outputs { get; set; }
        public string BlockHash { get; set; }
    }
}
