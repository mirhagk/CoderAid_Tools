using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net;

namespace AutoIssueSubmitter
{
    public static class GithubAPI
    {
        private static void GetObjectFromURL<T>(string url)
        {
            var data = GetDataFromURL(url);
        }
        public static string GitAPIURL(string apiPath, object parameters=null)
        {
            var query = System.Web.HttpUtility.ParseQueryString(String.Empty);
            foreach (var param in parameters.GetType().GetProperties())
            {
                query.Add(param.Name, param.GetValue(parameters).ToString());
            }
            string queryVal = query.ToString();
            string url = "https://api.github.com" + apiPath + (String.IsNullOrWhiteSpace(queryVal) ? "" : ("?" + queryVal));
            return url;
        }
        public static void Test()
        {
            Console.WriteLine(GetDataFromURL(GitAPIURL("/search/repositories", 
                new {q= "mysql $_GET"}
                )));
        }
        private static string GetDataFromURL(string url)
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            request.AllowAutoRedirect = true;
            request.UserAgent = "HelperBot Issue Submitter -- contact mirhagk for problems";
            request.Credentials = Config.config.NetworkCredential;
            HttpWebResponse response;
            try
            {
                response = (HttpWebResponse)request.GetResponse();
            }
            catch (WebException ex)
            {
                response = (HttpWebResponse)ex.Response;
            }
            var reader = new System.IO.StreamReader(response.GetResponseStream());
            string result = reader.ReadToEnd();
            reader.Close();
            return result;
        }
    }
}
