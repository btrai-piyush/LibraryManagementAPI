using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementClassLib.Entities
{
    public class Author
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "First Name is required")]
        public string FirstName { get; set; }

        [Required(ErrorMessage ="Last Name is required")]
        public string LastName { get; set; }

        public ICollection<Book>? Books { get; set; }
    }
}
