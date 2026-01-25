using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementClassLib.Services
{
    public interface IBookRequestService
    {
        Task<string> RequestBookAsync(int userId, int bookId);
    }
}
