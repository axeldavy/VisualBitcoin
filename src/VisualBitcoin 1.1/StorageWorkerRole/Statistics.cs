﻿using System;
using System.Collections.Generic;
using Storage;
using Storage.Models;

namespace StorageWorkerRole
{
	class Statistics
	{
		// Statistics Variable :
		static ulong numberOfBlocks;

		// Time.
		static ulong totalTime;
		static double averageTime;
		static double varianceTime;
		static double standardDeviationTime;
		// Number of seconds between 3 January 2009 (first BTC ?) and 1 January 1970.
		private const ulong InitialTime = 1230940800;

		// Statistics blocks BTC.
		private static ulong sumBtc;
		static double averageBtc;
		static double standardDeviationBtc;
		static double varianceBtc;

		// Statistics blocks transactions.
		static ulong numberOfTransactions;
		static double averageTrans;
		static double varianceTrans;
		static double standardDevTrans;

		// Statistics transactions.
		/* // Number_of_transactions
		static ulong Number_of_Transactions;
		static double Standard_deviation_T;
		static double Variance_T;*/

		static void Main()
		{
			var hash = Queue.PopMessage<string>();
			var block = Blob.GetBlock(hash);

			UpdateStatitistics(block);
			SortBlocks(block);

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

		//List<Block> Lastblocks = new List<Block>();

		private static int CompareBlock(Block x, Block y)
		{
            if (x == null)
                return -1;

            if (y == null)   // to verify: can x and y be null at the same time?
                return 1;    // x and y can be null at the same time (Christophe)
                            
            if (x.Time <= y.Time) // won't crash if x or y is null (proposed clean up could have crashed if y == null)
                return -1;

            return 1; 
		}

		//TODO: initialiser à 10 null
		private static void SortBlocks(Block block)
		{
			var blocklist = Blob.DownloadBlockBlob<List<Block>>("Last_Blocks");
			blocklist.Add(block);
			blocklist.Sort(CompareBlock);
			blocklist.RemoveAt(0);
			Blob.UploadBlockBlob("Last_Blocks", (List<Block>)blocklist);
		}

		static void UpdateStatitistics(Block x)
		{
            /* Ne pas toucher (debut)
                numberOfBlocks = Blob.DownloadBlockBlob<>("Number_of_blocks");
                numberOfTransactions = Blob.DownloadBlockBlob<>("Number_of_transactions");
                totalTime = Blob.DownloadBlockBlob<>("Total_time");
            */
            numberOfBlocks += 1;
			numberOfTransactions += Convert.ToUInt64(x.NumberOfTransactions);

			totalTime = (totalTime + Convert.ToUInt64(x.Time) - InitialTime);
			averageTime = totalTime / numberOfBlocks;
			varianceTime = Variance(totalTime, averageTime, numberOfBlocks);
			standardDeviationTime = Math.Sqrt(varianceTime);

            // Statistics blocks (BTC)
			// Change with BTC
			sumBtc += Convert.ToUInt64(x.Size);
			averageBtc = sumBtc / numberOfBlocks;
			varianceBtc = Variance(sumBtc, averageBtc, numberOfBlocks);
			standardDeviationBtc = Math.Sqrt(varianceBtc);

			// Statistics blocks (Transactions).
			numberOfTransactions += Convert.ToUInt64(x.NumberOfTransactions);
			averageTrans = numberOfTransactions / numberOfBlocks;
			varianceTrans = Variance(numberOfTransactions, averageTrans, numberOfBlocks);
			standardDevTrans = Math.Sqrt(varianceTrans);

            /* Ne pas toucher (fin)
                Blob.UploadBlockBlob("Number_of_blocks",numberOfBlocks);
                Blob.UploadBlockBlob("Number_of_transactions",numberOfTransactions);
               
                //Time
                Blob.UploadBlockBlob("Total_time",totalTime);
                Blob.UploadBlockBlob("Average_time",averageTime);
                Blob.UploadBlockBlob("Variance_time",varianceTime);
                Blob.UploadBlockBlob("Standard_deviation_time",standardDeviationTime);
                
            */
        }

		static double Variance(ulong sum, double average, ulong nb)
		{
			return sum * (sum - 2 * average) / nb + average * average;
		}

	}
}
