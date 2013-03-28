using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Table;

namespace Storage
{
    public interface IStorage
    {
        CloudBlobClient RetrieveBlobClient();
        CloudTableClient RetrieveTableClient();
        CloudQueueClient RetrieveQueueClient();
        void UploadContainer(CloudBlobClient blobClient, string containerName);
        void UploadBlob(CloudBlobClient blobClient, string containerName, string blobName, string fileName);
        void DownloadBlobToFile(CloudBlobClient blobClient, string containerName, string blobName, string destFileName);
        String DownloadBlobToString(string blobName);
        void DeleteBlob(CloudBlobClient blobClient, string containerName, string blobName);
        void DeleteContainer(CloudBlobClient blobClient, string containerName);
        CloudBlobContainer RetrieveContainer(CloudBlobClient blobClient, string containerName);
    }

    public class WindowsAzureStorage : IStorage
    {
        private static CloudStorageAccount _storageAccount;
        private static CloudBlobClient _blobClient;
        private static CloudTableClient _tableClient;
        private static CloudQueueClient _queueClient;
        private static CloudBlobContainer _containerReference;
        private static CloudTable _tableReference;
        private static CloudQueue _queueReference;

        // Configure and start the storage, only one call made on application start.
        public static void Start(bool useDevelopmentStorage, string connectionString, string containerName, string tableName, string queueName)
        {
            if (null != _storageAccount)
                throw new Exception("Windows Azure Storage can not be initialize twice.");

            _storageAccount = useDevelopmentStorage ? CloudStorageAccount.DevelopmentStorageAccount : CloudStorageAccount.Parse(connectionString);

            _blobClient = _storageAccount.CreateCloudBlobClient();
            _containerReference = _blobClient.GetContainerReference(containerName);
            _containerReference.CreateIfNotExists();

            _tableClient = _storageAccount.CreateCloudTableClient();
            _tableReference = _tableClient.GetTableReference(tableName);
            _tableReference.CreateIfNotExists();

            _queueClient = _storageAccount.CreateCloudQueueClient();
            _queueReference = _queueClient.GetQueueReference(queueName);
            _queueReference.CreateIfNotExists();
        }

        // Retriving methods.
        public CloudBlobClient RetrieveBlobClient()
        {
            return _storageAccount.CreateCloudBlobClient();
        }

        public CloudTableClient RetrieveTableClient()
        {
            return _storageAccount.CreateCloudTableClient();
        }

        public CloudQueueClient RetrieveQueueClient()
        {
            return _storageAccount.CreateCloudQueueClient();
        }

        public static CloudTable GetTableReference()
        {
            return _tableReference;
        }

        public void UploadContainer(CloudBlobClient blobClient, string containerName)
        {
            // Variables for the cloud storage objects.
            CloudBlobContainer blobContainer;

            // Get the container reference.
            blobContainer = blobClient.GetContainerReference(containerName);

            // Create the container if it does not exist.
            blobContainer.CreateIfNotExists();

            Console.WriteLine("Upload complete: container \"" + containerName + "\"");
        }

        public void UploadBlob(CloudBlobClient blobClient, string containerName, string blobName, string fileName)
        {
            CloudBlobContainer blobContainer;
            CloudBlockBlob blockBlob;

            // Retrieve reference to a container.
            blobContainer = blobClient.GetContainerReference(containerName);

            // Retrieve reference to a blob.
            blockBlob = blobContainer.GetBlockBlobReference(blobName);

            // Create or overwrite the blob with contents from a local file.
            using (var fileStream = System.IO.File.OpenRead(@fileName))
            {
                blockBlob.UploadFromStream(fileStream);
            }

            Console.WriteLine("Upload Complete: \"" + blobName + "\" in container \"" + containerName + "\" from file \"" + fileName + "\"");
        }

        public void DownloadBlobToFile(CloudBlobClient blobClient, string containerName, string blobName, string destFileName)
        {
            CloudBlobContainer blobContainer;

            // Retrieve reference to a container.
            blobContainer = blobClient.GetContainerReference(containerName);

            // Retrieve reference to blobName.
            CloudBlockBlob blockBlob = blobContainer.GetBlockBlobReference(blobName);

            // Save blob contents to a file.
            using (var fileStream = System.IO.File.OpenWrite(@destFileName))
            {
                blockBlob.DownloadToStream(fileStream);
            }
        }

        public String DownloadBlobToString(string blobName)
        {
            // Retrieve reference to a blob.
            CloudBlockBlob blockBlob = _containerReference.GetBlockBlobReference(blobName);

            string text;
            using (var memoryStream = new MemoryStream())
            {
                blockBlob.DownloadToStream(memoryStream);
                text = System.Text.Encoding.UTF8.GetString(memoryStream.ToArray());
            }
            return text;
        }

        public static Block FromXml(string xml)
        {
            var xmlSerializer = new XmlSerializer(typeof(Block));
            var stringReader = new StringReader(xml);
            var xmlReader = new XmlTextReader(stringReader);
            var block = xmlSerializer.Deserialize(xmlReader) as Block;
            xmlReader.Close();
            stringReader.Close();
            return block;
        }

        public static Block GetExampleBlock()
        {
            var storage = new WindowsAzureStorage();
            var blob = storage.DownloadBlobToString("block");
            var block = FromXml(blob);
            return block;
        }

        public void DeleteBlob(CloudBlobClient blobClient, string containerName, string blobName)
        {
            // Retrieve reference to a previously created container.
            CloudBlobContainer blobContainer = blobClient.GetContainerReference(containerName);

            // Retrieve reference to a blob.
            CloudBlockBlob blockBlob = blobContainer.GetBlockBlobReference(blobName);

            // Delete the blob.
            blockBlob.Delete();
        }

        public void DeleteContainer(CloudBlobClient blobClient, string containerName)
        {
            // Retrieve reference to a previously created container.
            CloudBlobContainer blobContainer = blobClient.GetContainerReference(containerName);

            // Delete the container
            blobContainer.Delete();
        }

        public CloudBlobContainer RetrieveContainer(CloudBlobClient blobClient, string containerName)
        {
            // Retrieve reference to a previously created container.
            return blobClient.GetContainerReference(containerName);
        }

        public List<string> RetrieveBlobsList(CloudBlobClient blobClient, string containerName)
        {
            // Retrieve reference to a previously created container.
            CloudBlobContainer blobContainer = blobClient.GetContainerReference(containerName);

            var blobsList = new List<string>();

            // Loop over blobs within the container and output the URI to each of them.
            foreach (var blobItem in blobContainer.ListBlobs())
            {
                char[] separator = new char[] { '/' };
                string blobUri = blobItem.Uri.ToString();
                string[] splitedBlobUri = blobUri.Split(separator);
                string blobName = splitedBlobUri[splitedBlobUri.Length - 1];
                CloudBlockBlob blockBlob = blobContainer.GetBlockBlobReference(blobName);
                string text;
                using (var memoryStream = new MemoryStream())
                {
                    blockBlob.DownloadToStream(memoryStream);
                    text = System.Text.Encoding.UTF8.GetString(memoryStream.ToArray());

                }
                blobsList.Add(text);
            }

            return blobsList;
        }
    }
}
