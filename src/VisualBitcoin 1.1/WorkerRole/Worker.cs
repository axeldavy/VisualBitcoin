using System;
using System.Net;
using System.IO;
using Bitnet.Client;
using Newtonsoft.Json.Linq;

namespace WorkerRole
{
    public class Worker
    {
        private readonly BitnetClient _bitClient;

        public Worker(String path)
        {
            // Read credentials file
            var reader = new StreamReader(path);
            String credentials = reader.ReadToEnd();
            string[] netInfo = credentials.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            reader.Close();

            String user = netInfo[0];
            String password = netInfo[1];
            _bitClient = new BitnetClient("http://127.0.0.1:8332")
	            {
		            Credentials = new NetworkCredential(user, password)
	            };
        }

        public JObject GetInfo()
        {
            return _bitClient.GetInfo();
        }

    }
}
