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
        [Required(ErrorMessage = "Title is required.")]
        public string Title { get; set; }

        public List<AuthorDto> Authors { get; set; } = new List<AuthorDto>();

        [Required(ErrorMessage = "ISBN is required.")]
        public string ISBN { get; set; }

        [Required(ErrorMessage = "Number of copies is required.")]
        public int Copies { get; set; }

        [Required(ErrorMessage = "Category is required.")]
        public List<string> Categories { get; set; } = new List<string>();

        [Required(ErrorMessage = "Publisher is required.")]
        public string Publisher { get; set; }

        [Required(ErrorMessage = "Publisher address is required.")]
        public string PublisherAddress { get; set; }
    }
}
