using LibraryManagementClassLib.Dtos;
using LibraryManagementClassLib.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementClassLib.Services
{
    public interface IBookService
    {
        Task<string> AddBookAsync(BookDto addBookDto);
        Task<List<BookDto>> SearchBookAsync(string searchTerm);
        Task<bool> UpdateBookAsync(int? bookId, BookDto bookDto);
    }
}
