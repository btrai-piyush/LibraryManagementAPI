using LibraryManagementClassLib.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementClassLib.Dtos
{
    public class RequestedBooksDto
    {
        public int? Id { get; set; }
        public BookDto Book {  get; set; }
        public UserDto User { get; set; }

        public DateTime RequestDate { get; set; }
        public string Status { get; set; }
        public int TotalCount { get; set; }
    }
}
