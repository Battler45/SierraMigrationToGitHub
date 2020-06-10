using Microsoft.Extensions.Options;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text;
using System.Xml.Serialization;
using System.Text.Json;
using SierraMigrationToGitHub.Models.Unfuddle;
using System.Collections.Generic;
using System.IO;
using System.Reflection.Metadata.Ecma335;
using System.Web;
using SierraMigrationToGitHub.Models.Github;
using System.Linq;

namespace SierraMigrationToGitHub
{
    class UnfuddleClient
    {
        //public const string currentProject = "sierra";
        //public const string ticketUrl = "https://sierra.unfuddle.com/api/v1/projects/1/tickets/";
        //public const string UnfuddleApiUrl = "https://sierra.unfuddle.com/api/v1";
        private const string UnfuddleProjectApiUrl = "https://sierra.unfuddle.com/api/v1/projects/1/";
        public const string UnfuddlePeopleApiUrl = "https://sierra.unfuddle.com/api/v1/people";
        private Dictionary<int, Models.Unfuddle.User> projectPeople;
        public async Task<Dictionary<int, Models.Unfuddle.User>> GetProjectPeople()
        {
            if (projectPeople != null) return projectPeople;
            using var response = await NewHttpClient.GetAsync(UnfuddlePeopleApiUrl);
            if (!response.IsSuccessStatusCode) throw new Exception("");
            var contentStr = await response.Content.ReadAsStringAsync();
            var users = JsonSerializer.Deserialize<List<Models.Unfuddle.User>>(contentStr);
            projectPeople = users.ToDictionary(u => u.id, u => u);
            return projectPeople;
        }
        private const string FullTicketModelQuery = "comments=true&formatted=true&attachments=true";
        private static UnfuddleClient client;
        public static UnfuddleClient GetClient(UnfuddleConfigSetup unfuddleOptions) => client ??= new UnfuddleClient(unfuddleOptions);
        //curl -H "Authorization: token OAUTH-TOKEN" https://api.github.com
        private UnfuddleConfigSetup githubConfigSetup { get; }
        private HttpClient _httpClient;
        public HttpClient HttpClient
        {
            get
            {
                if (_httpClient != null) return _httpClient;
                _httpClient = new HttpClient()
                {
                    BaseAddress = new Uri(UnfuddleProjectApiUrl)
                };
                SetupHttpClientHeadersForAuthorizationByBase64(_httpClient);
                SetupHttpClientHeadersForJSONFormat(_httpClient);
                return _httpClient;
            }
        }
        public HttpClient NewHttpClient
        {
            get
            {
                //if (_httpClient != null) return _httpClient;
                var _httpClient = new HttpClient()
                {
                    BaseAddress = new Uri(UnfuddleProjectApiUrl)
                };
                SetupHttpClientHeadersForAuthorizationByBase64(_httpClient);
                SetupHttpClientHeadersForJSONFormat(_httpClient);
                return _httpClient;
            }
        }
        private void SetupHttpClientHeadersForXMLFormat(HttpClient httpClient)
        {
            httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/xml"));
        }
        private void SetupHttpClientHeadersForJSONFormat(HttpClient httpClient)
        {
            httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        }
        private void SetupHttpClientHeadersForAuthorizationByBase64(HttpClient httpClient)
        {
            var authenticationString = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{githubConfigSetup.Account.UserName}:{githubConfigSetup.Account.Password}"));
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", authenticationString);
        }
        public UnfuddleClient(UnfuddleConfigSetup githubOptions)
            => githubConfigSetup = githubOptions;
        public async Task<Ticket> GetTicket(int tiketId)
        {
            using var httpClient = NewHttpClient;
            using var response = await httpClient.GetAsync($"tickets/{tiketId}?{FullTicketModelQuery}");
            if (!response.IsSuccessStatusCode) return null;
            using var content = response.Content;
            var contentStr = await content.ReadAsStringAsync();
            var ticket = JsonSerializer.Deserialize<Ticket>(contentStr);

            if (ticket.description_formatted != null)
                ticket.description_formatted = HttpUtility.HtmlDecode(ticket.description_formatted);
            if (ticket.resolution_description_formatted != null)
                ticket.resolution_description_formatted = HttpUtility.HtmlDecode(ticket.resolution_description_formatted);
            return ticket;
        }
        /*
        public async Task<Ticket> AddAuthorAnnotationToTicketAndComments(Ticket ticket, Dictionary<int, Models.Unfuddle.User> people)
        {
            if (people == null) return null;
            if (ticket.assignee_id.HasValue && people.ContainsKey(ticket.assignee_id.Value))
                ticket.AppendToDescriptionAuthor(GetUserNamesAndLogin(people[ticket.assignee_id.Value]));
            foreach (var comment in ticket.comments)
                if (people.ContainsKey(comment.author_id))
                    comment.AppendToBodyAuthor(GetUserNamesAndLogin(people[comment.author_id]));
            return ticket;
            static string GetUserNamesAndLogin(Models.Unfuddle.User user) => $"{user.last_name} {user.first_name}(unfuddle username: {user.username})";
        }*/

