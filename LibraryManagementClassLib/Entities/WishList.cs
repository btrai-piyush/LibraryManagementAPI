using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementClassLib.Entities
{
    public class WishList:BaseEntity
    {
        public int UserId { get; set; }
        public ICollection<Book>? Books { get; set; }=new List<Book>();
        public User User { get; set; }
    }
}
