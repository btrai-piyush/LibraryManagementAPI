using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementClassLib.Dtos
{
    public class BookFeatureDto
    {
        public int BookId { get; set; }

        public List<string> Features { get; set; } = new();
    }
}
