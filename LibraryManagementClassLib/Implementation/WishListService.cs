using LibraryManagementClassLib.Data;
using LibraryManagementClassLib.Dtos;
using LibraryManagementClassLib.Entities;
using LibraryManagementClassLib.Helpers;
using LibraryManagementClassLib.Migrations;
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
    public class WishListService : IWishListService
    {
        private readonly LibraryManagementAPIDbContext _context;

        public WishListService(LibraryManagementAPIDbContext context)
        {
            _context = context;
        }

        public async Task<string> AddBookToWishListAsync(WishlistRequestDto request)
        {
            var wishList = await _context.WishLists.Include(w => w.Books).FirstOrDefaultAsync(w => w.UserId == request.UserId);
            if (wishList == null)
            {
                wishList = new WishList { UserId = request.UserId };
                _context.WishLists.Add(wishList);
            }

            var book = await _context.Books.FindAsync(request.BookId);

            if(book == null)
            {
                throw new Exception("Server error");
            }

            var existingRequest = await _context.BorrowRequests
                .FirstOrDefaultAsync(br => br.UserId == request.UserId && br.BookId == request.BookId && br.Status == RequestStatus.Pending);

            if(existingRequest != null)
            {
                throw new Exception("Book already requested for borrowing.");
            }

            if (wishList.Books.Contains(book))
            {
                throw new Exception("Already added to wish list.");
            }

            var activityMetadata = JsonSerializer.Serialize(new
            {
                BookTitle = book.Title
            });
            var activityLog = ActivityLogHelper.CreateActivity(request.UserId, ActivityType.BookWishlisted, $"Wishlisted \"{book.Title}\"", activityMetadata);

            _context.ActivityLogs.Add(activityLog);
            wishList.Books.Add(book);


            await _context.SaveChangesAsync();
            return "Book added to wish list successfully.";
        }

        public async Task<List<BookDto>> GetWishListBooksAsync(int userId)
        {
            var books = await _context.WishLists
                .Where(w => w.UserId == userId)
                .SelectMany(w => w.Books)
                .Select(b => new BookDto
                {
                    Id = b.Id,
                    Title = b.Title,
                    ISBN = b.ISBN,
                    AvailableCopies = b.AvailableCopies,
                    Publisher = b.Publisher.Name,
                    PublisherAddress = b.Publisher.Address,
                    Authors = b.Authors.Select(a => new AuthorDto
                    {
                        FirstName = a.FirstName,
                        LastName = a.LastName
                    }).ToList(),
                    Categories = b.Categories.Select(c => c.Name).ToList()
                })
                .ToListAsync();

            return books;
        }

        public async Task<string> RemoveBookFromWishListAsync(WishlistRequestDto request)
        {
            var wishList = await _context.WishLists.Include(w => w.Books).FirstOrDefaultAsync(w => w.UserId == request.UserId);
            if (wishList == null) return "Wish list not found.";

            var book = wishList.Books.FirstOrDefault(b => b.Id == request.BookId);
            if (book == null) return "Book not found in wish list.";

            var activityMetadata = JsonSerializer.Serialize(new
            {
                BookTitle = book.Title
            });
            var activityLog = ActivityLogHelper.CreateActivity(request.UserId, ActivityType.BookUnwishlisted, $"Removed \"{book.Title}\" from wish list", activityMetadata);
            wishList.Books.Remove(book);
            await _context.SaveChangesAsync();
            return "Book removed from wish list successfully.";
        }
    }
}
