using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Storage.Models
{
    public class Statistic
    {
        public ulong NumberOfBlocks{ get; set; }
        // Time.
        public ulong TotalTime{ get; set; }
        public double AverageTime{ get; set; }
        public double VarianceTime{ get; set; }
        public double StandardDeviationTime{ get; set; }

        // Statistics blocks BTC.
        public ulong SumBtc{ get; set; }
        public double AverageBtc{ get; set; }
        public double StandardDeviationBtc{ get; set; }
        public double VarianceBtc{ get; set; }

        // Statistics blocks transactions.
        public ulong NumberOfTransactions{ get; set; }
        public double AverageTrans{ get; set; }
        public double VarianceTrans{ get; set; }
        public double StandardDevTrans{ get; set; }

        public Statistic()
		{
            NumberOfBlocks = 0;
            NumberOfTransactions = 0;
               
            //Btc
            SumBtc = 0;
            AverageBtc = 0;
            VarianceBtc = 0;
            StandardDeviationBtc = 0;
                          
            //Time
            TotalTime = 0;
            AverageTime = 0;
            VarianceTime = 0;
            StandardDeviationTime = 0;

            //Transactions
            AverageTrans = 0;
            VarianceTrans = 0;
            StandardDevTrans = 0;
		}

        public Statistic(ulong numberOfBlocks,ulong totalTime,double averageTime,double varianceTime,
		  double standardDeviationTime,ulong sumBtc,double averageBtc,double standardDeviationBtc,double varianceBtc,
		  ulong numberOfTransactions, double averageTrans, double varianceTrans, double standardDevTrans)
        {
            NumberOfBlocks = numberOfBlocks;
            NumberOfTransactions = numberOfTransactions;
               
            //Btc
            SumBtc = sumBtc;
            AverageBtc = averageBtc;
            VarianceBtc = varianceBtc;
            StandardDeviationBtc = standardDeviationBtc;
                          
            //Time
            TotalTime = totalTime;
            AverageTime = averageTime;
            VarianceTime = varianceTime;
            StandardDeviationTime = standardDeviationTime;

            //Transactions
            AverageTrans = averageTrans;
            VarianceTrans = varianceTrans;
            StandardDevTrans = standardDevTrans;
        }
    }
}
