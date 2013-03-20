using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using Storage;

namespace Data
{

	public class Data
	{
        public List<string> RetrieveBlobsList(string containerName) 
        { 
            WindowsAzureStorage blobStorage = new WindowsAzureStorage();           
            return blobStorage.RetrieveBlobsList(containerName);
        }
	}

}
