using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementClassLib.Entities
{
    [Index(nameof(Code), IsUnique = true)]
    public class Course:BaseEntity
    {
        public string Name { get; set; }

        public string Code { get; set; }

        public ICollection<Subject> Subjects { get; set; }
        public ICollection<StudentDetail> StudentDetails { get; set; }
    }
}
