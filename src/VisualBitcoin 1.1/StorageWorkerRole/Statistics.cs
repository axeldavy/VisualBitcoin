using System;
using Storage;
using Storage.Models;

namespace StorageWorkerRole
{
	class Statistics
	{
		// Statistics Variable :
        static ulong _numberOfBlocks; // Number_of_Blocks
        
        // Time.
        static ulong _totalTime;
        static double _averageTime;
        static double _varianceTime;
		// ReSharper disable NotAccessedField.Local
        static double _standardDeviationTime;
		// Number of seconds between 3 January 2009 (first BTC ?) and 1 January 1970.
        private const ulong InitialTime = 1230940800;

        // Statistics blocks BTC.
        private static ulong _sommeBtc;
        static double _averageBtc;
        static double _standardDeviationBtc;
        static double _varianceBtc;
		// Pourcentage bloc invalide ??
        //static double Invalide_block;

        // Statistics blocks transactions.
        static ulong _numberOfTransactions;
        static double _averageT;
        static double _varianceT;
        static double _standardDeviationT;

        // Statistics transactions.
		/* // Number_of_transactions
		static ulong Number_of_Transactions;
		static double Standard_deviation_T;
		static double Variance_T;*/


		// Retrieve a block from queue and call Review_Statics.
        static void Main()
        {
            var hash = Queue.PopMessage<string>();
            var block = Blob.GetBlock(hash);
            Review_Statistics(block);
        }

		// Update statistics.
        static void Review_Statistics(Block x)
        {
            _numberOfBlocks += 1;
            _numberOfTransactions += Convert.ToUInt64(x.NumberOfTransactions); 

            // Qualite de temps de distribution des blocs.
            _totalTime = (_totalTime + Convert.ToUInt64(x.ReceivedTime) - InitialTime);
			// ReSharper disable PossibleLossOfFraction
            _averageTime = _totalTime / _numberOfBlocks;
            _varianceTime = Variance(_totalTime, _averageTime, _numberOfBlocks);
            _standardDeviationTime = Math.Sqrt(_varianceTime);
            

            // Statistics blocks (BTC).
			// Changer avec BTC
			_sommeBtc += Convert.ToUInt64(x.Size);
			_averageBtc = _sommeBtc / _numberOfBlocks;
            _varianceBtc = Variance(_sommeBtc, _averageBtc, _numberOfBlocks);
            _standardDeviationBtc = Math.Sqrt(_varianceBtc);


            // Statistics blocks (Transactions).
            _numberOfTransactions += Convert.ToUInt64(x.NumberOfTransactions);
            _averageT = _numberOfTransactions / _numberOfBlocks;
            _varianceT = Variance(_numberOfTransactions, _averageT, _numberOfBlocks);
            _standardDeviationT = Math.Sqrt(_varianceT);
        }

        static double Variance(ulong somme,double average, ulong nb)
        {
            return somme*(somme - 2*average)/nb + average*average;
        }
	}
}
