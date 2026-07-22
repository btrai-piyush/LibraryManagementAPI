using LibraryManagementClassLib.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementClassLib.Services
{
    public interface IBookIssueService
    {
        Task<string> IssueBookAsync(int requestId, DateTime dueDate);
        Task<string> ReturnBookAsync(int issueId);
        Task<List<BookIssuesDto>> GetAllBookIssuesAsync(GeneralQueryDto query);
        Task<List<BookIssuesDto>> GetBookIssuesByUserIdAsync(int userId);
        Task UpdateBookIssueStatus();
    }
}
