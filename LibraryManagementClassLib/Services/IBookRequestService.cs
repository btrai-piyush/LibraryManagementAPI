using LibraryManagementClassLib.Dtos;
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
        Task<List<BookDto>> GetRequestedBooksAsync(int userId);
        Task<string> UndoRequest(int userId, int bookId, bool removeWishlistItem = false);
        Task<List<RequestedBooksDto>> GetUserBookRequestsAsync(GeneralQueryDto query);
        Task<string> RejectBookRequest(int requestId);
        Task<List<RequestedBooksDto>> GetBookRequestHistoryByUser(GeneralQueryDto query);
        Task<List<RequestedBooksDto>> AdminGetPendingRequests(GeneralQueryDto query);
        Task<List<RequestedBooksDto>> AdminGetRequestHistory(GeneralQueryDto query);

    }
}
