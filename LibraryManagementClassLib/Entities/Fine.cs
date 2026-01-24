using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementClassLib.Entities
{
    public enum PaidStatus
    {
        Unpaid,
        Paid
    }
    public class Fine
    {
        public int Id { get; set; }
        public int BookIssueId { get; set; }

        [Precision(8, 2)]
        public decimal Amount { get; set; }
        public PaidStatus Status { get; set; }
        public DateTime? PaidDate { get; set; }

        public BookIssue BookIssue { get; set; }
    }
}
