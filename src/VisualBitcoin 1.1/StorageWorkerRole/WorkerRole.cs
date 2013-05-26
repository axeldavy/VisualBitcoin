using System;
using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Configuration;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.ServiceRuntime;
using Storage;
using Storage.Models;

namespace StorageWorkerRole
{
	public class WorkerRole : RoleEntryPoint
	{
		public override void Run()
		{
			Trace.WriteLine("Entry point called",
				"VisualBitcoin.StorageWorkerRole.WorkerRole Information");

            while (true)
			{
                StatisticsCalculator.Main();
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
			var resetBlobBlocksEnableString = CloudConfigurationManager.GetSetting("ResetBlobBlocksEnable");
			var resetQueueMessagesEnableString = CloudConfigurationManager.GetSetting("ResetQueueMessagesEnable");
			var resetBlobBlocksEnable = bool.Parse(resetBlobBlocksEnableString);
			var resetQueueMessagesEnable = bool.Parse(resetQueueMessagesEnableString);
			WindowsAzure.Start(connectionString, resetBlobBlocksEnable, resetQueueMessagesEnable);

            Trace.WriteLine("On start", "VisualBitcoin.StorageWorkerRole.Statistics Information");
            StatisticsCalculator.initialise();
            
			// Set the maximum number of concurrent connections 
			ServicePointManager.DefaultConnectionLimit = 12;
			
			return base.OnStart();
		}
	}
}
