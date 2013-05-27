using System.Diagnostics;
using System.Threading;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;
using Storage;
using Storage.Models;
using System;

namespace BitcoinWorkerRole
{
	public class WorkerRole : RoleEntryPoint
	{
		private bool _isStopRequested;
		private bool _isBitcoinClientConnexionEnable;
        private BitcoinClient bitcoinClient;
        
        private Blob blob;
        private Queue queue;

		public override void Run()
		{
			Trace.WriteLine("Entry point called", "VisualBitcoin.BitcoinWorkerRole.WorkerRole Information");

            try
			{
				while (_isStopRequested && _isBitcoinClientConnexionEnable)
				{
					Trace.WriteLine("Working", "VisualBitcoin.BitcoinWorkerRole.WorkerRole Information");
                    
					Thread.Sleep(5000);

					bitcoinClient.UploadNewBlocks();
				}
			}
			catch (Exception exception)
			{
				var message = "Run (BitcoinWorkerRole) Error: " + exception.Message;
				if (exception.InnerException != null)
					message = message + " Inner Exception: " + exception.InnerException.Message;
				Trace.TraceError(message);
				Thread.Sleep(60000); // wait 60 seconds to prevent the workerRole to try again too soon
			}
		}

		public override bool OnStart()
		{
			Trace.WriteLine("Start", "VisualBitcoin.BitcoinWorkerRole.WorkerRole Information");

			try
			{
                
				// Storage configuration and start.
				var connectionString = CloudConfigurationManager.GetSetting("StorageConnectionString");
				var resetBlobBlocksEnableString = CloudConfigurationManager.GetSetting("ResetBlobBlocksEnable");
				var resetQueueMessagesEnableString = CloudConfigurationManager.GetSetting("ResetQueueMessagesEnable");
				var isBitcoinClientConnexionEnableString = CloudConfigurationManager.GetSetting("IsBitcoinClientConnexionEnable");

				var resetBlobBlocksEnable = bool.Parse(resetBlobBlocksEnableString);
				var resetQueueMessagesEnable = bool.Parse(resetQueueMessagesEnableString);
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
                blob = new Blob(storageAccount);
                queue = new Queue(storageAccount.CreateCloudQueueClient());

				_isStopRequested = true;
				_isBitcoinClientConnexionEnable = bool.Parse(isBitcoinClientConnexionEnableString);

				// Retrieve backup.
				var bitcoinWorkerRoleBackup = blob.GetBackup();

				// Bitcoin client connexion configuration.
				if (null == bitcoinWorkerRoleBackup)
				{
					var firstBlockHash = CloudConfigurationManager.GetSetting("FirstBlockHash");
					bitcoinClient = new BitcoinClient(blob, queue, firstBlockHash);
				}
				else
					bitcoinClient = new BitcoinClient(blob, queue,
                                                      bitcoinWorkerRoleBackup.MaximumNumberOfBlocksInTheStorage,
												      bitcoinWorkerRoleBackup.NumberOfBlocksInTheStorage,
												      bitcoinWorkerRoleBackup.FirstBlockHash,
												      bitcoinWorkerRoleBackup.LastBlockHash,
                                                      bitcoinWorkerRoleBackup.MinimalHeight);

				return base.OnStart();
			}
			catch (Exception exception)
			{
				var message = "OnStart (BitcoinWorkerRole) Error: " + exception.Message;
				if (exception.InnerException != null)
					message = message + " Inner Exception: " + exception.InnerException.Message;
				Trace.TraceError(message);
				Thread.Sleep(60000); // wait 60 seconds to prevent the workerRole to try again too soon
				throw; // to prevent run to be called after the failure of OnStart
			}
		}

		public override void OnStop()
		{
			Trace.WriteLine("Stop", "VisualBitcoin.BitcoinWorkerRole.WorkerRole Information");

			_isStopRequested = false;

			// Save backup.
			var backup = new BitcoinWorkerRoleBackup(bitcoinClient.MaximumNumberOfBlocks,
													 bitcoinClient.NumberOfBlocks,
													 bitcoinClient.FirstBlock.Hash,
													 bitcoinClient.LastBlock.Hash,
                                                     bitcoinClient.FirstBlock.Height);
			blob.UploadBackup(backup);
		}
	}
}
