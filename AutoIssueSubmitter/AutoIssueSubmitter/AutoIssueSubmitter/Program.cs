using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoIssueSubmitter
{
    class Program
    {
        static void Main(string[] args)
        {
            GithubAPI.Test();
            Console.ReadKey();
        }
    }
}
