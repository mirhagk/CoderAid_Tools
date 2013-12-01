using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AutoIssueSubmitter
{
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
            var config = JsonConvert.DeserializeObject<Config>(System.IO.File.ReadAllText(filename));
            config.BlacklistRegex = config.Blacklist.Select((x) => new System.Text.RegularExpressions.Regex(x)).ToArray();
            return config;
        }
        public System.Net.NetworkCredential NetworkCredential
        {
            get
            {
                return new System.Net.NetworkCredential(credentials.Username, credentials.Password);
            }
        }
        public string[] Blacklist { get; set; }
        [JsonIgnore]
        public System.Text.RegularExpressions.Regex[] BlacklistRegex { get; set; }
        public Credentials credentials { get; set; }
    }
}
