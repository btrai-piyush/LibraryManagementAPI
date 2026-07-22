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

                var wishlist = await _context.WishLists
                    .Include(w => w.Books)
                    .FirstOrDefaultAsync(w => w.UserId == userId);

                if (wishlist != null)
                {
                    var book = wishlist.Books.FirstOrDefault(b => b.Id == bookId);

                    if (book != null)
                    {
                        wishlist.Books.Remove(book);
                    }
                }


                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error submitting book request: " + ex.Message);
            }
            return "Book request submitted successfully.";
        }

        public async Task<List<BookDto>> GetRequestedBooksAsync(int userId)
        {
            var requestedBooks = await _context.BorrowRequests
                .Where(br => br.UserId == userId && br.Status == RequestStatus.Pending)
                .Include(br => br.Book.Authors)
                .Select(br => br.Book)
                .ToListAsync();

            var books = await _context.Books
                .Where(b => requestedBooks.Select(rb => rb.Id).Contains(b.Id))
                .Include(b => b.Authors)
                .Include(b => b.Publisher)
                .ToListAsync();

            return books.Select(b => new BookDto
            {
                Id = b.Id,
                Title = b.Title,
                Authors = b.Authors.Select(a => new AuthorDto
                {
                    FirstName = a.FirstName,
                    LastName = a.LastName
                }).ToList(),
                Publisher = b.Publisher.Name,
                PublisherAddress = b.Publisher.Address,
                ISBN = b.ISBN
            }).ToList();
        }

        public async Task<string> UndoRequest(int userId, int bookId, bool removeWishlistItem = false)
        {
            var borrowRequest = await _context.BorrowRequests
                .FirstOrDefaultAsync(br => br.UserId == userId && br.BookId == bookId && br.Status == RequestStatus.Pending);

            if (borrowRequest == null)
            {
                throw new InvalidOperationException("No pending request found for the specified user and book.");
            }

            _context.BorrowRequests.Remove(borrowRequest);
            await _context.SaveChangesAsync();

            if (!removeWishlistItem)
            {
                var userWishlist = await _context.WishLists
                     .Include(w => w.Books)
                     .FirstOrDefaultAsync(w => w.UserId == userId);

                if (userWishlist != null)
                {
                    var book = await _context.Books.FindAsync(bookId);
                    if (book != null)
                    {
                        userWishlist.Books.Add(book);
                        await _context.SaveChangesAsync();
                    }
                }
            }

            return "Book request undone successfully.";
        }

        public async Task<List<RequestedBooksDto>> GetAllRequestedBooksAsync(GeneralQueryDto query)
        {
            var bookRequestsQuery = _context.BorrowRequests
                .Where(br => br.Status == RequestStatus.Pending)
                .Include(br => br.Book.Authors)
                .Include(br => br.User)
                .AsNoTracking()
                .AsQueryable();

            if(!string.IsNullOrEmpty(query.SearchTerm))
            {
                bookRequestsQuery = bookRequestsQuery.Where(br =>
                    br.Book.Title.Contains(query.SearchTerm) ||
                    br.Book.Authors.Any(a => a.FirstName.Contains(query.SearchTerm) || a.LastName.Contains(query.SearchTerm)) ||
                    br.User.FirstName.Contains(query.SearchTerm) ||
                    br.User.LastName.Contains(query.SearchTerm));
            }

            var queryCount = await bookRequestsQuery.CountAsync();

            bookRequestsQuery.OrderBy(br => br.RequestDate);

            var pageNumber = query.PageNumber <= 0 ? 1 : query.PageNumber;
            var pageSize = query.PageSize <= 0 ? 10 : query.PageSize;
            var skip = (pageNumber - 1) * pageSize;

            bookRequestsQuery = bookRequestsQuery.Skip(skip).Take(pageSize);
            
            var response = await bookRequestsQuery
                .Select(br => new RequestedBooksDto
                {
                    Id = br.Id,
                    Book = new BookDto
                    {
                        Id = br.Book.Id,
                        Title = br.Book.Title,
                        Authors = br.Book.Authors.Select(a => new AuthorDto
                        {
                            FirstName = a.FirstName,
                            LastName = a.LastName
                        }).ToList(),
                        Publisher = br.Book.Publisher.Name,
                        PublisherAddress = br.Book.Publisher.Address,
                        ISBN = br.Book.ISBN,
                        AvailableCopies = br.Book.AvailableCopies
                    },
                    User = new UserDto
                    {
                        Id = br.User.Id,
                        FirstName = br.User.FirstName,
                        LastName = br.User.LastName,
                        Email = br.User.Email
                    },
                    RequestDate = br.RequestDate,
                    Status = br.Status.ToString(),
                    TotalCount = queryCount
                })
                .ToListAsync();

            return response;
        }

        public Task<string> RejectBookRequest(int requestId)
        {
            var borrowRequest = _context.BorrowRequests.FirstOrDefault(br => br.Id == requestId);
            if (borrowRequest != null)
            {
                borrowRequest.Status = RequestStatus.Rejected;
                _context.SaveChanges();
                return Task.FromResult("Request status updated successfully.");
            }
            else
            {
                throw new InvalidOperationException("Borrow request not found.");
            }
        }
    }
}
