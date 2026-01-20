using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementClassLib.Entities
{
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string ISBN { get; set; }
        public int TotalCopies { get; set; }
        public int AvailableCopies { get; set; }
        public int AuthorId { get; set; }
        public int CategoryId { get; set; }
        public int PublisherId { get; set; }

        public Author Author { get; set; }
        public Category Category { get; set; }
        public Publisher Publisher { get; set; }
        public ICollection<BookIssue>? BookIssues { get; set; }
    }
}
