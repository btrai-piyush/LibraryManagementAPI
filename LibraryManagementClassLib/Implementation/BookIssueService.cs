

using LibraryManagementClassLib.Data;
using LibraryManagementClassLib.Dtos;
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
        public async Task<string> IssueBookAsync(int requestId, DateTime dueDate)
        {
            var borrowRequest = await _context.BorrowRequests.FindAsync(requestId);
            if (borrowRequest != null)
            {
                try
                {
                    await using var transaction = await _context.Database.BeginTransactionAsync();

                    await ValidateBorrowRequestAsync(borrowRequest.UserId, borrowRequest.BookId);

                    borrowRequest.Status = RequestStatus.Issued;
                    await _context.SaveChangesAsync();

                    var bookIssue = new BookIssue
                    {
                        BookId = borrowRequest.BookId,
                        UserId = borrowRequest.UserId,
                        IssueDate = DateTime.Today,
                        DueDate = dueDate,
                        Status = IssueStatus.Active
                    };
                    _context.BookIssues.Add(bookIssue);
                    await _context.SaveChangesAsync();

                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    throw new Exception("Error issuing book: " + ex.Message);
                }
            }
            else
            {
                throw new Exception("Borrow request not found.");
            }

            return "Book request approved successfully.";
        }

        public async Task<string> ReturnBookAsync(int issueId)
        {
            var bookIssue = _context.BookIssues.Where(
                bi => bi.Id == issueId).FirstOrDefault();
            if (bookIssue == null)
            {
                throw new Exception("No active issue found for this book and user.");
            }
            if (bookIssue.Status == IssueStatus.Returned)
            {
                throw new Exception("This book has already been returned.");
            }
            var fineStatus = _context.Fines.Any(f => f.BookIssueId == bookIssue.Id && f.Status == PaidStatus.Unpaid);
            if (fineStatus)
            {
                throw new Exception("Cannot return book with unpaid fines.");
            }
            var book = _context.Books.Find(bookIssue.BookId);
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

        public async Task<List<BookIssuesDto>> GetBookIssuesByUserIdAsync(int userId)
        {
            var bookIssues = await _context.BookIssues
                .Include(bi => bi.Book)
                .Include(bi => bi.User)
                .Where(bi => bi.UserId == userId && bi.ReturnDate == null)
                .Select(bi => new BookIssuesDto
                {
                    BookIssueId = bi.Id,
                    Book = new BookDto
                    {
                        Id = bi.Book.Id,
                        Title = bi.Book.Title,
                        Authors = bi.Book.Authors.Select(a => new AuthorDto
                        {
                            FirstName = a.FirstName,
                            LastName = a.LastName
                        }).ToList(),
                        ISBN = bi.Book.ISBN,
                        AvailableCopies = bi.Book.AvailableCopies,
                        Publisher = bi.Book.Publisher.Name,
                    },
                    User = new UserDto
                    {
                        FirstName = bi.User.FirstName,
                        LastName = bi.User.LastName,
                        Email = bi.User.Email
                    },
                    IssuedDate = bi.IssueDate,
                    DueDate = bi.DueDate,
                    Status = bi.Status.ToString()
                })
                .ToListAsync();
            if(bookIssues == null || !bookIssues.Any())
            {
                throw new Exception("No active book issues found for this user.");
            }

            return bookIssues;
        }

        private async Task ValidateBorrowRequestAsync(int memberId, int bookId)
        {
            var book = await _context.Books.FindAsync(bookId);
            if (book == null || book.AvailableCopies <= 0)
            {
                throw new InvalidOperationException("Book is not available for borrowing.");
            }

            book.AvailableCopies -= 1;
            await _context.SaveChangesAsync();
        }

        public async Task<List<BookIssuesDto>> GetAllBookIssuesAsync(GeneralQueryDto query)
        {
            var bookIssuesQuery = _context.BookIssues
                .Include(bi => bi.Book)
                .Include(bi => bi.User)
                .AsQueryable();

            if(!string.IsNullOrEmpty(query.SearchTerm))
            {
                var searchTerm = query.SearchTerm.ToLower();
                bookIssuesQuery = bookIssuesQuery.Where(bi =>
                    bi.Book.Title.ToLower().Contains(searchTerm) ||
                    bi.User.FirstName.ToLower().Contains(searchTerm) ||
                    bi.User.LastName.ToLower().Contains(searchTerm));
            }

            var queryCount = await bookIssuesQuery.CountAsync();

            bookIssuesQuery = bookIssuesQuery.OrderBy(bi => bi.IssueDate);

            var pageNumber = query.PageNumber <= 0 ? 1 : query.PageNumber;
            var pageSize = query.PageSize <= 0 ? 10 : query.PageSize;
            var skip = (pageNumber - 1) * pageSize;

            bookIssuesQuery = bookIssuesQuery.Skip(skip).Take(pageSize);

            return await bookIssuesQuery
                .Select(bi => new BookIssuesDto
                {
                    BookIssueId = bi.Id,
                    Book = new BookDto
                    {
                        Id = bi.Book.Id,
                        Title = bi.Book.Title,
                        Authors = bi.Book.Authors.Select(a => new AuthorDto
                        {
                            FirstName = a.FirstName,
                            LastName = a.LastName
                        }).ToList(),
                        ISBN = bi.Book.ISBN,
                        AvailableCopies = bi.Book.AvailableCopies,
                        Publisher = bi.Book.Publisher.Name,
                    },
                    User = new UserDto
                    {
                        FirstName = bi.User.FirstName,
                        LastName = bi.User.LastName,
                        Email = bi.User.Email
                    },
                    IssuedDate = bi.IssueDate,
                    DueDate = bi.DueDate,
                    Status = bi.Status.ToString(),
                    TotalCount = queryCount
                })
                .ToListAsync();
        }

        public async  Task UpdateBookIssueStatus()
        {
            var bookIssues=await _context.BookIssues.Where(bi=>bi.Status==IssueStatus.Active).ToListAsync();
            foreach (var bookIssue in bookIssues)
            {
                if(bookIssue.DueDate< DateTime.Now)
                {
                    bookIssue.Status = IssueStatus.Overdue;
                }
            }
            await _context.SaveChangesAsync();
        }
    }
}
