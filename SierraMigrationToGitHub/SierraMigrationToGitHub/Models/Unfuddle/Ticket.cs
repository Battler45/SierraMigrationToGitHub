using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SierraMigrationToGitHub.Models.Unfuddle
{
    //generate from Tickets.json
    public class Ticket
    {
        public int? assignee_id { get; set; }
        public object component_id { get; set; }
        public DateTime created_at { get; set; }
        public string description { get; set; }
        public string description_format { get; set; }
        public string due_on { get; set; }
        public object field1_value_id { get; set; }
        public object field2_value_id { get; set; }
        public object field3_value_id { get; set; }
        public float hours_estimate_current { get; set; }
        public float hours_estimate_initial { get; set; }
        public int id { get; set; }
        public object milestone_id { get; set; }
        public int number { get; set; }
        public string priority { get; set; }
        public int project_id { get; set; }
        public int reporter_id { get; set; }
        public string resolution { get; set; }
        public string resolution_description { get; set; }
        public string resolution_description_format { get; set; }
        public int? severity_id { get; set; }
        public object sort_order { get; set; }
        public string status { get; set; }
        public string summary { get; set; }
        public DateTime updated_at { get; set; }
        public object version_id { get; set; }
        public string description_formatted { get; set; }
        public string resolution_description_formatted { get; set; }
        public string created_at_formatted { get; set; }
        public string updated_at_formatted { get; set; }
        public string due_on_formatted { get; set; }
        public int count { get; set; }
        public Comment[] comments { get; set; }
        public Attachment[] attachments { get; set; }

        public void AppendToDescriptionAuthor(string author)
        {
            //string newLine = Environment.NewLine;
            description += $"\n Posted by {author}";
            if (description_formatted != null)
            description_formatted += $"\n Posted by <b>{author}</b>";
        }

        public Ticket AddAuthorAnnotationToTicketAndComments(Dictionary<int, Models.Unfuddle.User> people)
        {
            if (people == null) return null;
            if (this.assignee_id.HasValue && people.ContainsKey(this.assignee_id.Value))
                this.AppendToDescriptionAuthor(GetUserNamesAndLogin(people[this.assignee_id.Value]));
            if (this.comments != null)
            foreach (var comment in this.comments)
                if (people.ContainsKey(comment.author_id))
                    comment.AppendToBodyAuthor(GetUserNamesAndLogin(people[comment.author_id]));
            return this;
            static string GetUserNamesAndLogin(Models.Unfuddle.User user) => $"{user.last_name} {user.first_name}(unfuddle username: {user.username})";
        }
    }

    public class Comment
    {
        public int author_id { get; set; }
        public string body { get; set; }
        public string body_format { get; set; }
        public DateTime created_at { get; set; }
        public int id { get; set; }
        public int parent_id { get; set; }
        public string parent_type { get; set; }
        public DateTime updated_at { get; set; }
        public Attachment[] attachments { get; set; }
        public void AppendToBodyAuthor(string author)
        {
            //string newLine = Environment.NewLine;
            body += body_format == null 
                ? $"\n Posted by {author}"
                : $"\n Posted by <b>{author}</b>";
        }
    }

    public class Attachment
    {
        public Parent parent { get; set; }
        public Grandparent grandparent { get; set; }
        public string content_type { get; set; }
        public DateTime created_at { get; set; }
        public string filename { get; set; }
        public int id { get; set; }
        public int parent_id { get; set; }
        public string parent_type { get; set; }
        public DateTime updated_at { get; set; }
        public int size { get; set; }
        public int project_id { get; set; }

        public class Grandparent
        {
            public string type { get; set; }
            public int id { get; set; }
            public string title { get; set; }
            public string status { get; set; }
            public int number { get; set; }
        }

        public class Parent
        {
            public string title { get; set; }
            public string status { get; set; }
            public int number { get; set; }
        }
    }

}
