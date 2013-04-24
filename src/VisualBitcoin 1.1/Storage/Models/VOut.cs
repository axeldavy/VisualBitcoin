namespace Storage.Models
{
    public class Vout 
    {
        // Constructors
        public Vout()
        {
            Value = 0;
            N = 0;
        }

        public Vout(ulong value, int n)
        {
            Value = value; 
            N = n;
        }

        // Properties
        public ulong Value { get; set; }   
        public int N { set; get; }    
    }
}
