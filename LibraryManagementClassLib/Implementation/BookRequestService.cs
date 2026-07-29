using LibraryManagementClassLib.Data;
using LibraryManagementClassLib.Dtos;
using LibraryManagementClassLib.Entities;
using LibraryManagementClassLib.Helpers;
using LibraryManagementClassLib.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
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
            var existingRequest = await _context.BorrowRequests.Where(br => br.UserId == userId && br.BookId == bookId && br.Status == RequestStatus.Pending).FirstOrDefaultAsync();
            
            if (existingRequest != null)
            {
                throw new Exception("Book already requested.");
            }

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
                    .Include(w=>w.User)
                    .FirstOrDefaultAsync(w => w.UserId == userId);

                if (wishlist != null)
                {
                    var book = wishlist.Books.FirstOrDefault(b => b.Id == bookId);

                    if (book != null)
                    {
                        wishlist.Books.Remove(book);

                        var activityMetadata = JsonSerializer.Serialize(new
                        {
                            BookTitle=book.Title,
                            UserName = $"{wishlist.User.FirstName} {wishlist.User.LastName}",
                        });

                        var activityLog = ActivityLogHelper.CreateActivity(userId, ActivityType.BookRequested, $"Requested book \"{book.Title}\"", activityMetadata);
                        await _context.ActivityLogs.AddAsync(activityLog);
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
                .Include(br => br.Book)
                .Include(br => br.User)
                .FirstOrDefaultAsync(br => br.UserId == userId && br.BookId == bookId && br.Status == RequestStatus.Pending);

            if (borrowRequest == null)
            {
                throw new InvalidOperationException("No pending request found for the specified user and book.");
            }

            _context.BorrowRequests.Remove(borrowRequest);

            var activityMetadata = JsonSerializer.Serialize(new
            {
                BookTitle = borrowRequest.Book.Title,
                UserName = $"{borrowRequest.User.FirstName} {borrowRequest.User.LastName}",
            });
            var activityLog = ActivityLogHelper.CreateActivity(userId, ActivityType.BookRequestCancelled, $"Undid request for \"{borrowRequest.Book.Title}\"", activityMetadata);
            await _context.ActivityLogs.AddAsync(activityLog);
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
            else
            {
                activityLog = ActivityLogHelper.CreateActivity(userId, ActivityType.BookUnwishlisted, $"Removed \"{borrowRequest.Book.Title}\" from wishlist", activityMetadata);
                await _context.ActivityLogs.AddAsync(activityLog);
                await _context.SaveChangesAsync();
            }

            return "Book request undone successfully.";
        }

        public async Task<List<RequestedBooksDto>> GetUserBookRequestsAsync(GeneralQueryDto query)
        {
            var bookRequestsQuery = _context.BorrowRequests
                .Where(br => br.Status == RequestStatus.Pending)
                .Include(br => br.Book.Authors)
                .Include(br => br.User)
                .AsNoTracking()
                .AsQueryable();

            var result = await CommonRequestQuery(query, bookRequestsQuery);
            return result;
        }

        public Task<string> RejectBookRequest(int requestId)
        {
            var borrowRequest = _context.BorrowRequests.Include(br=>br.User).Include(br => br.Book).FirstOrDefault(br => br.Id == requestId);
            if (borrowRequest != null)
            {
                borrowRequest.Status = RequestStatus.Rejected;

                var activityMetadata = JsonSerializer.Serialize(new
                {
                    BookTitle = borrowRequest.Book.Title,
                    UserName = $"{borrowRequest.User.FirstName} {borrowRequest.User.LastName}",
                });

                var activityLog = ActivityLogHelper.CreateActivity(borrowRequest.UserId, ActivityType.BookRequestRejected, $"Rejected request for \"{borrowRequest.Book.Title}\"", activityMetadata);
                _context.ActivityLogs.Add(activityLog);

                _context.SaveChanges();
                return Task.FromResult("Request status updated successfully.");
            }
            else
            {
                throw new InvalidOperationException("Borrow request not found.");
            }
        }

        public async Task<List<RequestedBooksDto>> GetBookRequestHistoryByUser(GeneralQueryDto query)
        {
            var userRequestsQuery = _context.BorrowRequests
                .Where(br => br.UserId == query.SearchId)
                .Include(br => br.Book.Authors)
                .Include(br => br.User)
                .AsNoTracking()
                .AsQueryable();

           var result = await CommonRequestQuery(query, userRequestsQuery);
            return result;
        }

        public async Task<List<RequestedBooksDto>> AdminGetPendingRequests(GeneralQueryDto query)
        {
            var pendingRequestsQuery = _context.BorrowRequests
                .Where(br => br.Status == RequestStatus.Pending)
                .Include(br => br.Book.Authors)
                .Include(br => br.User)
                .AsNoTracking()
                .AsQueryable();
            var result = await CommonRequestQuery(query, pendingRequestsQuery);
            return result;
        }

        public async Task<List<RequestedBooksDto>> AdminGetRequestHistory(GeneralQueryDto query)
        {
            var requestHistoryQuery = _context.BorrowRequests
                .Where(br => br.Status != RequestStatus.Pending)
                .Include(br => br.Book.Authors)
                .Include(br => br.User)
                .AsNoTracking()
                .AsQueryable();
            var result = await CommonRequestQuery(query, requestHistoryQuery);
            return result;
        }

        private async Task<List<RequestedBooksDto>> CommonRequestQuery(GeneralQueryDto query, IQueryable<BorrowRequest> requestsQuery)
        {
            if (!string.IsNullOrEmpty(query.SearchTerm))
            {
                requestsQuery = requestsQuery.Where(br =>
                    br.Book.Title.Contains(query.SearchTerm) ||
                    br.Book.Authors.Any(a => a.FirstName.Contains(query.SearchTerm) || 
                                             a.LastName.Contains(query.SearchTerm) ||
                                             (a.FirstName + " " + a.LastName).Contains(query.SearchTerm)) ||
                    br.User.FirstName.Contains(query.SearchTerm) ||
                    br.User.LastName.Contains(query.SearchTerm));
            }



            if(!string.IsNullOrEmpty(query.SortBy)) {
                switch(query.SortBy.ToLower())
                {
                    case "title":
                        requestsQuery = query.IsDescending ? requestsQuery.OrderByDescending(br => br.Book.Title) : requestsQuery.OrderBy(br => br.Book.Title);
                        break;
                    case "status":
                        requestsQuery = query.IsDescending ? requestsQuery.OrderByDescending(br => br.Status) : requestsQuery.OrderBy(br => br.Status);
                        break;
                    case "user":
                        requestsQuery = query.IsDescending ? requestsQuery.OrderByDescending(br => br.User.FirstName) : requestsQuery.OrderBy(br => br.User.FirstName);
                        break;
                    case "requestdate":
                        requestsQuery = query.IsDescending ? requestsQuery.OrderByDescending(br => br.RequestDate) : requestsQuery.OrderBy(br => br.RequestDate);
                        break;
                }
            }

            var queryCount = await requestsQuery.CountAsync();

            var pageNumber = query.PageNumber <= 0 ? 1 : query.PageNumber;
            var pageSize = query.PageSize <= 0 ? 10 : query.PageSize;
            var skip = (pageNumber - 1) * pageSize;
            requestsQuery = requestsQuery.Skip(skip).Take(pageSize);

            var response = await requestsQuery
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
                        AvailableCopies =br.Book.AvailableCopies,
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
    }
}
