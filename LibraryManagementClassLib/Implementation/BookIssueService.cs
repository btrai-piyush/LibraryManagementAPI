

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
        public async Task<string> ConfirmBorrowAsync(int userId, int bookId)
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

            await _context.SaveChangesAsync();
            return "Book borrowed successfully";
        }

        public async Task<string> ReturnBookAsync(int userId, int bookId)
        {
            var bookIssue = _context.BookIssues.Where(
                bi => bi.BookId == bookId && bi.UserId == userId).FirstOrDefault();
            if (bookIssue == null)
            {
                throw new Exception("No active issue found for this book and user.");
            }
            if (bookIssue.Status != IssueStatus.Active)
            {
                throw new Exception("This book has already been returned.");
            }
            var fineStatus = _context.Fines.Any(f => f.BookIssueId == bookIssue.Id && f.Status == PaidStatus.Unpaid);
            if (fineStatus)
            {
                throw new Exception("Cannot return book with unpaid fines.");
            }
            var book = _context.Books.Find(bookId);
            if (book is null)
            {
                throw new Exception("Book not found.");
            }
            bookIssue.ReturnDate = DateTime.Today;
            bookIssue.Status = IssueStatus.Returned;
            book.AvailableCopies += 1;
            try
            {
                _context.Books.Update(book);
                _context.BookIssues.Update(bookIssue);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error returning book: " + ex.Message);
            }
            return "Book returned successfully.";
        }

        public Task<IEnumerable<BookIssue>> GetBorrowedBooksAsync(int memberId)
        {
            var borrowedBooks = _context.BookIssues
                .Include(bi => bi.Book)
                .Where(bi => bi.UserId == memberId && bi.ReturnDate == null);
            return Task.FromResult(borrowedBooks.AsEnumerable());
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
