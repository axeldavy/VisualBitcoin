using System.Diagnostics;
using System.Net;
using System.Threading;
using Microsoft.WindowsAzure.ServiceRuntime;

namespace BitcoinWorkerRole
{
	public class WorkerRole : RoleEntryPoint
	{
		public override void Run()
		{
			Trace.WriteLine("Entry point called",
				"VisualBitcoin.BitcoinWorkerRole.WorkerRole Information");

			while (true)
			{
                BitcoinClient.UploadNewBlocks(5);

				Thread.Sleep(10000);
				Trace.WriteLine("Working",
					"VisualBitcoin.BitcoinWorkerRole.WorkerRole Information");
			}
		}

		public override bool OnStart()
		{
			Trace.WriteLine("On start",
				"VisualBitcoin.BitcoinWorkerRole.WorkerRole Information");

            BitcoinClient.Init();

			// Set the maximum number of concurrent connections 
			ServicePointManager.DefaultConnectionLimit = 12;
			
			return base.OnStart();
		}
	}
}
