using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementClassLib.Services
{
    public interface IBookIssueService
    {
        Task<string> BorrowBookAsync(int userId, int bookId);
        Task<bool> ReturnBookAsync(int userId,int bookId);
    }
}
