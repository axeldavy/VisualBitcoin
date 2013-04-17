namespace Storage.Models
{
    public class Vout 
    {
        // Constructors
        public Vout()
        {
            Hash = "";
            Value = 0;
            ScriptPubKey = "Unavailable";
        }

        public Vout(string hash, ulong value, string scriptPubKey)
        {
            Hash = hash;
            Value = value;
            ScriptPubKey = scriptPubKey;
        }

        // Properties
        public string Hash { set; get; }    //
        public ulong Value { get; set; }    //
        public string ScriptPubKey { set; get; } //
    }
}
