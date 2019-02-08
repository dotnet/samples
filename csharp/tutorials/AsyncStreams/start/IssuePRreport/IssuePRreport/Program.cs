using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GitHubActivityReport
{
    public class GraphQLRequest
    {
        public string query { get; set; }
        public IDictionary<string, object> variables { get; } = new Dictionary<string, object>();

        public string ToJsonText() =>
            JsonConvert.SerializeObject(this);
    }

    class Program
    {
        private const string PagedIssueQuery =
@"query ($repo_name: String!,  $start_cursor:String) {
  repository(owner: ""dotnet"", name: $repo_name) {
    issues(last: 25, before: $start_cursor)
     {
        totalCount
        pageInfo {
          hasPreviousPage
          startCursor
        }
        nodes {
          title
          number
          createdAt
        }
      }
    }
  }
";
        private class progressStatus : IProgress<int>
        {
            Action<int> action;
            public progressStatus(Action<int> progressAction) => 
                action = progressAction;

            public void Report(int value) => action(value);
        }
        static async Task Main(string[] args)
        {
            var key = GetEnvVariable("GitHubKey",
            "You must store you GitHub key in the 'GitHubKey' environment variable",
            "");

            var client = new GitHubClient(new Octokit.ProductHeaderValue("IssueQueryDemo"))
            {
                Credentials = new Octokit.Credentials(key)
            };

            var progressReporter = new progressStatus((num) =>
            {
                Console.WriteLine($"Received {num} issues in total");
            });
            CancellationTokenSource cancellationSource = new CancellationTokenSource();

            try
            {
                var results = await runPagedQuery(client, PagedIssueQuery, "docs",
                    cancellationSource.Token, progressReporter);
                foreach(var issue in results)
                    Console.WriteLine(issue);
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Work has been cancelled");
            }
        }

        private static async Task<JArray> runPagedQuery(GitHubClient client, string queryText, string repoName, CancellationToken cancel, IProgress<int> progress)
        {
            var issueAndPRQuery = new GraphQLRequest
            {
                query = queryText
            };
            issueAndPRQuery.variables["repo_name"] = repoName;

            JArray finalResults = new JArray();
            bool hasMorePages = true;
            int pagesReturned = 0;
            int issuesReturned = 0;

            // Stop with 10 pages, because these are big repos:
            while (hasMorePages && (pagesReturned++ < 10))
            {
                var postBody = issueAndPRQuery.ToJsonText();
                var response = await client.Connection.Post<string>(new Uri("https://api.github.com/graphql"),
                    postBody, "application/json", "application/json");

                JObject results = JObject.Parse(response.HttpResponse.Body.ToString());

                int totalCount = (int)issues(results)["totalCount"];
                hasMorePages = (bool)pageInfo(results)["hasPreviousPage"];
                issueAndPRQuery.variables["start_cursor"] = pageInfo(results)["startCursor"].ToString();
                finalResults.Merge(issues(results)["nodes"]);
                issuesReturned += issues(results)["nodes"].Count();
                progress?.Report(issuesReturned);
                cancel.ThrowIfCancellationRequested();
            }
            return finalResults;

            JObject issues(JObject result) => (JObject)result["data"]["repository"]["issues"];
            JObject pageInfo(JObject result) => (JObject)issues(result)["pageInfo"];
        }


        private static string GetEnvVariable(string item, string error, string defaultValue)
        {
            var value = Environment.GetEnvironmentVariable(item);
            if (string.IsNullOrWhiteSpace(value))
            {
                if (!string.IsNullOrWhiteSpace(error))
                {
                    Console.WriteLine(error);
                    Environment.Exit(0);
                }

                if (!string.IsNullOrWhiteSpace(defaultValue))
                {
                    return defaultValue;
                }
            }
            return value;
        }
    }
}
