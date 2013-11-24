using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AutoIssueSubmitter
{
    [Serializable]
    class Config
    {
        public class Credentials
        {
            public string Username { get; set; }
            public string Password { get; set; }
        }
        static public Config config;
        static Config()
        {
            config = LoadConfig("config.json");
        }
        public static Config LoadConfig(string filename)
        {
            return JsonConvert.DeserializeObject<Config>(System.IO.File.ReadAllText(filename));
        }
        public System.Net.NetworkCredential NetworkCredential
        {
            get
            {
                return new System.Net.NetworkCredential(credentials.Username, credentials.Password);
            }
        }
        public Credentials credentials { get; set; }
    }
}
