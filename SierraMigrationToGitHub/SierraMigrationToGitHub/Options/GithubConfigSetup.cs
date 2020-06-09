namespace SierraMigrationToGitHub
{
    public class GithubConfigSetup
    {
        public GithubAccount Account { get; set; }
        public GithubProject Project { get; set; }

        public class GithubAccount
        {
            public string UserName { get; set; }
            public string Token { get; set; }
            public string Password { get; set; }
        }
        public class GithubProject
        {
            public string Name { get; set; }
        }
    }
}
