using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementClassLib.Dtos
{
    public class UndoBookRequestDto
    {
        public int UserId { get; set; }
        public int BookId { get; set; }
        public bool RemoveFromWishlist { get; set; }
    }
}
