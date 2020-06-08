using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text;
using SierraMigrationToGitHub.Models.Github;
using System.Text.Json;
using System.Text.Encodings.Web;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace SierraMigrationToGitHub
{
    class GithubClient
    {
        private const string GithubApiUrl = "https://api.github.com/";
        private static GithubClient client;
        public static GithubClient GetClient(GithubConfigSetup githubOptions) => client ??= new GithubClient(githubOptions);
        private Dictionary<string, Issue> repositoryIssues;
        public async Task<Dictionary<string, Issue>> GetRepositoryIssues()
        {
            if (repositoryIssues != null) return repositoryIssues;
            var issuesResponse = await HttpClient.GetAsync(ApiIssuesUrl);
            //if (!issuesResponse.IsSuccessStatusCode) throw new Exception();
            var issuesResponseContentStream = await issuesResponse.Content.ReadAsStreamAsync();
            var issues = await JsonSerializer.DeserializeAsync<List<Issue>>(issuesResponseContentStream);
            repositoryIssues = issues.ToDictionary(i => i.title, issue => issue);
            return repositoryIssues;
        }
        private GithubConfigSetup githubConfigSetup { get; }
        private HttpClient _httpClient;
        private HttpClient HttpClient
        {
            get
            {
                if (_httpClient != null) return _httpClient;
                _httpClient = new HttpClient()
                {
                    BaseAddress = new Uri(GithubApiUrl)
                };
                SetupHttpClientHeadersForAuthorizationByToken();
                SetupHttpClientHeadersForJSONFormat();
                return _httpClient;
            }
        }
        private void SetupHttpClientHeadersForJSONFormat()
        {
            HttpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        }
        private void SetupHttpClientHeadersForAuthorizationByBase64()
        {
            HttpClient.DefaultRequestHeaders.UserAgent.Add(new System.Net.Http.Headers.ProductInfoHeaderValue("AppName", "1.0"));
            var authenticationString = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{githubConfigSetup.Account.UserName}:{githubConfigSetup.Account.Token}"));
            HttpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", authenticationString);
        }
        //SAML SSO
        private void SetupHttpClientHeadersForAuthorizationByToken()
        {
            HttpClient.DefaultRequestHeaders.UserAgent.Add(new System.Net.Http.Headers.ProductInfoHeaderValue("AppName", "1.0"));
            HttpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Token", githubConfigSetup.Account.Token);
        }
        public GithubClient(GithubConfigSetup githubOptions)
            => githubConfigSetup = githubOptions;
        public async Task<bool> CheckAuthorization()
            => (await HttpClient.GetAsync("user")).IsSuccessStatusCode;
        private string ApiIssuesUrl => $"repos/{githubConfigSetup.Account.UserName}/{githubConfigSetup.Project.Name}/issues";
        private string ApiCommentsUrl => $"{ApiIssuesUrl}/comments";
        private string GetApiIssueCommentsUrl(int issueNumber) => $"{ApiIssuesUrl}/{issueNumber}/comments";
        private JsonSerializerOptions GetUnsafeSerializerOptions => new JsonSerializerOptions
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            IgnoreNullValues = true
        };
        private async Task<Issue> CreateIssue(IssueCreatingModel issue)
        {
            var jsonIssue = JsonSerializer.Serialize(issue, GetUnsafeSerializerOptions);
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, ApiIssuesUrl)
            {
                Content = new JSONContent(jsonIssue)
            };
            var response = await HttpClient.SendAsync(requestMessage);
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<Issue>(content);
        }
        public async Task<Issue> UpdateIssue(int issueNumber, IssueUpsertModel issueModel)
        {
            var jsonIssue = JsonSerializer.Serialize(issueModel, GetUnsafeSerializerOptions);
            var requestMessage = new HttpRequestMessage(HttpMethod.Patch, $"{ApiIssuesUrl}/{issueNumber}")
            {
                Content = new JSONContent(jsonIssue)
            };
            var response = await HttpClient.SendAsync(requestMessage);
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<Issue>(content);
        }
        public async Task<Issue> CreateIssue(IssueUpsertModel issueCreatingFullApiModel)
        {
            var issue = await CreateIssue((IssueCreatingModel)issueCreatingFullApiModel);
            if (issueCreatingFullApiModel.state == GithubIssueStates.Closed)
            {
                var issueUpdatingModel = new IssueUpsertModel 
                {
                    state = issueCreatingFullApiModel.state
                };
                issue = await UpdateIssue(issue.number, issueUpdatingModel);
            }
            return issue;
        }
        public async Task<List<Issue>> CreateIssues(IEnumerable<IssueUpsertModel> issueCreatingFullApiModels)
        {
            var createIssueTasks = issueCreatingFullApiModels.Select(CreateIssue).ToArray();
            var issues = await Task.WhenAll(createIssueTasks);
            return issues.ToList();
        }
        public async Task<string> GetIssue(int issueId)
        {
            var response = await HttpClient.GetAsync($"{ApiIssuesUrl}/{issueId}");
            if (!response.IsSuccessStatusCode) return null;
            return await response.Content.ReadAsStringAsync();
        }
        public async Task<Comment> CreateComment(int issueNumber, CommentUpsertApiModel commentUpsertApiModel)
        {
            var jsonComment = JsonSerializer.Serialize(commentUpsertApiModel, GetUnsafeSerializerOptions);
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, $"{ApiIssuesUrl}/{issueNumber}/comments")
            {
                Content = new JSONContent(jsonComment)
            };
            var response = await HttpClient.SendAsync(requestMessage);
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<Comment>(content);
        }
        public async Task<List<Comment>> CreateComments(int issueNumber, IEnumerable<CommentUpsertApiModel> commentUpsertApiModel)
        {
            var createIssueTasks = commentUpsertApiModel.Select(c => CreateComment(issueNumber, c)).ToArray();
            var comments = await Task.WhenAll(createIssueTasks);
            return comments.ToList();
        }
        public async Task<List<Comment>> GetIssueComments(int issueNumber)
        {
            var issueCommentsResponse = await HttpClient.GetAsync(GetApiIssueCommentsUrl(issueNumber));
            //if (!issueCommentsResponse.IsSuccessStatusCode) return null;
            //if (!issuesResponse.IsSuccessStatusCode) throw new Exception();
            var issueCommentsResponseContentStream = await issueCommentsResponse.Content.ReadAsStreamAsync();
            var comments = await JsonSerializer.DeserializeAsync<List<Comment>>(issueCommentsResponseContentStream);
            return comments;
        }
        public async Task<Comment> UpdateComment(int commentId, int issueNumber, CommentUpsertApiModel commentUpsertApiModel)
        {
            var jsonComment = JsonSerializer.Serialize(commentUpsertApiModel, GetUnsafeSerializerOptions);
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, $"{ApiIssuesUrl}/{issueNumber}/comments/{commentId}")
            {
                Content = new JSONContent(jsonComment)
            };
            var response = await HttpClient.SendAsync(requestMessage);
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<Comment>(content);
        }
        public async Task<Issue> FindIssueByTitle(string issueTitle)
        {
            var issues = await GetRepositoryIssues();
            return issues.ContainsKey(issueTitle) 
                ? issues[issueTitle]
                : null;
        }
    }
}
