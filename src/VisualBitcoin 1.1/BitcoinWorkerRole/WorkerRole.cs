using System.Diagnostics;
using System.Threading;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.ServiceRuntime;
using Storage;
using Storage.Models;

namespace BitcoinWorkerRole
{
	public class WorkerRole : RoleEntryPoint
	{
		// Flag to stop the run loop.
		private bool _isNotOnStop;
		
		public override void Run()
		{
			Trace.WriteLine("Entry point called", "VisualBitcoin.BitcoinWorkerRole.WorkerRole Information");

			while (_isNotOnStop)
			{
				Trace.WriteLine("Working", "VisualBitcoin.BitcoinWorkerRole.WorkerRole Information");

				Thread.Sleep(5000);

                BitcoinClient.UploadNewBlocks();
			}
		}

		public override bool OnStart()
		{
			Trace.WriteLine("Start", "VisualBitcoin.BitcoinWorkerRole.WorkerRole Information");

			_isNotOnStop = true;

			// Storage configuration and start.
			var connectionString = CloudConfigurationManager.GetSetting("StorageConnectionString");
			WindowsAzure.Start(connectionString);

			// Retrieve backup.
			var bitcoinWorkerRoleBackup = Blob.DownloadBlockBlob<BitcoinWorkerRoleBackup>("bitcoinworkerrolebackup");

			// Bitcoin client connexion configuration.
			if (null == bitcoinWorkerRoleBackup)
			{
				BitcoinClient.Initialisation();
			}
			else
			{
				BitcoinClient.Initialisation(bitcoinWorkerRoleBackup.NumberOfBlocksInTheStorage,
					bitcoinWorkerRoleBackup.FirstBlockHash, bitcoinWorkerRoleBackup.LastBlockHash);
			}

			return base.OnStart();
		}

		public override void OnStop()
		{
			Trace.WriteLine("Stop", "VisualBitcoin.BitcoinWorkerRole.WorkerRole Information");

			_isNotOnStop = false;

			// Save backup.
			var backup = new BitcoinWorkerRoleBackup(BitcoinClient.NumberOfBlocksInTheStorage, BitcoinClient.FirstBlock.Hash, BitcoinClient.LastBlock.Hash);
			Blob.UploadBlockBlob("bitcoinworkerrolebackup", backup);
		}
	}
}
