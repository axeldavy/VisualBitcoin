using System;
using System.Collections.Generic;
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
            // Test 1: Upload a blob to a container
            WindowsAzureStorage storage = new WindowsAzureStorage();

            CloudBlobClient blobClient = storage.RetrieveBlobClient();
            storage.UploadContainer(blobClient, "test");
            storage.UploadBlob(blobClient, "test", "testfile", "C://Users/Public/test.txt");
            

        }
    }

}
