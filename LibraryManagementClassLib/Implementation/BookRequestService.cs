using LibraryManagementClassLib.Data;
using LibraryManagementClassLib.Entities;
using LibraryManagementClassLib.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementClassLib.Implementation
{
    public class BookRequestService : IBookRequestService
    {
        private readonly LibraryManagementAPIDbContext _context;

        public BookRequestService(LibraryManagementAPIDbContext context)
        {
            _context = context;
        }

        public async Task<string> RequestBookAsync(int userId, int bookId)
        {
            var borrowRequest = new BorrowRequest
            {
                BookId = bookId,
                UserId = userId,
                RequestDate = DateTime.Now,
                Status = RequestStatus.Pending
            };
            try
            {
                await _context.BorrowRequests.AddAsync(borrowRequest);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error submitting book request: " + ex.Message);
            }
            return "Book request submitted successfully.";
        }
    }
}
