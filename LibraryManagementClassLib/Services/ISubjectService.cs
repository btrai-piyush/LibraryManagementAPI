using LibraryManagementClassLib.Dtos;

namespace LibraryManagementClassLib.Services
{
    public interface ISubjectService
    {
        Task<List<SubjectDto>> GetAllSubjectsAsync();
    }
}
