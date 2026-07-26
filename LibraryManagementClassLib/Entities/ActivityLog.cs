using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace LibraryManagementClassLib.Entities
{
    public enum ActivityType
    {
        BookIssued,
        BookReturned,
        BookWishlisted,
        BookUnwishlisted,
        BookRequested,
        BookRequestCancelled,
        BookRequestRejected,
        FinePaid,
        FineIncurred
    }

    public class ActivityLog : BaseEntity
    {
        public int UserId { get; set; }
        public ActivityType ActivityType { get; set; }
        public string Description { get; set; }
        public string? MetaData { get; set; }
        public DateTime CreatedAt { get; set; }
        public int ReferenceId { get; set; }

        public User User { get; set; }
    }
}
