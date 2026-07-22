using LibraryManagementClassLib.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementClassLib.Dtos
{
    public class WishlistResponseDto
    {
        public List<Book> Books { get; set; } = new List<Book>();
    }
}
