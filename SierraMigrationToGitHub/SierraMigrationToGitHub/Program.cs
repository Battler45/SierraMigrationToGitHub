using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SierraMigrationToGitHub.HttpClients;
using SierraMigrationToGitHub.Models.Github;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace SierraMigrationToGitHub
{

    class Program
    {
        private static readonly IServiceProvider ServiceProvider;
        public static IConfiguration Configuration { get; private set; }
        static async Task Main()
        {
            var unfuddleOptions = ServiceProvider.GetService<IOptions<UnfuddleConfigSetup>>().Value;
            var githubOptions = ServiceProvider.GetService<IOptions<GithubConfigSetup>>().Value;
            var githubMigration =  GithubMigration.GetGithubMigration(githubOptions, unfuddleOptions);
            var tickets = await githubMigration.UnfuddleClient.GetTickets();

            var ticketsWithoutNullAttachments = tickets.Where(t => t.attachments != null).ToList();
            var ticketsWithoutNullCommentsAttachments = tickets.Where(t => t?.comments?.Any(c => c.attachments != null) ?? false).ToList();


            var attachesQueue = await githubMigration.MigrateTicketsWithFiles(ticketsWithoutNullAttachments.Take(2).ToList());

            if (attachesQueue != null)
            {
                var attachesMigration = AttachesMigration.GetAttachesMigration(attachesQueue, new FileService(), githubOptions);
                attachesMigration.MigrateAttaches();
            }
            //var succ = await githubMigration.MigrateTicket(115);
            //Console.WriteLine(tickets.Count);
            //TestSelenium(githubOptions);

        }
        static void TestSelenium(GithubConfigSetup githubOptions)
        {

            var appPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var filePathes = new List<string>
            {
                $"{appPath}\\Files\\work.zip",
                $"{appPath}\\Files\\work.txt"
            };

            var selenium = Selenium.GetSeleniumClient;
            selenium.SignIn(githubOptions.Account.UserName, githubOptions.Account.Password)
                .AttachFileToComment("https://github.com/Battler45/SierraMigrationToGitHub/issues/16", filePathes, "632684202", false)
                .AttachFileToComment("https://github.com/Battler45/SierraMigrationToGitHub/issues/17", filePathes, "640603413", true)
                ;

        }

        static Program()
        {
            Configuration = new ConfigurationBuilder()
               .AddJsonFile(@"appsettings.json", optional: false, reloadOnChange: true)
               .Build();

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddOptions();
            serviceCollection.Configure<GithubConfigSetup>(Configuration.GetSection(nameof(GithubConfigSetup)));
            serviceCollection.Configure<UnfuddleConfigSetup>(Configuration.GetSection(nameof(UnfuddleConfigSetup)));

            ServiceProvider = serviceCollection.BuildServiceProvider();
        }

    }
}
