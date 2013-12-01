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
        public class CandidateRepo
        {
            public string URL;
            public DateTime LastUpdate;
            public string Owner;
            public override string ToString()
            {
                return String.Format("Repo:\nURL: {0}\nOwner: {1}\nLast Updated: {2}", URL, Owner, LastUpdate);
            }
        }
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
        private static List<CandidateRepo> FindCandidateRepos(string searchWords, int page =1)
        {
            List<CandidateRepo> candidates = new List<CandidateRepo>();
            //https://github.com/search?q=css+background-colour&type=Code&ref=searchresults
            var url = "https://github.com/search?" +
                ObjectToQuery(new
                {
                    q = searchWords,
                    type = "Code",
                    o = "desc",
                    s = "indexed",
                    p = page
                    
                });
            var data = GetDataFromURL(url);
            CQ cq = data;
            var titles = cq[".code-list-item .title"].ToList();
            foreach (var title in titles)
            {
                CQ section = title.InnerHTML;
                CandidateRepo candidate = new CandidateRepo()
                {
                    Owner = section["a"].Skip(0).First()["href"],
                    URL = section["a"].Skip(1).First()["href"],
                    LastUpdate = DateTime.Parse(section["time"].Attr("datetime"))
                };
                candidates.Add(candidate);
            }
            return candidates;
        }
        private static bool IsOnBlackList(CandidateRepo repo)
        {
            foreach (var black in Config.config.BlacklistRegex)
            {
                if (black.Match(repo.URL).Length == repo.URL.Length)
                    return true;
            }
            return false;
        }
        private const int SearchDelayMilli = 1000;
        private static List<CandidateRepo> FindAllReposSince(string searchWords, DateTime sinceDate)
        {
            bool done = false;
            int page = 1;
            List<CandidateRepo> candidates = new List<CandidateRepo>();
            while (!done)
            {
                var searched = FindCandidateRepos(searchWords, page++);
                foreach (var repo in searched)
                {
                    if (repo.LastUpdate < sinceDate)
                    {
                        done = true;
                        break;
                    }
                    if (!IsOnBlackList(repo))
                        candidates.Add(repo);
                }
                Console.WriteLine("Searching page {0}, earliest found {1}, total found {2}", page, candidates.Last().LastUpdate,candidates.Count);
                if (done)
                    break;
                System.Threading.Thread.Sleep(SearchDelayMilli);
            }
            return candidates;
        }
        public static void Test()
        {
            //Console.WriteLine(GetDataFromURL(GitAPIURL("/search/code", 
            //new {q= "mysql $_GET"}
            //)));
            foreach (var repo in FindAllReposSince("mysql_query $_GET",DateTime.Today))
            {
                Console.WriteLine(repo);
            }
            //FindCandidateRepos("mysql_query $_GET");
            Console.WriteLine("Done");
        }
        private static T Request<T>(string apiPath, object parameters = null)
        {
            return GetObjectFromURL<T>(GitAPIURL(apiPath, parameters));
        }
        private static CookieCollection cookies = null;
        private static string GetDataFromURL(string url)
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            request.AllowAutoRedirect = true;
            request.UserAgent = "HelperBot Issue Submitter -- contact mirhagk for problems";
            request.Credentials = Config.config.NetworkCredential;
            if (cookies != null)
                request.CookieContainer.Add(cookies);
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
