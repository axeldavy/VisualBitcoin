using System.Diagnostics;
using System.Net;
using System.Threading;
using Microsoft.WindowsAzure.ServiceRuntime;

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

			// Set the maximum number of concurrent connections 
			ServicePointManager.DefaultConnectionLimit = 12;
			
			return base.OnStart();
		}
	}
}
