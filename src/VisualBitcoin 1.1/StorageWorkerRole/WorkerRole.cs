using System;
using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Configuration;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.ServiceRuntime;
using Storage;
using Storage.Models;

namespace StorageWorkerRole
{
	public class WorkerRole : RoleEntryPoint
	{
        private StatisticsCalculator statCalculator;
        private Blob blob;
        private Queue queue;

		public override void Run()
		{
			Trace.WriteLine("Entry point called",
				"VisualBitcoin.StorageWorkerRole.WorkerRole Information");

            while (true)
			{
                statCalculator.PerformBlockStatistics();
				Thread.Sleep(500);
				Trace.WriteLine("Working",
					"VisualBitcoin.StorageWorkerRole.WorkerRole Information");
			}
		}

		public override bool OnStart()
		{
			Trace.WriteLine("On start",
				"VisualBitcoin.StorageWorkerRole.WorkerRole Information");

			// Storage configuration and start.
			var connectionString = CloudConfigurationManager.GetSetting("StorageConnectionString");
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
            blob = new Blob(storageAccount);
            queue = new Queue(storageAccount.CreateCloudQueueClient());

            // TODO: needed three BlobContainers: for brute blocks, for clear blocks, for transactions
            Trace.WriteLine("On start", "VisualBitcoin.StorageWorkerRole.Statistics Information");
            statCalculator = new StatisticsCalculator(queue, blob);
            
			// Set the maximum number of concurrent connections 
			ServicePointManager.DefaultConnectionLimit = 12;
			
			return base.OnStart();
		}
	}
}
