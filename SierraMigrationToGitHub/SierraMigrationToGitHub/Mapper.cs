using SierraMigrationToGitHub.Models.Github;
using SierraMigrationToGitHub.Models.Unfuddle;
using System;
using System.Collections.Generic;
using System.Text;

namespace SierraMigrationToGitHub
{

    class Mapper
    {
        public static IssueUpsertModel ToIssueUpsertModel(Ticket ticket)
        {
            var ticketDescription = string.IsNullOrWhiteSpace(ticket.description_formatted)
                ? ticket.description
                : ticket.description_formatted;
            var issue = new IssueUpsertModel
            {
                title = ticket.summary,
                body = ticketDescription,
                state = ticket.status?.ToLower() == GithubIssueStates.Closed
                ? GithubIssueStates.Closed
                : GithubIssueStates.Open,
            }; 

            //issue.assignees //ticket.assignee_id
            //issue.milestone ticket.milestone_id every milestone is null
            //issue.labels ticket.priority ticket.status
            return issue;
        }
        public static CommentUpsertApiModel ToCommentUpsertModel(Models.Unfuddle.Comment comment)
            => new CommentUpsertApiModel 
            {
                body = comment.body
            };
    }
}
