using SierraMigrationToGitHub.Models.Github;
using SierraMigrationToGitHub.Models.Unfuddle;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static SierraMigrationToGitHub.FileService;

namespace SierraMigrationToGitHub.HttpClients
{
    class GithubMigration
    {
        #region
        private FileService fileService;
        private FileService FileService
        {
            get => fileService ??= new FileService();
        }
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

        #region
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
        public async Task<bool> MigrateAllTicketsFromId(int ticketId)
        {
            //it's not efficient way to do this
            var tickets = (await UnfuddleClient.GetTickets()).Where(t => t.id >= ticketId);
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
            var issueUpdateResult = await GithubClient.UpdateIssue(issue.number, issueUpsertModel);
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
            return newComments.All(c => c != null);
        }
        #endregion

        public async Task<bool> MigrateTicketWithFiles(Ticket ticket)
        {
            var projectPeople = await UnfuddleClient.GetProjectPeople();
            ticket = ticket.AddAuthorAnnotationToTicketAndComments(projectPeople);
            //issue
            var issueUpsertModel = Mapper.ToIssueUpsertModel(ticket);
            var issue = await GithubClient.CreateIssue(issueUpsertModel);
            if (issue == null) return false;
            //comments
            //var commentsMigrateResult = await MigrateTicketComments(ticket, issue.number);

            //migrate comments
            var commentsMigration = ticket.comments != null
                ? await MigrateTicketComments_(ticket, issue.number)
                : new List<CommentMigration>();

            //save attachments 
            var commentWithAttaches = commentsMigration.Where(commentsMigration => commentsMigration.UnfuddleComment.attachments?.Any() ?? false);
            var localDir = Directory.GetCurrentDirectory();
            var saveCommentsAttachesTasks = commentWithAttaches.Select(commentMigration => SaveCommentAttachesToQueue(ticket, issue, commentMigration, localDir));
            var commentsAttachesModels = await Task.WhenAll(saveCommentsAttachesTasks);
            var ticketAttachesModel = await SaveTikcetAttachesToQueue(ticket, issue, localDir);

            //add attacments model to queue
            AttachesQueue.Enqueue(ticketAttachesModel);
            commentsAttachesModels.ToList().ForEach(commentsAttachesModel => AttachesQueue.Enqueue(commentsAttachesModel));
            return true;
        }
        private async Task<List<CommentMigration>> MigrateTicketComments_(Ticket ticket, int githubIssueNumber)
        {
            //if (ticket == null) throw new ArgumentNullException($"{nameof(MigrateTicketComments_)}:ticket is null");
            var MigrateCommentsTasks = ticket.comments.Select(c => MigrateTickeComment(c, githubIssueNumber));
            var commentsMigration = await Task.WhenAll(MigrateCommentsTasks);
            return commentsMigration.ToList();
        }
        private async Task<CommentMigration> MigrateTickeComment(Models.Unfuddle.Comment comment, int githubIssueNumber)
        {
            var commentUpsertModel = Mapper.ToCommentUpsertModel(comment);
            var githubComment = await GithubClient.CreateComment(githubIssueNumber, commentUpsertModel);
            return new CommentMigration
            {
                GithubComment = githubComment,
                UnfuddleComment = comment
            };
        }

        private async Task<Selenium.AttachToGithubTaskModel> SaveTikcetAttachesToQueue(Ticket ticket, Issue issue, string localDir)
        {
            if (ticket?.attachments.Length < 0) return null;
            var filePathes = await DownloadTicketFiles(ticket, localDir);
            var updateModel = new Selenium.AttachToGithubTaskModel
            {
                IsFirstIssueComment = true,
                CommentId = issue.id.ToString(),
                IssueUrl = issue.html_url,
                FilePathes = filePathes
            };
            return updateModel;
        }
        private async Task<Selenium.AttachToGithubTaskModel> SaveCommentAttachesToQueue(Ticket ticket, Issue issue, CommentMigration commentMigration, string localDir)
        {
            if ((commentMigration.UnfuddleComment.attachments?.Length ?? 0) == 0) return null;
            var filePathes = await DownloadCommentFiles(ticket, commentMigration.UnfuddleComment, localDir);
            var updateModel = new Selenium.AttachToGithubTaskModel
            {
                IsFirstIssueComment = false,
                CommentId = commentMigration.GithubComment.id.ToString(),
                IssueUrl = issue.html_url,
                FilePathes = filePathes
            };
            return updateModel;
        }
        private async Task<List<string>> DownloadTicketFiles(Ticket ticket, string fileFolder)
        {
            var downloadFilesTasks = ticket.attachments.Select(attacment => UnfuddleClient.DownloadTicketFile(ticket.id, attacment.id, fileFolder, attacment.filename));
            var filePathes = await Task.WhenAll(downloadFilesTasks);
            return filePathes.Where(filePath => filePath != null).ToList();
        }
        private async Task<List<string>> DownloadCommentFiles(Ticket ticket, Models.Unfuddle.Comment comment, string fileFolder)
        {
            var downloadFilesTasks = ticket.attachments.Select(attacment => UnfuddleClient.DownloadCommentFile(ticket.id, comment.id, attacment.id, fileFolder, attacment.filename));
            var filesPathes = await Task.WhenAll(downloadFilesTasks);
            return filesPathes.Where(streamFileSaveModels => streamFileSaveModels != null).ToList();
        }




