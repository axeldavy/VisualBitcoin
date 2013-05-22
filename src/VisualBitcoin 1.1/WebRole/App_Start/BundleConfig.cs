using System.Web;
using System.Web.Optimization;

namespace WebRole
{
	public class BundleConfig
	{
		public static void RegisterBundles(BundleCollection bundles)
		{
			bundles.Add(new ScriptBundle("~/bundles/js").Include("~/Scripts/jquery-{version}.js", "~/Content/bootstrap/js/bootstrap.js"));

			// "~/Scripts/jquery-ui-{version}.js"));
			// "~/Scripts/jquery.unobtrusive*",
			// "~/Scripts/jquery.validate*"));
			// "~/Scripts/modernizr-*"));

			bundles.Add(new StyleBundle("~/bundles/css").Include("~/Content/bootstrap/css/bootstrap.css", "~/Content/style.css"));
		}
	}
}