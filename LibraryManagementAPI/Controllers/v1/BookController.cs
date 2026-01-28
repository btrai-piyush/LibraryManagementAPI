using Asp.Versioning;
using LibraryManagementClassLib.Dtos;
using LibraryManagementClassLib.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementAPI.Controllers.v1
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]

    public class BookController : ControllerBase
    {
        private readonly IBookService _bookService;

        public BookController(IBookService bookService)
        {
            _bookService = bookService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] BookQueryDto query)
        {
            query.SearchTerm = query.SearchTerm?.Trim().ToLower();
            var books = await _bookService.GetAllBooksAsync(query);
            return Ok(books);
        }

        [HttpPost("add-book")]
        [Authorize(Roles = "Librarian")]
        public async Task<IActionResult> AddBook(BookDto request)
        {
            var result = await _bookService.AddBookAsync(request);
            return Ok(result);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetBookById(int id)
        {
            var result = await _bookService.GetBookById(id);
            return Ok(result);
        }

        [HttpPut("update-book/{bookId}")]
        [Authorize(Roles = "Librarian")]
        public async Task<IActionResult> UpdateBook(int bookId, BookDto request)
        {
            var result = await _bookService.UpdateBookAsync(bookId, request);
            return Ok(result);
        }

        [HttpDelete("delete-book/{bookId}")]
        [Authorize(Roles = "Librarian")]
        public async Task<IActionResult> DeleteBook(int bookId)
        {
            var result = await _bookService.DeleteBookAsync(bookId);
            return Ok(result);
        }

        [HttpPost("bulk-add")]
        [Authorize(Roles = "Librarian")]
        public async Task<IActionResult> BulkAddBooks(List<BookDto> books)
        {
            var result = await _bookService.BulkAddBooksAsync(books);
            return Ok(result);
        }
    }
}
