using System;
using System.Collections.Generic;
using System.Text;

namespace SierraMigrationToGitHub.Models.Github
{
    public class IssueUpsertModel : IssueCreatingModel
    {
        public string state { get; set; }
    }

    public class IssueCreatingModel
    {
        public string title { get; set; }
        public string body { get; set; }
        public string[] assignees { get; set; }
        public int? milestone { get; set; }
        public string[] labels { get; set; }
    }
}
