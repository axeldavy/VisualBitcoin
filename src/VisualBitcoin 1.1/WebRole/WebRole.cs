using System.Diagnostics;
using System.Threading;
using Microsoft.WindowsAzure.ServiceRuntime;

namespace WebRole
{
    public class WebRole : RoleEntryPoint
    {
        public override void Run()
        {
			Trace.WriteLine("Entry point called",
				"VisualBitcoin.WebRole.WebRole Information");

            while (true)
			{
                Thread.Sleep(10000);
				Trace.WriteLine("Working",
					"VisualBitcoin.WebRole.WebRole Information");
            }
        }

		public override bool OnStart()
		{
			Trace.WriteLine("On start",
				"VisualBitcoin.WebRole.WebRole Information");

			return base.OnStart();
		}
    }
}
