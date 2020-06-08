using SierraMigrationToGitHub.Models.Github;
using SierraMigrationToGitHub.Models.Unfuddle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SierraMigrationToGitHub.HttpClients
{
    class GithubMigration
    {
        #region
        private GithubConfigSetup githubConfigSetup;
        private GithubClient githubClient;
        private GithubClient GithubClient
        {
            get => githubClient ??= GithubClient.GetClient(githubConfigSetup);
        }
        private UnfuddleConfigSetup unfuddleConfigSetup;
        private UnfuddleClient unfuddleClient;
        public UnfuddleClient UnfuddleClient
        {
            get => unfuddleClient ??= UnfuddleClient.GetClient(unfuddleConfigSetup);
        }

        private static GithubMigration migration;
        public static GithubMigration GetGithubMigration(GithubConfigSetup githubOptions, UnfuddleConfigSetup unfuddleConfigSetup)
            => migration ??= new GithubMigration(githubOptions, unfuddleConfigSetup);
        private GithubMigration(GithubConfigSetup githubOptions, UnfuddleConfigSetup unfuddleOptions)
            => (unfuddleConfigSetup, githubConfigSetup) = (unfuddleOptions, githubOptions);
        #endregion

        //private async Task<>
        public async Task<bool> MigrateTicket(int ticketId)
        {
            var ticket = await UnfuddleClient.GetTicket(ticketId);
            return await MigrateTicket(ticket);
        }
        public async Task<bool> MigrateTicket(Ticket ticket)
        {
            var projectPeople = await UnfuddleClient.GetProjectPeople();
            ticket = ticket.AddAuthorAnnotationToTicketAndComments(projectPeople);
            //issue
            var issueUpsertModel = Mapper.ToIssueUpsertModel(ticket);
            var issue = await GithubClient.CreateIssue(issueUpsertModel);
            if (issue == null) return false;
            //comments
            var commentsMigrateResult = await MigrateTicketComments(ticket, issue.number);
            return commentsMigrateResult;
        }
        public async Task<bool> MigrateAllTickets()
        {
            var tickets = await UnfuddleClient.GetTickets();
            return await MigrateTickets(tickets);
        }
        public async Task<bool> MigrateTickets(IEnumerable<Ticket> tickets)
        {
            var ticketsMigrationTasks = tickets.Select(MigrateTicket);
            var ticketsMigrationResults = await Task.WhenAll(ticketsMigrationTasks);
            return ticketsMigrationResults.All(r => r);
        }
        private async Task<bool> MigrateTicketComments(Ticket ticket, int githubIssueNumber)
        {
            var commentUpsertModels = ticket.comments.Select(Mapper.ToCommentUpsertModel);
            var comments = await GithubClient.CreateComments(githubIssueNumber, commentUpsertModels);
            return comments.All(c => c != null);
        }

        public async Task<bool> UpsertTicketsFromNumber(int beginTicketNumber)
        {
            var tickets = await UnfuddleClient.GetTicketsFromNumber(beginTicketNumber);
            return await UpsertTickets(tickets);
        }
        public async Task<bool> UpsertTickets(IEnumerable<Ticket> tickets)
        {
            var upsertTicketTasks = tickets.Select(UpsertTicket);
            var upsertTicketResults = await Task.WhenAll(upsertTicketTasks);
            return upsertTicketResults.All(r => r);
        }
        public async Task<bool> UpsertTicket(Ticket ticket)
        {
            var issue = await GithubClient.FindIssueByTitle(ticket.summary);
            return issue == null
                ? await MigrateTicket(ticket)
                : await UpdateTicket(ticket, issue);
        }
        public async Task<bool> UpdateTicket(Ticket ticket)
        {
            var issue = await GithubClient.FindIssueByTitle(ticket.summary);
            return await UpdateTicket(ticket, issue);
        }
        public async Task<bool> UpdateTicket(Ticket ticket, Issue issue)
        {
            var issueUpsertModel = Mapper.ToIssueUpsertModel(ticket);
            var issueUpdateResult =  await GithubClient.UpdateIssue(issue.number, issueUpsertModel);
            if (issueUpdateResult == null) return false;
            if (ticket.comments.Length > issue.comments)
               return await UpdateIssueComments(issue, ticket);
            return true;
        }
        private async Task<bool> UpdateIssueComments(Issue issue, Ticket ticket)
        {
            var issueComments = await GithubClient.GetIssueComments(issue.number);
            var newUnfuddleComments = new List<Models.Unfuddle.Comment>();
            foreach (var ticketComment in ticket.comments)
                if (issueComments.All(issueComment => ticketComment.body != issueComment.body))
                    newUnfuddleComments.Add(ticketComment);
            var newComments = await GithubClient.CreateComments(issue.number, newUnfuddleComments.Select(Mapper.ToCommentUpsertModel));
            return newComments.All(c => c != null) ;
        }
    }
}
