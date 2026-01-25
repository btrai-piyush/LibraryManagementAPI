using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementClassLib.Entities
{
    public enum RequestStatus
    {
        Pending,
        Issued,
        Rejected
    }
    public class BorrowRequest
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public int UserId { get; set; }
        public DateTime RequestDate { get; set; }
        public RequestStatus Status { get; set; }

        public Book Book { get; set; }
        public User User { get; set; }
    }
}
