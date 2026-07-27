using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementClassLib.Dtos
{
    public class BookRecommendationDto
    {
        public int BookId { get; set; }

        public double Similarity { get; set; }
    }
}
