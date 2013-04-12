using System.Diagnostics;
using System.Threading;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.ServiceRuntime;
using Storage;

namespace BitcoinWorkerRole
{
	public class WorkerRole : RoleEntryPoint
	{
		public override void Run()
		{
			Trace.WriteLine("Entry point called", "VisualBitcoin.BitcoinWorkerRole.WorkerRole Information");

			while (true)
			{
                // BitcoinClient.UploadNewBlocks(5);

				Thread.Sleep(10000);
				Trace.WriteLine("Working", "VisualBitcoin.BitcoinWorkerRole.WorkerRole Information");
			}
		}

		public override bool OnStart()
		{
			Trace.WriteLine("Start", "VisualBitcoin.BitcoinWorkerRole.WorkerRole Information");

			// Storage configuration and start.
			var connectionString = CloudConfigurationManager.GetSetting("StorageConnectionString");
			WindowsAzure.Start(connectionString);

			// Bitcoin connexion configuration.
            // TODO: BitcoinClient.Init();

			return base.OnStart();
		}
	}
}
