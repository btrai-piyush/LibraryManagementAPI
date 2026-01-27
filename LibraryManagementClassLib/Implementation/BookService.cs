using Azure.Core;
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

namespace LibraryManagementClassLib.Implementation;

public class BookService : IBookService
{
    private readonly LibraryManagementAPIDbContext _context;

    public BookService(LibraryManagementAPIDbContext context)
    {
        _context = context;
    }

    public async Task<List<BookDto>> GetAllBooksAsync(BookQueryDto queryDto)
    {
        var booksQuery = _context.Books.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(queryDto.SearchTerm))
        {
            booksQuery = booksQuery.Where(b => b.Title.ToLower().Contains(queryDto.SearchTerm) ||
                                               b.ISBN.ToLower().Contains(queryDto.SearchTerm) ||
                                               b.Categories.Any(c => c.Name.ToLower().Contains(queryDto.SearchTerm)) ||
                                               b.Authors.Any(ba => ba.FirstName.ToLower().Contains(queryDto.SearchTerm) || ba.LastName.ToLower().Contains(queryDto.SearchTerm)));
        }

        if (!string.IsNullOrWhiteSpace(queryDto.SortBy))
        {
            if (queryDto.SortBy.Equals("title", StringComparison.OrdinalIgnoreCase))
            {
                booksQuery = queryDto.IsDescending
                    ? booksQuery.OrderByDescending(b => b.Title)
                    : booksQuery.OrderBy(b => b.Title);
            }
            if (queryDto.SortBy.Equals("isbn", StringComparison.OrdinalIgnoreCase))
            {
                booksQuery = queryDto.IsDescending
                    ? booksQuery.OrderByDescending(b => b.ISBN)
                    : booksQuery.OrderBy(b => b.ISBN);
            }
            if (queryDto.SortBy.Equals("availableCopies", StringComparison.OrdinalIgnoreCase))
            {
                booksQuery = queryDto.IsDescending
                    ? booksQuery.OrderByDescending(b => b.AvailableCopies)
                    : booksQuery.OrderBy(b => b.AvailableCopies);
            }


        }

        var pageNumber = queryDto.PageNumber <= 0 ? 1 : queryDto.PageNumber;
        var pageSize = queryDto.PageSize <= 0 ? 10 : queryDto.PageSize;

        var skipNumber = (pageNumber - 1) * pageSize;
        booksQuery = booksQuery.Skip(skipNumber).Take(pageSize);

        var books = await booksQuery
            .Select(b => new BookDto
            {
                Id = b.Id,
                Title = b.Title,
                ISBN = b.ISBN,
                Copies = b.AvailableCopies,
                Authors = b.Authors.Select(a => new AuthorDto
                {
                    FirstName = a.FirstName,
                    LastName = a.LastName
                }).ToList(),
                Categories = b.Categories.Select(c => c.Name).ToList(),
                Publisher = b.Publisher.Name,
                PublisherAddress = b.Publisher.Address
            })
            .ToListAsync();
        return books;
    }

    public async Task<string> AddBookAsync(BookDto request)
    {
        var bookExists = await _context.Books.Where(b => b.ISBN == request.ISBN).FirstOrDefaultAsync();
        if (bookExists == null)
        {
            var book = new Book
            {
                Title = request.Title,
                ISBN = request.ISBN,
                TotalCopies = request.Copies,
                AvailableCopies = request.Copies
            };
            foreach (var requestAuthor in request.Authors)
            {
                var author = await HandleAuthorAsync(requestAuthor);
                book.Authors.Add(author);
            }

            foreach (var requestCategory in request.Categories)
            {
                var category = await HandleCategoryAsync(requestCategory);
                book.Categories.Add(category);
            }

            var publisher = await HandlePublisherAsync(request.Publisher, request.PublisherAddress);
            book.Publisher = publisher;

            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            return "New book was added";
        }
        else if (bookExists != null)
        {
            bookExists.TotalCopies += request.Copies;
            bookExists.AvailableCopies += request.Copies;
            await _context.SaveChangesAsync();
            return $"{request.Copies} copies of {request.Title} was added to inventory. New available copies: {bookExists.AvailableCopies}";
        }

        return "An error occurred while adding the book";
    }

    public async Task<bool> UpdateBookAsync(int? bookId, BookDto bookDto)
    {
        var existingBook = await _context.Books.FindAsync(bookId);
        if (existingBook == null)
        {
            return false;
        }
        existingBook.Title = bookDto.Title;
        existingBook.ISBN = bookDto.ISBN;
        existingBook.TotalCopies = bookDto.Copies;
        existingBook.AvailableCopies = bookDto.Copies;
        _context.Books.Update(existingBook);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteBookAsync(int? bookId)
    {
        var existingBook = await _context.Books.FindAsync(bookId);
        if (existingBook == null)
        {
            return false;
        }
        _context.Books.Remove(existingBook);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<BookDto> GetBookById(int bookId)
    {
        var availableBook = await _context.Books
            .Where(b => b.Id == bookId)
            .Select(b => new BookDto
            {
                Id = b.Id,
                Title = b.Title,
                ISBN = b.ISBN,
                Copies = b.AvailableCopies,
                Authors = b.Authors.Select(a => new AuthorDto
                {
                    FirstName = a.FirstName,
                    LastName = a.LastName
                }).ToList(),
                Categories = b.Categories.Select(c => c.Name).ToList(),
                Publisher = b.Publisher.Name,
                PublisherAddress = b.Publisher.Address
            })
            .FirstOrDefaultAsync();
        if (availableBook == null)
        {
            return null;
        }
        return availableBook;
    }

    private async Task<Author> HandleAuthorAsync(AuthorDto authorDto)
    {
        var author = await _context.Authors
            .FirstOrDefaultAsync(a => a.FirstName == authorDto.FirstName && a.LastName == authorDto.LastName);
        if (author == null)
        {
            author = new Author
            {
                FirstName = authorDto.FirstName,
                LastName = authorDto.LastName
            };
            _context.Authors.Add(author);
        }
        return author;
    }

    private async Task<Category> HandleCategoryAsync(string categoryName)
    {
        var category = await _context.Categories
            .FirstOrDefaultAsync(c => c.Name == categoryName);
        if (category == null)
        {
            category = new Category
            {
                Name = categoryName
            };
            _context.Categories.Add(category);
        }
        return category;
    }

    private async Task<Publisher> HandlePublisherAsync(string publisherName, string publisherAddress)
    {
        var publisher = await _context.Publishers
            .FirstOrDefaultAsync(p => p.Name == publisherName && p.Address == publisherAddress);
        if (publisher == null)
        {
            publisher = new Publisher
            {
                Name = publisherName,
                Address = publisherAddress
            };
            _context.Publishers.Add(publisher);
        }
        return publisher;
    }

    public async Task<string> BulkAddBooksAsync(List<BookDto> bookDtos)
    {
        foreach (var bookDto in bookDtos)
        {
            await AddBookAsync(bookDto);
        }
        return "Bulk insert completed successfully";
    }
}

