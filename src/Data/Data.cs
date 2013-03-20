using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using Storage;

namespace Data
{

	public class Data
	{
        public int size;
        public decimal total;
        public string relayedby;
        public int transactions;
        public string hash;

        public Data() 
        {
            this.size = 0;
            this.total = 0;
            this.relayedby = "";
            this.transactions = 0;
            this.hash = "";
        }

        public Data(int size, decimal total, string relayedby, int transactions, string hash)
        {
            this.size = size;
            this.total = total;
            this.relayedby = relayedby;
            this.transactions = transactions;
            this.hash = hash;
        }

        public string XMLSerialize()
        {
            XmlSerializer x = new XmlSerializer(this.GetType());
            StringWriter writer = new StringWriter();
            x.Serialize(writer, this);

            return writer.ToString();
        }

        public List<string> RetrieveBlobsList(string containerName) 
        { 
            WindowsAzureStorage blobStorage = new WindowsAzureStorage();           
            return blobStorage.RetrieveBlobsList(containerName);
        }
	}
}
