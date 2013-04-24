using System;
using System.Globalization;
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
        // Added parsing here
// ReSharper disable UnusedMember.Local
        static void Main()
        {
            var hash = Queue.PopMessage<string>();
            var block = Blob.GetBlock(hash);
            
            Review_Statistics(block);
            Sort_blocks(block);

            /*
             * Can't add this because I can't test it(((
            //Blob.DeleteBlockBlob(hash);
            
            //Sending clear block to the Blob storage
            Blob.UploadBlockBlob<BlockClear>(clearBlock.Hash, clearBlock);
            //Sending transactions to the Blob storage
            foreach (Transactions singleTransaction in parsed.Item2)
            {
                Blob.UploadBlockBlob<Transactions>(singleTransaction.Txid, singleTransaction);
            }*/
        }

        //TODO : à déplacer et initialiser les Higher_Block_Sort_
	    private static void Sort_blocks(Block block)
	    {
	        int n = 9; // number of blocks - 1
            var blockinarray = Blob.GetBlockHigh("Higher_Block_Sort_" + n.ToString(CultureInfo.InvariantCulture));
            var blockhigh = new BlockHigh(block.Hash, block.Time);
            

            while ((n >= 0) && (block.Time) < (blockinarray.Time))
            {
                blockinarray = Blob.GetBlockHigh("Higher_Block_Sort_"+ n.ToString(CultureInfo.InvariantCulture));// Attention sur le Hash
                n--;
            }
	        if (n >= 0)//simplifier
	        {
                var previousblockhigh = Blob.GetBlockHigh("Higher_Block_Sort_" + n.ToString(CultureInfo.InvariantCulture));
// ReSharper disable RedundantAssignment
                var permuteblockhigh = new BlockHigh();
// ReSharper restore RedundantAssignment
                Blob.UploadBlockBlob("Higher_Block_Sort_" + n.ToString(CultureInfo.InvariantCulture), blockhigh);
	            n--;
                while (n >= 0)
                {
                    permuteblockhigh = Blob.GetBlockHigh("Higher_Block_Sort_" + n.ToString(CultureInfo.InvariantCulture));
                    Blob.UploadBlockBlob("Higher_Block_Sort_" + n.ToString(CultureInfo.InvariantCulture), previousblockhigh);
                    previousblockhigh = permuteblockhigh;
                    n--;
                }
	        }

	    }

	    // Update statistics.
        static void Review_Statistics(Block x)
        {
            _numberOfBlocks += 1;
            _numberOfTransactions += Convert.ToUInt64(x.NumberOfTransactions); 

            // Qualite de temps de distribution des blocs.
            _totalTime = (_totalTime + Convert.ToUInt64(x.Time) - InitialTime);
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
