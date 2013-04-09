using System;
using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Configuration;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.ServiceRuntime;
using Storage;

namespace StorageWorkerRole
{
	public class WorkerRole : RoleEntryPoint
	{
		public override void Run()
		{
			Trace.WriteLine("Entry point called",
				"VisualBitcoin.StorageWorkerRole.WorkerRole Information");
            
            // TODO: get configuration settings from cscfg file
            bool useDevelopmentStorage = Boolean.Parse(RoleEnvironment.GetConfigurationSettingValue("UseDevelopmentStorage"));
            string connectionString = RoleEnvironment.GetConfigurationSettingValue("StorageConnectionString");
            string containerName = RoleEnvironment.GetConfigurationSettingValue("Container");
            string tableName = RoleEnvironment.GetConfigurationSettingValue("Table");
            string queueName = RoleEnvironment.GetConfigurationSettingValue("Queue");

            WindowsAzure.Start(useDevelopmentStorage, connectionString, containerName, tableName, queueName);            

			while (true)
			{
				// TODO: check queue and process data.

				Thread.Sleep(10000);
				Trace.WriteLine("Working",
					"VisualBitcoin.StorageWorkerRole.WorkerRole Information");
			}
		}

		public override bool OnStart()
		{
			Trace.WriteLine("On start",
				"VisualBitcoin.StorageWorkerRole.WorkerRole Information");

			// TODO: Configure the StorageWorkerRole.
            // TODO: needed three BlobContainers: for brute blocks, for clear blocks, for transactions

			// Set the maximum number of concurrent connections 
			ServicePointManager.DefaultConnectionLimit = 12;
			
			return base.OnStart();
		}
	}
}
