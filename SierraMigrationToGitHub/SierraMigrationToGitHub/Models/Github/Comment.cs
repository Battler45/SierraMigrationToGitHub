using System;
using System.Collections.Generic;
using System.Text;

namespace SierraMigrationToGitHub.Models.Github
{
    public class Comment
    {
        public int id { get; set; }
        public string node_id { get; set; }
        public string url { get; set; }
        public string html_url { get; set; }
        public string body { get; set; }
        public User user { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
    }

}
