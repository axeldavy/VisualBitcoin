namespace Storage.Models
{
    public class Vin 
    {
        // Constructors
        public Vin()
        {
            prev_out = new Prev_out();
            ScriptSig = "Unavailable";
        }

        public Vin(Prev_out _prev_out, string scriptSig)
        {
            prev_out = _prev_out;
            ScriptSig = scriptSig;
        }

        // Properties
        public Prev_out prev_out { set; get; } //From wich vout it came 
        public string ScriptSig { set; get; }  //Don't know for what 
    }
}
