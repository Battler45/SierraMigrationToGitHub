namespace SierraMigrationToGitHub.Models.Github
{
    public class Label
    {
        public int id { get; set; }
        public string node_id { get; set; }
        public string url { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string color { get; set; }
        public bool _default { get; set; }
    }
}
