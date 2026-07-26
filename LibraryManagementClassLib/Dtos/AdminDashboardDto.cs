using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementClassLib.Dtos
{
    public class AdminDashboardDto
    {
        public int TotalBooks { get; set; }
        public int TotalUsers { get; set; }
        public int ActiveBorrowings { get; set; }
        public int PendingRequests { get; set; }
        public int UnpaidFines { get; set; }
        public List<ActivityLogDto>? RecentActivity { get; set; }
    }
}
