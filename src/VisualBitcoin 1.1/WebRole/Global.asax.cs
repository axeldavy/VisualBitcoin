using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using WebRole.App_Start;
using Storage;

namespace WebRole
{
	public class MvcApplication : System.Web.HttpApplication
	{
		protected void Application_Start()
		{
			AreaRegistration.RegisterAllAreas();

			WebApiConfig.Register(GlobalConfiguration.Configuration);
			FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
			RouteConfig.RegisterRoutes(RouteTable.Routes);
			BundleConfig.RegisterBundles(BundleTable.Bundles);

			// Check if all of the table, queue and blob containers used in this application
			// exist, and create any that don't already exist.
			var storage = new WindowsAzureStorage();
			storage.CreateIfNotExistsTableBlobsContainerQueue();
		}
	}
}