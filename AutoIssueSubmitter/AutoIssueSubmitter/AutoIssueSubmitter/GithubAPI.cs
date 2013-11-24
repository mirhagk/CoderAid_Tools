using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net;
using CsQuery;

namespace AutoIssueSubmitter
{
    public static class GithubAPI
    {
        public class Repository
        {

        }
        private static T GetObjectFromURL<T>(string url)
        {
            var data = GetDataFromURL(url);
            return JsonConvert.DeserializeObject<T>(data);
        }
        public static string GitAPIURL(string apiPath, object parameters = null)
        {
            string queryVal = ObjectToQuery(parameters ?? new object());
            string url = "https://api.github.com" + apiPath + (String.IsNullOrWhiteSpace(queryVal) ? "" : ("?" + queryVal));
            return url;
        }
        private static string ObjectToQuery(object parameters)
        {
            var query = System.Web.HttpUtility.ParseQueryString(String.Empty);
            if (parameters != null)
                foreach (var param in parameters.GetType().GetProperties())
                {
                    query.Add(param.Name, param.GetValue(parameters).ToString());
                }
            return query.ToString();
        }
        private static void SearchQuery(string searchWords)
        {
            //https://github.com/search?q=css+background-colour&type=Code&ref=searchresults
            var url = "https://github.com/search?" +
                ObjectToQuery(new
                {
                    q = searchWords,
                    type = "Code",
                    o = "desc",
                    s = "indexed"
                });
            var data = GetDataFromURL(url);
            CQ cq = data;
            var thing = cq[".code-list-item"].ToList();
            Console.WriteLine(thing.First().InnerHTML);
            //Console.WriteLine(cq[".code-list-item"].Skip(1).First().InnerHTML);
        }
        public static void Test()
        {
            //Console.WriteLine(GetDataFromURL(GitAPIURL("/search/code", 
            //new {q= "mysql $_GET"}
            //)));
            SearchQuery("mysql_query $_POST");
            Console.WriteLine("Done");
        }
        private static T Request<T>(string apiPath, object parameters = null)
        {
            return GetObjectFromURL<T>(GitAPIURL(apiPath, parameters));
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
