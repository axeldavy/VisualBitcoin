using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Storage.Models;
using Storage;

namespace WebRole.Controllers
{
    public class StatisticsController : Controller
    {
        //
        // GET: /Statistics/
        private Blob blob;

        public ActionResult Index()
        {
            var connectionString = CloudConfigurationManager.GetSetting("StorageConnectionString");
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
            blob = new Blob(storageAccount);

            Statistics stat = (blob.GetStatistics<Statistics>("General_Statistics"));

            return View(stat);
        }

    }
}
