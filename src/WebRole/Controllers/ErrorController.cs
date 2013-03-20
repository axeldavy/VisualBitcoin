using System.Web.Mvc;

namespace WebRole.Controllers
{
    public class ErrorController : Controller
    {
        //
        // GET: /Error/

        public ActionResult NotFound()
        {
            Response.TrySkipIisCustomErrors = true;
            Response.Status = "404 Not Found";
            Response.StatusCode = 404;
            return View();
        }
    }
}