using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementClassLib.Dtos
{
    public class AdminStudentViewDto
    {
        public UserResponseDto User { get; set; }
        public List<BookIssuesDto> BookIssues { get; set; }
        public List<FineDto> Fines { get; set; }
    }
}
