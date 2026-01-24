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

        // In your DbContext class (e.g., LibraryDbContext.cs)
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Seed Publishers
            modelBuilder.Entity<Publisher>().HasData(
                new Publisher { Id = 1, Name = "Penguin Random House", Address = "1745 Broadway, New York, NY 10019" },
                new Publisher { Id = 2, Name = "HarperCollins", Address = "195 Broadway, New York, NY 10007" },
                new Publisher { Id = 3, Name = "Simon & Schuster", Address = "1230 Avenue of the Americas, New York, NY 10020" },
                new Publisher { Id = 4, Name = "Macmillan Publishers", Address = "120 Broadway, New York, NY 10271" },
                new Publisher { Id = 5, Name = "Hachette Book Group", Address = "1290 Avenue of the Americas, New York, NY 10104" }
            );

            // Seed Authors
            modelBuilder.Entity<Author>().HasData(
                new Author { Id = 1, FirstName = "George", LastName = "Orwell" },
                new Author { Id = 2, FirstName = "Harper", LastName = "Lee" },
                new Author { Id = 3, FirstName = "F. Scott", LastName = "Fitzgerald" },
                new Author { Id = 4, FirstName = "Jane", LastName = "Austen" },
                new Author { Id = 5, FirstName = "J.K.", LastName = "Rowling" },
                new Author { Id = 6, FirstName = "J.R.R.", LastName = "Tolkien" },
                new Author { Id = 7, FirstName = "Stephen", LastName = "King" },
                new Author { Id = 8, FirstName = "Agatha", LastName = "Christie" },
                new Author { Id = 9, FirstName = "Ernest", LastName = "Hemingway" },
                new Author { Id = 10, FirstName = "Gabriel", LastName = "García Márquez" }
            );

            // Seed Categories
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Fiction" },
                new Category { Id = 2, Name = "Non-Fiction" },
                new Category { Id = 3, Name = "Science Fiction" },
                new Category { Id = 4, Name = "Fantasy" },
                new Category { Id = 5, Name = "Mystery" },
                new Category { Id = 6, Name = "Romance" },
                new Category { Id = 7, Name = "Biography" },
                new Category { Id = 8, Name = "History" },
                new Category { Id = 9, Name = "Science" },
                new Category { Id = 10, Name = "Children" }
            );

            // Seed Books
            modelBuilder.Entity<Book>().HasData(
                new Book
                {
                    Id = 1,
                    Title = "1984",
                    ISBN = "978-0451524935",
                    TotalCopies = 10,
                    AvailableCopies = 8,
                    PublisherId = 1
                },
                new Book
                {
                    Id = 2,
                    Title = "To Kill a Mockingbird",
                    ISBN = "978-0446310789",
                    TotalCopies = 8,
                    AvailableCopies = 6,
                    PublisherId = 2
                },
                new Book
                {
                    Id = 3,
                    Title = "The Great Gatsby",
                    ISBN = "978-0743273565",
                    TotalCopies = 12,
                    AvailableCopies = 10,
                    PublisherId = 3
                },
                new Book
                {
                    Id = 4,
                    Title = "Pride and Prejudice",
                    ISBN = "978-1503290563",
                    TotalCopies = 15,
                    AvailableCopies = 12,
                    PublisherId = 4
                },
                new Book
                {
                    Id = 5,
                    Title = "Harry Potter and the Sorcerer's Stone",
                    ISBN = "978-0439708180",
                    TotalCopies = 20,
                    AvailableCopies = 15,
                    PublisherId = 5
                },
                new Book
                {
                    Id = 6,
                    Title = "The Hobbit",
                    ISBN = "978-0547928227",
                    TotalCopies = 10,
                    AvailableCopies = 7,
                    PublisherId = 1
                },
                new Book
                {
                    Id = 7,
                    Title = "The Shining",
                    ISBN = "978-0307743657",
                    TotalCopies = 8,
                    AvailableCopies = 5,
                    PublisherId = 2
                },
                new Book
                {
                    Id = 8,
                    Title = "Murder on the Orient Express",
                    ISBN = "978-0062073501",
                    TotalCopies = 6,
                    AvailableCopies = 4,
                    PublisherId = 3
                },
                new Book
                {
                    Id = 9,
                    Title = "The Old Man and the Sea",
                    ISBN = "978-0684801223",
                    TotalCopies = 7,
                    AvailableCopies = 6,
                    PublisherId = 4
                },
                new Book
                {
                    Id = 10,
                    Title = "One Hundred Years of Solitude",
                    ISBN = "978-0060883287",
                    TotalCopies = 9,
                    AvailableCopies = 7,
                    PublisherId = 5
                }
            );

            // Seed Many-to-Many relationships for Book-Author
            // EF Core requires a join entity for many-to-many relationships with seeded data
            modelBuilder.Entity<Book>()
                .HasMany(b => b.Authors)
                .WithMany(a => a.Books)
                .UsingEntity<Dictionary<string, object>>(
                    "AuthorBook",
                    r => r.HasOne<Author>().WithMany().HasForeignKey("AuthorsId"),
                    l => l.HasOne<Book>().WithMany().HasForeignKey("BooksId"),
                    je =>
                    {
                        je.HasKey("BooksId", "AuthorsId");
                        je.HasData(
                            new { BooksId = 1, AuthorsId = 1 },
                            new { BooksId = 2, AuthorsId = 2 },
                            new { BooksId = 3, AuthorsId = 3 },
                            new { BooksId = 4, AuthorsId = 4 },
                            new { BooksId = 5, AuthorsId = 5 },
                            new { BooksId = 6, AuthorsId = 6 },
                            new { BooksId = 7, AuthorsId = 7 },
                            new { BooksId = 8, AuthorsId = 8 },
                            new { BooksId = 9, AuthorsId = 9 },
                            new { BooksId = 10, AuthorsId = 10 }
                        );
                    });

            // Seed Many-to-Many relationships for Book-Category
            modelBuilder.Entity<Book>()
                .HasMany(b => b.Categories)
                .WithMany(c => c.Books)
                .UsingEntity<Dictionary<string, object>>(
                    "BookCategory",
                    r => r.HasOne<Category>().WithMany().HasForeignKey("CategoriesId"),
                    l => l.HasOne<Book>().WithMany().HasForeignKey("BooksId"),
                    je =>
                    {
                        je.HasKey("BooksId", "CategoriesId");
                        je.HasData(
                            new { BooksId = 1, CategoriesId = 1 },
                            new { BooksId = 1, CategoriesId = 3 },
                            new { BooksId = 2, CategoriesId = 1 },
                            new { BooksId = 3, CategoriesId = 1 },
                            new { BooksId = 4, CategoriesId = 1 },
                            new { BooksId = 4, CategoriesId = 6 },
                            new { BooksId = 5, CategoriesId = 4 },
                            new { BooksId = 5, CategoriesId = 10 },
                            new { BooksId = 6, CategoriesId = 4 },
                            new { BooksId = 7, CategoriesId = 1 },
                            new { BooksId = 7, CategoriesId = 5 },
                            new { BooksId = 8, CategoriesId = 5 },
                            new { BooksId = 9, CategoriesId = 1 },
                            new { BooksId = 10, CategoriesId = 1 }
                        );
                    });
        }
    }
}


