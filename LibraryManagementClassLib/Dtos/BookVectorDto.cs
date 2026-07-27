using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementClassLib.Dtos
{
    public class BookVectorDto
    {
        public int BookId { get; set; }

        public double[] Vector { get; set; } = Array.Empty<double>();
    }
}
