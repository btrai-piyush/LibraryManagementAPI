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

    public BookService(LibraryManagementAPIDbContext _context)
    {
        this._context = _context;
    }

    public async Task<string> AddBookAsync(AddBookDto request)
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
}
