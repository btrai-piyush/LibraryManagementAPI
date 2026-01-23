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
        public int PublisherId { get; set; }

        public ICollection<Author> Authors { get; set; } = new List<Author>();
        public ICollection<Category> Categories { get; set; } = new List<Category>();
        public Publisher Publisher { get; set; }
        public ICollection<BookIssue>? BookIssues { get; set; } = new List<BookIssue>();
    }
}
