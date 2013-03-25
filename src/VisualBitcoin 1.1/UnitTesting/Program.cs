using Microsoft.WindowsAzure.Storage.Blob;
using Storage;


namespace UnitTesting
{
    class Program
    {
        static void Main()
        {   
            // Test 1: Upload a blob to a container
            var storage = new WindowsAzureStorage();

            CloudBlobClient blobClient = storage.RetrieveBlobClient();
            storage.UploadContainer(blobClient, "test");
            storage.UploadBlob(blobClient, "test", "testfile", "C://Users/Public/test.txt");
        }
    }
}
