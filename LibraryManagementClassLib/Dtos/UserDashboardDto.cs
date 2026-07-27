using LibraryManagementClassLib.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementClassLib.Dtos
{
    public class UserDashboardDto
    {
        public int ActiveBorrowings { get; set; }
        public int UnpaidFines { get; set; }
        public decimal UnpaidFinesAmount { get; set; }
        public int WishlistItems { get; set; }
        public int RequestedBooks { get; set; }
        public List<BookIssuesDto> UpcomingDueDates { get; set; } = new List<BookIssuesDto>();
        public List<ActivityLogDto> RecentActivity { get; set; } = new List<ActivityLogDto>();
        public List<BookDto> RecommendedBooks { get; set; } = new List<BookDto>();
    }
}