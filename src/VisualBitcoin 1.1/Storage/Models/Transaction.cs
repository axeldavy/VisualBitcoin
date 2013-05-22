using System.ComponentModel.DataAnnotations;

namespace Storage.Models
{
    public class Transaction 
    {
        public Transaction() { } 

        public Transaction(int version, ulong locktime, string transactionId, double amount)
        {
            Version = version;
            Locktime = locktime;
            TransactionId = transactionId;
            Amount = amount;
        }

        // Properties
        public int Version;
        [Display(Name = "Time")]
        public ulong Locktime;
        [Display(Name = "Transaction ID")]
        public string TransactionId;
        public double Amount;
    }
}
