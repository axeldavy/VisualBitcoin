using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using Storage;


namespace UnitTesting
{
    class Program
    {
        static void Main(string[] args)
        {
            


            var useless = new WindowsAzureStorage();

            CloudBlobClient blobClient = useless.RetrieveBlobClient();
            useless.UploadContainer(blobClient, "test");
            useless.UploadBlob(blobClient, "test", "testfile", "C://Users/Public/test.txt");
            
        }
    }

}
