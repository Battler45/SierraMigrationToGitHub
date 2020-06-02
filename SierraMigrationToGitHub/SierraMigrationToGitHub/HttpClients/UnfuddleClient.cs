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

namespace SierraMigrationToGitHub
{
    class UnfuddleClient
    {
        //public const string currentProject = "sierra";
        //public const string ticketUrl = "https://sierra.unfuddle.com/api/v1/projects/1/tickets/";
        private const string UnfuddleProjectApiUrl = "https://sierra.unfuddle.com/api/v1/projects/1/";
        //public const string UnfuddleApiUrl = "https://sierra.unfuddle.com/api/v1";
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
                SetupHttpClientHeadersForAuthorizationByBase64();
                SetupHttpClientHeadersForJSONFormat();
                return _httpClient;
            }
        }
        private void SetupHttpClientHeadersForXMLFormat()
        {
            HttpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/xml"));
        }
        private void SetupHttpClientHeadersForJSONFormat()
        {
            HttpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        }
        private void SetupHttpClientHeadersForAuthorizationByBase64()
        {
            var authenticationString = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{githubConfigSetup.Account.UserName}:{githubConfigSetup.Account.Password}"));
            HttpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", authenticationString);
        }
        public UnfuddleClient(UnfuddleConfigSetup githubOptions)
            => githubConfigSetup = githubOptions;
        public async Task<Ticket> GetTicket(int tiketId)
        {
            var response = await HttpClient.GetAsync($"tickets/{tiketId}?{FullTicketModelQuery}");
            if (!response.IsSuccessStatusCode) return null;
            var content = response.Content;
            var contentStr = await content.ReadAsStringAsync();
            var ticket = JsonSerializer.Deserialize<Ticket>(contentStr);

            if (ticket.description_formatted != null)
                ticket.description_formatted = HttpUtility.HtmlDecode(ticket.description_formatted);
            if (ticket.resolution_description_formatted != null)
                ticket.resolution_description_formatted = HttpUtility.HtmlDecode(ticket.resolution_description_formatted);
            return ticket;
        }
        //?limit=500&page=1
        public async Task<List<Ticket>> GetTicketsPage(int page = 1, int limit = 500)
        {
            var contentStr = await GetTicketsPageContent(page, limit);// ?? throw new Exception();
            if (contentStr == null) return null;
            var pageTickets = JsonSerializer.Deserialize<List<Ticket>>(contentStr);
            return pageTickets;
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
            var response = await HttpClient.GetAsync($"tickets?{nameof(limit)}={limit}&{nameof(page)}={page}&{FullTicketModelQuery}");
            if (!response.IsSuccessStatusCode) return null;
            var content = response.Content;
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
    }
}
