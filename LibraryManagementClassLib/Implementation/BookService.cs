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
    public class BookService : IBookService
    {
        private readonly LibraryManagementAPIDbContext _context;

        public BookService(LibraryManagementAPIDbContext _context)
        {
            this._context = _context;
        }

        public async Task<bool> AddBookAsync(AddBookDto request)
        {
            var bookExists = await _context.Books.Where(b => b.Title == request.Title).FirstOrDefaultAsync();
            if (bookExists == null)
            {
                await _context.Books.AddAsync(
                    new Book
                    {
                        Title = request.Title,
                        ISBN = request.Title,
                        TotalCopies = request.Copies
                    });

                var authorExists = await _context.Authors.Where(
                    a => a.FirstName == request.AuthorFirstName && a.LastName == request.AuthorLastName)
                    .FirstOrDefaultAsync();
                if (authorExists == null)
                {
                    await _context.Authors.AddAsync(new Author
                    {
                        FirstName = request.AuthorFirstName,
                        LastName = request.AuthorLastName,
                    });
                }

            }
            return true;
        }
    }
}
