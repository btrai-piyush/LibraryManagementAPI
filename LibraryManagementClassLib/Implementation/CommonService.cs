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

        private readonly IRecommendationService _recommendationService;
        public CommonService(LibraryManagementAPIDbContext context, IRecommendationService recommendationService)
        {
            _context = context;
            _recommendationService = recommendationService;
        }

        public async Task<AdminDashboardDto> AdminDashboard()
        {
            int totalBooks = await _context.Books.CountAsync();
            int totalUsers = await _context.Users.CountAsync();
            int activeBorrowings = await _context.BookIssues.CountAsync(bi => bi.ReturnDate == null);
            int pendingRequests = await _context.BorrowRequests.CountAsync(br => br.Status == RequestStatus.Pending);
            int unpaidFines = await _context.Fines.CountAsync(f => f.Status == Entities.PaidStatus.Unpaid);

            var recentActivity = await _context.ActivityLogs
                .Where(al => al.ActivityType == ActivityType.BookIssued
                             || al.ActivityType==ActivityType.BookRequested
                             || al.ActivityType == ActivityType.BookReturned
                             || al.ActivityType == ActivityType.FinePaid
                             || al.ActivityType == ActivityType.BookRequestCancelled
                             || al.ActivityType == ActivityType.BookRequestRejected
                             )
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
                .ToListAsync();

            var users= await _context.Users.Where(u => recentActivity.Select(ra => ra.UserId).Contains(u.Id)).ToListAsync();

            foreach (var activity in recentActivity)
            {
                var activityUser = users.FirstOrDefault(u => u.Id == activity.UserId);
                activity.Description = GetAdminActivityDescription(activity.ActivityType,activity.MetaData);
            }

            return new AdminDashboardDto
            {
                TotalBooks = totalBooks,
                TotalUsers = totalUsers,
                ActiveBorrowings = activeBorrowings,
                PendingRequests = pendingRequests,
                UnpaidFines = unpaidFines,
                RecentActivity = recentActivity
            };
        }

        public async Task<UserDashboardDto> UserDashboard(int userId)
        {
            int wishlistItems = await _context.WishLists.Include(w => w.Books).Where(w => w.UserId == userId).SelectMany(w => w.Books).CountAsync();
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

            response.RecommendedBooks = await _recommendationService.GetUserRecommendations(userId);
            response.RecommendedBooks = response.RecommendedBooks.Take(5).ToList();

            return response;
        }

        private string GetAdminActivityDescription(string activityType,string? metaData)
        {
            var serializedMetaData = string.IsNullOrEmpty(metaData) ? null : System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(metaData);
            switch (activityType)
            {
                case "BookIssued":
                    return $"\"{serializedMetaData?["BookTitle"]}\" issued to {serializedMetaData?["UserName"]}";
                case "BookRequested":
                    return $"\"{serializedMetaData?["BookTitle"]}\" requested by {serializedMetaData?["UserName"]}";
                case "BookReturned":
                   return $"\"{serializedMetaData?["BookTitle"]}\" returned by {serializedMetaData?["UserName"]}";
                case "FinePaid":
                    return $"\"{serializedMetaData?["UserName"] }\" paid रु {serializedMetaData?["FineAmount"]} fine.";
                case "BookRequestCancelled":
                    return $"Request for \"{serializedMetaData?["BookTitle"]}\" cancelled by {serializedMetaData?["UserName"]}";
                case "BookRequestRejected":
                    return $"Request for \"{serializedMetaData?["BookTitle"]}\" by {serializedMetaData?["UserName"]} rejected";
                default:
                    return $"No description available for activity type: {activityType}";
            }
        }
    }
}
