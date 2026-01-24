using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementClassLib.Services
{
    public interface IBookIssueService
    {
        Task<bool> BorrowBookAsync(string isbn, int memberId);
        Task<bool> ReturnBookAsync(string isbn, int memberId);
    }
}
