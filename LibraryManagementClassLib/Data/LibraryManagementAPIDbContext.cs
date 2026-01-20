using LibraryManagementClassLib.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
