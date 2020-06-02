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
        private UnfuddleClient UnfuddleClient 
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
            //issue
            var issueUpsertModel = Mapper.ToIssueUpsertModel(ticket);
            var issue = await GithubClient.CreateIssue(issueUpsertModel);
            if (issue == null) return false;
            //comments
            var commentTasks = ticket.comments.Select(Mapper.ToIssueUpsertModel)
                .Select(c => GithubClient.CreateComment(issue.number, c))
                .ToArray();
            Task.WaitAll(commentTasks);
            return commentTasks.All(c => c.Result != null);
        }
    }
}
