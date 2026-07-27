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
        Task<List<BookDto>> AdminGetAllBooksAsync(BookQueryDto queryDto);
        Task<string> AddBookAsync(BookDto addBookDto);
        Task<AdminBookViewDto> AdminGetBookById(int bookId);
        Task<bool> UpdateBookAsync(int? bookId, BookDto bookDto);
        Task<bool> DeleteBookAsync(int? bookId);
        //Task<string> BulkAddBooksAsync(List<AddBookDto> books);
        Task<List<BookDto>> UserGetAllBooksAsync(BookQueryDto queryDto);
        Task<string> AddBooksAsync(List<AddBookDto> books);
    }
}
