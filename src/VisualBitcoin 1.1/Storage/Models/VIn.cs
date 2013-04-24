namespace Storage.Models
{
    public class Vin 
    {
        // Constructors
        public Vin()
        {
            Coinbase = "";
            Sequence = 0;
        }

        public Vin(string coinbase, ulong sequence)
        {
            Coinbase = coinbase;
            Sequence = sequence;
        }

        // Properties
        public string Coinbase { set; get; }
        public ulong Sequence { set; get; }
    }
}
