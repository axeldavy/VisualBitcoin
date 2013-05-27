using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Storage;
using Storage.Models;


namespace WebRole.Controllers
{
    public class TransactionsController : Controller
    {
        //
        // GET: /Transaction/
        private Blob blob;

        public ActionResult Index()
        {
            var connectionString = CloudConfigurationManager.GetSetting("StorageConnectionString");
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
            blob = new Blob(storageAccount);
            List<Transaction> t = blob.GetTransactionList(10);
            return View(t);
        }

    }
}
