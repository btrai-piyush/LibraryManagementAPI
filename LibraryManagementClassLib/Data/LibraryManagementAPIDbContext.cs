using LibraryManagementClassLib.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Text.Json;

namespace LibraryManagementClassLib.Data
{
    public class LibraryManagementAPIDbContext : DbContext
    {
        public LibraryManagementAPIDbContext(DbContextOptions<LibraryManagementAPIDbContext> options) : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<Publisher> Publishers => Set<Publisher>();
        public DbSet<Author> Authors => Set<Author>();
        public DbSet<Book> Books => Set<Book>();
        public DbSet<BookIssue> BookIssues => Set<BookIssue>();
        public DbSet<Fine> Fines => Set<Fine>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<BorrowRequest> BorrowRequests => Set<BorrowRequest>();
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
        public DbSet<Course> Courses => Set<Course>();
        public DbSet<Subject> Subjects => Set<Subject>();
        public DbSet<WishList> WishLists => Set<WishList>();
        public DbSet<ActivityLog> ActivityLogs => Set<ActivityLog>();
    }
}


