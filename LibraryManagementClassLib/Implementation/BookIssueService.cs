using LibraryManagementClassLib.Data;
using LibraryManagementClassLib.Entities;
using LibraryManagementClassLib.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementClassLib.Implementation
{
    public class BookIssueService : IBookIssueService
    {
        private readonly LibraryManagementAPIDbContext _context;
        private readonly IBookService _bookService;

        public BookIssueService(LibraryManagementAPIDbContext context, IBookService bookService)
        {
            _context = context;
            _bookService = bookService;
        }
        public async Task<string> BorrowBookAsync(int userId, int bookId)
        {
            await ValidateBorrowRequestAsync(userId, bookId);
            var book = await _context.Books.FindAsync(bookId);
            var member = await _context.Users.FindAsync(userId);
            if (book is null || member is null)
            {
                return "Book or member not found";
            }

            var bookIssue = new BookIssue
            {
                BookId = bookId,
                UserId = userId,
                IssueDate = DateTime.Now,
                DueDate = DateTime.Now.AddDays(14),
                Status = IssueStatus.Active
            };
            book.AvailableCopies -= 1;
            await _context.BookIssues.AddAsync(bookIssue);
            await _bookService.UpdateBookAsync(book);

            await _context.SaveChangesAsync();
            return "Book borrowed successfully";
        }

        public Task<bool> ReturnBookAsync(int userId, int bookId)
        {
            throw new NotImplementedException();
        }

        private async Task ValidateBorrowRequestAsync(int memberId, int bookId)
        {
            var book = await _context.Books.FindAsync(bookId);
            if (book == null || book.AvailableCopies <= 0)
            {
                throw new InvalidOperationException("Book is not available for borrowing.");
            }
            var activeIssues = await _context.BookIssues
                .CountAsync(bi => bi.UserId == memberId && bi.ReturnDate == null);
            if (activeIssues >= 1)
            {
                throw new InvalidOperationException("Member has reached the maximum number of borrowed books.");
            }
        }
    }
}
