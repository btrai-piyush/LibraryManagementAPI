using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementClassLib.Dtos
{
    public class BookIssuesDto
    {
        public int BookIssueId { get; set; }
        public BookDto Book { get; set; }
        public UserDto User { get; set; }
        public DateTime IssuedDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? ReturnedDate { get; set; }
        public string Status { get; set; }
        public int TotalCount { get; set; }
    }
}
