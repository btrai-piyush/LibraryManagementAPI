using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementClassLib.Dtos
{
    public class IssueBookDto
    {
        public int RequestId { get; set; }
        public DateTime DueDate { get; set; }
    }
}
