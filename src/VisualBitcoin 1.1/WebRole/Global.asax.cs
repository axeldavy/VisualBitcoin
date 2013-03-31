using System.Configuration;
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

			// Storage configuration and start.
			var useDevelopmentStorage = bool.Parse(ConfigurationManager.AppSettings["useDevelopmentStorage"]);
			var connectionString = ConfigurationManager.ConnectionStrings["StorageConnectionString"].ConnectionString;
			var containerName = ConfigurationManager.AppSettings["Container"];
			var tableName = ConfigurationManager.AppSettings["Table"];
			var queueName = ConfigurationManager.AppSettings["Queue"];
			WindowsAzure.Start(useDevelopmentStorage, connectionString, containerName, tableName, queueName);
		}
	}
}