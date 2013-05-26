using System;
using System.Collections.Generic;
using System.Diagnostics;
using Storage;
using Storage.Models;


namespace StorageWorkerRole
{
	class StatisticsCalculator
	{
        // Number of seconds between 3 January 2009 (first BTC ?) and 1 January 1970.
        private const ulong InitialTime = 1230940800;

		public static void Main()
		{
			var hash = Queue.PopMessage<string>();
			var block = Blob.GetBlock(hash);

			UpdateStatitistics(block);
			SortBlocks(block);
		}
        
		private static int CompareBlock(Block x, Block y)
		{
            if (x == null)
                return -1;

            if (y == null)   
                return 1;  
                            
            if (x.Time <= y.Time) 
                return -1;

            return 1; 
		}

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
            var stat = Blob.DownloadBlockBlob<Statistics>("General_Statistics");
            //Charts
            var chartsNbTrans = Blob.DownloadBlockBlob<List<int>>("Charts_Number_Of_Transactions");
            var chartsSizeBlock = Blob.DownloadBlockBlob<List<double>>("Charts_Size_Block");
            var chartsHeighBlock = Blob.DownloadBlockBlob<List<int>>("Charts_Height_Block");
            var chartsTime = Blob.DownloadBlockBlob<List<int>>("Charts_Time"); //abscisse

            stat.NumberOfBlocks += 1;
            stat.NumberOfTransactions += Convert.ToUInt64(x.NumberOfTransactions);

            //Time
            stat.TotalTime = (stat.TotalTime + Convert.ToUInt64(x.Time) - InitialTime);
            stat.AverageTime = stat.TotalTime / stat.NumberOfBlocks;
            stat.VarianceTime = Variance(stat.TotalTime, stat.AverageTime, stat.NumberOfBlocks);
            stat.StandardDeviationTime = Math.Sqrt(stat.VarianceTime);

            // Statistics blocks (BTC)
            stat.SumBtc += Convert.ToUInt64(x.Amount);
            stat.AverageBtc = stat.SumBtc / stat.NumberOfBlocks;
            stat.VarianceBtc = Variance(stat.SumBtc, stat.AverageBtc, stat.NumberOfBlocks);
            stat.StandardDeviationBtc = Math.Sqrt(stat.VarianceBtc);

            // Statistics blocks (Transactions).
            stat.NumberOfTransactions += Convert.ToUInt64(x.NumberOfTransactions);
            stat.AverageTrans = stat.NumberOfTransactions / stat.NumberOfBlocks;
            stat.VarianceTrans = Variance(stat.NumberOfTransactions, stat.AverageTrans, stat.NumberOfBlocks);
            stat.StandardDevTrans = Math.Sqrt(stat.VarianceTrans);

            chartsNbTrans.Add(x.NumberOfTransactions);
            chartsSizeBlock.Add(x.Amount);
            chartsHeighBlock.Add(x.Height);
            chartsTime.Add(x.Time);
            Blob.UploadBlockBlob("Charts_Number_Of_Transactions", (List<int>) chartsNbTrans);
            Blob.UploadBlockBlob("Charts_Size_Block", (List<double>)chartsSizeBlock);
            Blob.UploadBlockBlob("Charts_Height_Block", (List<int>)chartsHeighBlock);
            Blob.UploadBlockBlob("Charts_Time", (List<int>)chartsTime);//abscisse

            Blob.UploadBlockBlob("General_Statistics", (Statistics) stat);
        }

		static double Variance(ulong sum, double average, ulong nb)
		{
			return sum * (sum - 2 * average) / nb + average * average;
		}

        public static void initialise()
        {
            if (Blob.DownloadBlockBlob<List<Block>>("Last_Blocks") == null)
            {
                Block[] liste = { null, null, null, null, null, null, null, null, null, null };
                List<Block> listeini = new List<Block>(liste);
                Blob.UploadBlockBlob("Last_Blocks", (List<Block>)listeini);
            }
            if (Blob.DownloadBlockBlob<Statistics>("General_Statistics") == null)
            {
                Statistics statini = new Statistics();
                Blob.UploadBlockBlob("General_Statistics", statini);
            }
        }

	}
}
