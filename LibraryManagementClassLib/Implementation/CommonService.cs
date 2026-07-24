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
    public class CommonService : ICommonService
    {
        private readonly LibraryManagementAPIDbContext _context;

        public CommonService(LibraryManagementAPIDbContext context)
        {
            _context = context;
        }
        public async Task<UserDashboardDto> UserDashboard(int userId)
        {
            int wishlistItems = await _context.WishLists.CountAsync(w => w.UserId == userId);
            int requestedBooks = await _context.BorrowRequests.CountAsync(br => br.UserId == userId && br.Status == RequestStatus.Pending);

            var activeBookIssues = await _context.BookIssues.Include(bi => bi.Fine)
                                                            .Include(bi => bi.Book)
                                                            .Where(bi => bi.UserId == userId && bi.ReturnDate == null)
                                                            .ToListAsync();

            int activeBorrowings = activeBookIssues.Count();

            decimal fineAmount = 0;
            var unpaidFines = activeBookIssues.Where(bi => bi.Fine?.Status == Entities.PaidStatus.Unpaid);
            int unpaidFinesCount = unpaidFines.Count();
            foreach (var fine in unpaidFines)
            {
                fineAmount += fine.Fine?.Amount ?? 0;
            }

            var upcomingDueDates = activeBookIssues.Where(bi => bi.DueDate >= DateTime.Today)
                .OrderBy(bi => bi.DueDate)
                .Take(5)
                .Select(bi => new BookIssuesDto
                {
                    Book = new BookDto
                    {
                        Title = bi.Book.Title
                    },
                    DueDate = bi.DueDate
                }).ToList();

            var response = new UserDashboardDto
            {
                ActiveBorrowings = activeBorrowings,
                UnpaidFines = unpaidFinesCount,
                UnpaidFinesAmount = fineAmount,
                WishlistItems = wishlistItems,
                RequestedBooks = requestedBooks,
                UpcomingDueDates = upcomingDueDates,
                RecentActivity = await _context.ActivityLogs
                    .Where(al => al.UserId == userId)
                    .OrderByDescending(al => al.CreatedAt)
                    .Take(5)
                    .Select(al => new ActivityLogDto
                    {
                        UserId = al.UserId,
                        ActivityType = al.ActivityType.ToString(),
                        Description = al.Description,
                        MetaData = al.MetaData,
                        CreatedAt = al.CreatedAt,
                        ReferenceId = al.ReferenceId
                    })
                    .ToListAsync()
            };

            return response;
        }
    }
}
