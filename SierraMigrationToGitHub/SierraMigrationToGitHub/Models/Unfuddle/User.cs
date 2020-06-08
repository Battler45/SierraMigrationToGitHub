using System;
using System.Collections.Generic;
using System.Text;

namespace SierraMigrationToGitHub.Models.Unfuddle
{
    public class User
    {
        public int account_id { get; set; }
        public DateTime created_at { get; set; }
        public string email { get; set; }
        public string first_name { get; set; }
        public int id { get; set; }
        public bool is_administrator { get; set; }
        public string last_name { get; set; }
        public DateTime last_signed_in { get; set; }
        public string notification_frequency { get; set; }
        public bool notification_ignore_self { get; set; }
        public DateTime notification_last_sent { get; set; }
        public string notification_scope_messages { get; set; }
        public string notification_scope_milestones { get; set; }
        public string notification_scope_source { get; set; }
        public string notification_scope_tickets { get; set; }
        public string notification_scope_notebooks { get; set; }
        public string time_zone { get; set; }
        public DateTime updated_at { get; set; }
        public string username { get; set; }
        public bool is_removed { get; set; }
        public string text_markup { get; set; }
        public bool authy_registered { get; set; }
        public DateTime? otp_enabled { get; set; }
        public bool is_owner { get; set; }
    }

}
