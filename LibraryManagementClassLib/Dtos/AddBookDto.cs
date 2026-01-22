using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementClassLib.Dtos
{
    public class AddBookDto
    {
        public string Title { get; set; }
        public string AuthorFirstName { get; set; }
        public string AuthorLastName { get; set; }
        public string ISBN { get; set; }
        public int Copies { get; set; }
        public string Category { get; set; }
        public string Publisher { get; set; }
        public string PublisherAddress { get; set; }
    }
}