        //?limit=500&page=1
        public async Task<List<Ticket>> GetTicketsPage(int page = 1, int limit = 500)
        {
            var contentStr = await GetTicketsPageContent(page, limit);// ?? throw new Exception();
            if (contentStr == null) return null;
            var pageTickets = JsonSerializer.Deserialize<List<Ticket>>(contentStr);
            return pageTickets;
        }
        public async Task<List<Ticket>> GetTicketsFromNumber(int beginTicketNumber)
        {
            throw new NotImplementedException();
            /*
            var contentStr = await GetTicketsPageContent(page, limit);// ?? throw new Exception();
            if (contentStr == null) return null;
            var pageTickets = JsonSerializer.Deserialize<List<Ticket>>(contentStr);
            return pageTickets;
            */
        }
        public async Task<List<Ticket>> GetTickets()
        {
            var tickets = new List<Ticket>();
            var page = 1;
            while (true)
            {
                var nextpageTickets = await GetTicketsPage(page);
                if (nextpageTickets == null) return null;
                if (nextpageTickets.Count == 0) return tickets;
                tickets.AddRange(nextpageTickets);
                ++page;
            }
        }
        private async Task<string> GetTicketsPageContent(int page = 1, int limit = 500)
        {
            using var httpClient = NewHttpClient;
            using var response = await HttpClient.GetAsync($"tickets?{nameof(limit)}={limit}&{nameof(page)}={page}&{FullTicketModelQuery}");
            if (!response.IsSuccessStatusCode) return null;
            using var content = response.Content;
            var contentStr = await content.ReadAsStringAsync();
            return contentStr;
        }
        private async Task<string> GetTicketsJSON()
        {
            const string JSONEmptyArray = "[]";
            var ticketsJSON = JSONEmptyArray;
            var page = 1;
            while (true)
            {
                var nextpageTickets = await GetTicketsPageContent(page);
                if (nextpageTickets == null) return null;
                if (nextpageTickets == JSONEmptyArray) return ticketsJSON;
                ticketsJSON = ticketsJSON == JSONEmptyArray
                    ? nextpageTickets
                    : $"[{ticketsJSON[1..^1]}, {nextpageTickets[1..^1]}]";
                ++page;
            }
        }
        public async Task WriteAllTicketsIntoFile(string filePath)
        {
            var tickets = await GetTicketsJSON();
            await File.WriteAllTextAsync(filePath, tickets);
        }

        //https://sierra.unfuddle.com/api/v1/projects/1/tickets/38/attachments/372/download
        public async Task<string> DownloadTicketFile(int ticketId, int attacmentId, string fileFolder, string fileName)
        {
            try
            {
                using var httpClient = NewHttpClient;
                using var response = await httpClient.GetAsync($"tickets/{ticketId}/attachments/{attacmentId}/download");
                if (!response.IsSuccessStatusCode) return null;
                using var contentStream = await response.Content.ReadAsStreamAsync();
                var filePath = await FileService.SaveFileAsync(fileFolder, fileName, contentStream);
                return filePath;

            }
            catch (Exception e)
            {

                return null;
            }
        }

        //api/v1/projects/{id}/tickets/{id}/comments/{id}/attachments/{id}/download[GET]
        public async Task<string> DownloadCommentFile(int ticketId, int commentId, int attacmentId, string fileFolder, string fileName)
        {

            try
            {
                using var httpClient = NewHttpClient;
                using var response = await httpClient.GetAsync($"tickets/{ticketId}/comments/{commentId}/attachments/{attacmentId}/download");
                if (!response.IsSuccessStatusCode) return null;
                using var contentStream = await response.Content.ReadAsStreamAsync();
                var filePath = await FileService.SaveFileAsync(fileFolder, fileName, contentStream);
                return filePath;
            }
            catch (Exception e)
            {

                return null;
            }
        }
        private FileService FileService { get; } = new FileService();

    }
}
