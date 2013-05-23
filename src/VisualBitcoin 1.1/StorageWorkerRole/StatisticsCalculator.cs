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
        private Blob blob;
        private Queue queue;
        private List<Block> blocklist; 

        public StatisticsCalculator(Queue queue, Blob blob)
        {
            this.queue = queue;
            this.blob = blob;

            if (blob.GetBlock("Last_Blocks") == null)
            {
                Block[] list = { null, null, null, null, null, null, null, null, null, null };
                List<Block> blocklist = new List<Block>(list);
                blob.UploadBlock("Last_Blocks", (List<Block>)blocklist);
                this.blocklist = blocklist;
            }
            else
            {
                this.blocklist = blob.GetLastBlocks(); 
            }
            if (blob.GetStatistics<Statistics>("General_Statistics") == null)
            {
                Statistics statini = new Statistics();
                blob.UploadStatistics("General_Statistics", statini);
            }
        }

        public void PerformBlockStatistics()
        {
            var hash = queue.PopMessage<string>();
            var block = blob.GetBlock(hash);

            UpdateStatitistics(block);
            SortBlocks(block);
        }

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
		private void SortBlocks(Block block)
		{
			blocklist.Add(block);
			blocklist.Sort(CompareBlock);
			blocklist.RemoveAt(0);
			blob.UploadBlock("Last_Blocks", (List<Block>) blocklist);
		}

		void UpdateStatitistics(Block x)
		{
            Statistics stat = blob.GetStatistics<Statistics>("General_Statistics");
            //Charts
            List<int> chartsNbTrans = blob.GetStatistics<List<int>>("Charts_Number_Of_Transactions");
            List<int> chartsSizeBlock = blob.GetStatistics<List<int>>("Charts_Size_Block");
            List<int> chartsHeighBlock = blob.GetStatistics<List<int>>("Charts_Height_Block");
            List<int> chartsTime = blob.GetStatistics<List<int>>("Charts_Time"); //abscisse

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

            chartsNbTrans.Add(x.NumberOfTransactions);
            chartsSizeBlock.Add(x.Size);
            chartsHeighBlock.Add(x.Height);
            chartsTime.Add(x.Time);
            blob.UploadStatistics("Charts_Number_Of_Transactions", chartsSizeBlock);
            blob.UploadStatistics("Charts_Size_Block", chartsSizeBlock);
            blob.UploadStatistics("Charts_Height_Block", chartsHeighBlock);
            blob.UploadStatistics("Charts_Time", chartsTime);//abscisse

            blob.UploadStatistics("General_Statistics", stat);
        }

		static double Variance(ulong sum, double average, ulong nb)
		{
			return sum * (sum - 2 * average) / nb + average * average;
		}

	}
}
