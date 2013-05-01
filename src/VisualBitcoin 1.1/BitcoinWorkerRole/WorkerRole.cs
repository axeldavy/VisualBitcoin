using System.Diagnostics;
using System.Threading;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.ServiceRuntime;
using Storage;
using Storage.Models;
using System;

namespace BitcoinWorkerRole
{
	public class WorkerRole : RoleEntryPoint
	{
		// Flag to stop the run loop.
		private bool _isNotOnStop;
		private bool _isBitcoinClientConnexionEnable;
		
		public override void Run()
		{
			Trace.WriteLine("Entry point called", "VisualBitcoin.BitcoinWorkerRole.WorkerRole Information");

		    try
		    {

		        while (_isNotOnStop && _isBitcoinClientConnexionEnable)
		        {
		            Trace.WriteLine("Working", "VisualBitcoin.BitcoinWorkerRole.WorkerRole Information");

		            Thread.Sleep(5000);

		            BitcoinClient.UploadNewBlocks();
		        }

		    }

		    catch (Exception ex)
		    {
		        string err = ex.Message;
		        err = "Run (BitcoinWorkerRole) Error: " + err;
		        if (ex.InnerException != null)
		        {
		            err = err + " Inner Exception: " + ex.InnerException.Message;
		        }
		        Trace.TraceError(err);
		        Thread.Sleep(60000);// wait 60 seconds to prevent the workerRole to try again too soon
		    }
		}

		public override bool OnStart()
		{
			Trace.WriteLine("Start", "VisualBitcoin.BitcoinWorkerRole.WorkerRole Information");

			_isNotOnStop = true;
			_isBitcoinClientConnexionEnable = true;

		    try
		    {

		        // Storage configuration and start.
		        var connectionString = CloudConfigurationManager.GetSetting("StorageConnectionString");
		        WindowsAzure.Start(connectionString);

		        // Retrieve backup.
		        var bitcoinWorkerRoleBackup = Blob.DownloadBlockBlob<BitcoinWorkerRoleBackup>("bitcoinworkerrolebackup");

		        // Bitcoin client connexion configuration.
		        if (_isBitcoinClientConnexionEnable)
		        {
		            if (null == bitcoinWorkerRoleBackup)
		            {
		                BitcoinClient.Initialisation();
		            }
		            else
		            {
		                BitcoinClient.Initialisation(bitcoinWorkerRoleBackup.MaximumNumberOfBlocksInTheStorage,
		                                             bitcoinWorkerRoleBackup.NumberOfBlocksInTheStorage,
		                                             bitcoinWorkerRoleBackup.FirstBlockHash,
		                                             bitcoinWorkerRoleBackup.LastBlockHash);
		            }
		        }

		        return base.OnStart();
		    }
            catch (Exception ex)
            {
                string err = ex.Message;
                err = "OnStart (BitcoinWorkerRole) Error: " + err;
                if (ex.InnerException != null)
                {
                    err = err + " Inner Exception: " + ex.InnerException.Message;
                }
                Trace.TraceError(err);
                Thread.Sleep(60000);// wait 60 seconds to prevent the workerRole to try again too soon
                throw; // to prevent run to be called after the failure of OnStart
            }
		}

		public override void OnStop()
		{
			Trace.WriteLine("Stop", "VisualBitcoin.BitcoinWorkerRole.WorkerRole Information");

			_isNotOnStop = false;

			// Save backup.
			var backup = new BitcoinWorkerRoleBackup(BitcoinClient.MaximumNumberOfBlocksInTheStorage,
				BitcoinClient.NumberOfBlocksInTheStorage, BitcoinClient.FirstBlock.Hash,
				BitcoinClient.LastBlock.Hash);
			Blob.UploadBlockBlob("bitcoinworkerrolebackup", backup);
		}
	}
}
