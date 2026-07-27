using LibraryManagementClassLib.Data;
using LibraryManagementClassLib.Dtos;
using LibraryManagementClassLib.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementClassLib.Implementation
{
    public class SubjectService : ISubjectService
    {
        private readonly LibraryManagementAPIDbContext _context;

        public SubjectService(LibraryManagementAPIDbContext context)
        {
            _context = context;
        }

        public async Task<List<SubjectDto>> GetAllSubjectsAsync()
        {
            var subjects = await Task.Run(() => _context.Subjects.ToList());
            return subjects.Select(s => new SubjectDto
            {
                Id = s.Id,
                Name = s.Name,
                Code = s.Code,
                CourseId = s.CourseId,
                SemesterCode = s.SemesterCode
            }).ToList();
        }
    }
}
