﻿using System;
using System.Collections.Generic;
using Storage;
using Storage.Models;

namespace StorageWorkerRole
{
	class Statistics
	{
        // Number of seconds between 3 January 2009 (first BTC ?) and 1 January 1970.
        private const ulong InitialTime = 1230940800;

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
			Blob.UploadBlockBlob("Last_Blocks", (List<Block>) blocklist);
		}

		static void UpdateStatitistics(Block x)
		{
            var stat = Blob.DownloadBlockBlob<Statistic>("General_Statistics");

            stat.NumberOfBlocks += 1;
            stat.NumberOfTransactions += Convert.ToUInt64(x.NumberOfTransactions);

            //Time
            stat.TotalTime = (stat.TotalTime + Convert.ToUInt64(x.Time) - InitialTime);
            stat.AverageTime = stat.TotalTime / stat.NumberOfBlocks;
            stat.VarianceTime = Variance(stat.TotalTime, stat.AverageTime, stat.NumberOfBlocks);
            stat.StandardDeviationTime = Math.Sqrt(stat.VarianceTime);

            // Statistics blocks (BTC)
            // Change with BTC
            stat.SumBtc += Convert.ToUInt64(x.Size);
            stat.AverageBtc = stat.SumBtc / stat.NumberOfBlocks;
            stat.VarianceBtc = Variance(stat.SumBtc, stat.AverageBtc, stat.NumberOfBlocks);
            stat.StandardDeviationBtc = Math.Sqrt(stat.VarianceBtc);

            // Statistics blocks (Transactions).
            stat.NumberOfTransactions += Convert.ToUInt64(x.NumberOfTransactions);
            stat.AverageTrans = stat.NumberOfTransactions / stat.NumberOfBlocks;
            stat.VarianceTrans = Variance(stat.NumberOfTransactions, stat.AverageTrans, stat.NumberOfBlocks);
            stat.StandardDevTrans = Math.Sqrt(stat.VarianceTrans);

            Blob.UploadBlockBlob("General_Statistics", (Statistic) stat);
        }

		static double Variance(ulong sum, double average, ulong nb)
		{
			return sum * (sum - 2 * average) / nb + average * average;
		}

	}
}
