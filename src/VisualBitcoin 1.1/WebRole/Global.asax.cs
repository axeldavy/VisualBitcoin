using System.Diagnostics;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Microsoft.WindowsAzure;
using Storage;
using WebRole.App_Start;

namespace WebRole
{
	public class MvcApplication : System.Web.HttpApplication
	{
		protected void Application_Start()
		{
			Trace.WriteLine("Application start",
				"VisualBitcoin.WebRole Information");
			
			AreaRegistration.RegisterAllAreas();

			WebApiConfig.Register(GlobalConfiguration.Configuration);
			FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
			RouteConfig.RegisterRoutes(RouteTable.Routes);
			BundleConfig.RegisterBundles(BundleTable.Bundles);
		}
	}
}