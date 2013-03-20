using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Data
{

	public class Data
	{
        public WindowsAzureStorage blobStorage = new WindowsAzureStorage();

        public List<string> RetrieveBlobsList(string containerName) 
        {
            CloudBlobClient emulatedBlobClient = blobStorage.RetrieveEmulatedBlobClient();         
            return blobStorage.RetrieveBlobsList(emulatedBlobClient, containerName);
        }
	}

}
