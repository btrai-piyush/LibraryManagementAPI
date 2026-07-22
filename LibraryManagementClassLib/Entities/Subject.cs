using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementClassLib.Entities
{
    public class Subject:BaseEntity
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public int CourseId { get; set; }
        public  string SemesterCode { get; set; }

        public ICollection<Book> Books { get; set; }
        public Course Course { get; set; }
    }
}
