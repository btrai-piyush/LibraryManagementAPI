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
        Task<List<BookDto>> GetAllBooksAsync(BookQueryDto queryDto);
        Task<string> AddBookAsync(BookDto addBookDto);
        Task<BookDto> GetBookById(int bookId);
        Task<bool> UpdateBookAsync(int? bookId, BookDto bookDto);
        Task<bool> DeleteBookAsync(int? bookId);
        Task<string> BulkAddBooksAsync(List<BookDto> books);
    }
}
