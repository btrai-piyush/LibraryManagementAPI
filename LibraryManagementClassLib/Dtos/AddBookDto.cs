using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementClassLib.Dtos
{
    public class AddBookDto
    {
        public string Title { get; set; }

        public List<AuthorDto> Authors { get; set; } = new List<AuthorDto>();

        [Required(ErrorMessage = "ISBN is required.")]
        public string ISBN { get; set; }

        [Required(ErrorMessage = "Number of copies is required.")]
        public int TotalCopies { get; set; }

        [Required(ErrorMessage = "Category is required.")]
        public List<string> Categories { get; set; } = new List<string>();

        [Required(ErrorMessage = "Publisher is required.")]
        public string Publisher { get; set; }
        public string? PublisherAddress { get; set; } = "";
        public List<int> SubjectIds { get; set; } = new List<int>();
    }
}
