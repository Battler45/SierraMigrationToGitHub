using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SierraMigrationToGitHub.HttpClients;
using SierraMigrationToGitHub.Models.Github;
using System;
using System.IO;
using System.Threading.Tasks;

namespace SierraMigrationToGitHub
{
    class Program
    {
        private static readonly IServiceProvider ServiceProvider;
        public static IConfiguration Configuration { get; private set; }
        static async Task Main()
        {
            //
            var unfuddleOptions = ServiceProvider.GetService<IOptions<UnfuddleConfigSetup>>().Value;
            var githubOptions = ServiceProvider.GetService<IOptions<GithubConfigSetup>>().Value;
            var githubMigration =  GithubMigration.GetGithubMigration(githubOptions, unfuddleOptions);
            var succ = await githubMigration.MigrateTicket(115);
            Console.WriteLine(succ);
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
