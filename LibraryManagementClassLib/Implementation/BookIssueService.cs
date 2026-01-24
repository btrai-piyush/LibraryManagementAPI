using LibraryManagementClassLib.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementClassLib.Implementation
{
    public class BookIssueService : IBookIssueService
    {
        public Task<bool> BorrowBookAsync(string isbn, int memberId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ReturnBookAsync(string isbn, int memberId)
        {
            throw new NotImplementedException();
        }
    }
}
