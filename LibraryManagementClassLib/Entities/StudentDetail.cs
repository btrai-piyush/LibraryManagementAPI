using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementClassLib.Entities
{
    public class StudentDetail : BaseEntity
    {
        public int CourseId { get; set; }
        public int UserId { get; set; }
        public int Semester { get; set; }

        public User? User { get; set; }
        public Course Course { get; set; }
    }
}
