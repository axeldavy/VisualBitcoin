using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using Bitnet.Client;
using Newtonsoft.Json.Linq;

namespace WorkerRole
{
    public class Worker
    {
        private BitnetClient bitClient;

        public Worker(String path)
        {
            // Read credentials file
            StreamReader reader = new StreamReader(path);
            String credentials = reader.ReadToEnd();
            string[] netInfo = credentials.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
            reader.Close();

            String user = netInfo[0];
            String password = netInfo[1];
            this.bitClient = new BitnetClient("http://127.0.0.1:8332");
            this.bitClient.Credentials = new NetworkCredential(user, password);
        }

        public JObject GetInfo()
        {
            return this.bitClient.GetInfo();
        }

    }
}