        private ConcurrentQueue<Selenium.AttachToGithubTaskModel> attachesQueue;
        private ConcurrentQueue<Selenium.AttachToGithubTaskModel> AttachesQueue => attachesQueue ??= new ConcurrentQueue<Selenium.AttachToGithubTaskModel>();

        public async Task<ConcurrentQueue<Selenium.AttachToGithubTaskModel>> MigrateTicketsWithFiles(IEnumerable<Ticket> tickets)
        {

            //Selenium

            //var attachesMigrationTask = attachesMigration.MigrateAttachesAsync();

            
            foreach (var ticket in tickets)
            {
                await MigrateTicketWithFiles(ticket);
            }
            
//-----
/*
            var ticketsMigrationTasks = tickets.Select(MigrateTicketWithFiles);
            var ticketsMigrationResults = await Task.WhenAll(ticketsMigrationTasks);
*/
            //attachesMigration.DontWaitNewFiles();
            //await attachesMigrationTask;

            
            return AttachesQueue;
        }
        public class CommentMigration
        {
            public Models.Github.Comment GithubComment { get; set; }
            public Models.Unfuddle.Comment UnfuddleComment { get; set; }
        }
    }

    public class AttachesMigration
    {
        private Selenium selenium;
        private Selenium Selenium => selenium ??= Selenium.GetSeleniumClient;
        static private AttachesMigration attachesMigration;
        static public AttachesMigration GetAttachesMigration(ConcurrentQueue<Selenium.AttachToGithubTaskModel> attachesQueue, FileService fileService, GithubConfigSetup githubConfigSetup) 
            => attachesMigration ??= new AttachesMigration(attachesQueue, fileService, githubConfigSetup);
        private ConcurrentQueue<Selenium.AttachToGithubTaskModel> AttachesQueue { get; set; }
        private GithubConfigSetup GithubConfigSetup { get; set; }
        private FileService FileService { get; set; }
        private AttachesMigration(ConcurrentQueue<Selenium.AttachToGithubTaskModel> attachesQueue, FileService fileService, GithubConfigSetup githubConfigSetup)
        {
            (AttachesQueue, FileService, GithubConfigSetup) = (attachesQueue, fileService, githubConfigSetup);
        }
        private bool WaitingNewFiles { get; set; } = true;
        public void DontWaitNewFiles() => WaitingNewFiles = false; 
        public void MigrateAttaches() 
        {
            Selenium.SignIn(GithubConfigSetup.Account.UserName, GithubConfigSetup.Account.Password);
                while (!AttachesQueue.IsEmpty)
                {
                    AttachesQueue.TryDequeue(out Selenium.AttachToGithubTaskModel attachToGithubTaskModel);
                    if (attachToGithubTaskModel == null)
                    {
                        Thread.Sleep(500);
                        break;
                    }
                    Selenium.AttachFileToComment(attachToGithubTaskModel);
                    attachToGithubTaskModel.FilePathes.ForEach(FileService.DeleteFile);
                }
        }
        public async Task MigrateAttachesAsync()
        {
            await Task.Run(() =>
            {
                Selenium.SignIn(GithubConfigSetup.Account.UserName, GithubConfigSetup.Account.Password);
                while (!AttachesQueue.IsEmpty || WaitingNewFiles)
                {
                    AttachesQueue.TryDequeue(out Selenium.AttachToGithubTaskModel attachToGithubTaskModel);
                    if (attachToGithubTaskModel == null)
                    {
                        Thread.Sleep(500);
                        break;
                    }
                    Selenium.AttachFileToComment(attachToGithubTaskModel);
                    attachToGithubTaskModel.FilePathes.ForEach(FileService.DeleteFile);
                }
            });
        }
    }
}
