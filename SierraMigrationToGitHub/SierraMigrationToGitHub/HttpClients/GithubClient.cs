using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text;
using SierraMigrationToGitHub.Models.Github;
using System.Text.Json;
using System.Text.Encodings.Web;

namespace SierraMigrationToGitHub
{
    class GithubClient
    {
        private const string GithubApiUrl = "https://api.github.com/";
        private static GithubClient client;
        public static GithubClient GetClient(GithubConfigSetup githubOptions) => client ??= new GithubClient(githubOptions);
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
    }
}
