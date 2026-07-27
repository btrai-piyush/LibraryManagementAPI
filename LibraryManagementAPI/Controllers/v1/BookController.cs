using Asp.Versioning;
using LibraryManagementClassLib.Dtos;
using LibraryManagementClassLib.Services;
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

        [HttpGet("get-all")]
        public async Task<IActionResult> AdminGetAll([FromQuery] BookQueryDto query)
        {
            query.SearchTerm = query.SearchTerm?.Trim().ToLower();
            var books = await _bookService.AdminGetAllBooksAsync(query);
            return Ok(books);
        }

        [HttpGet("get-all-user")]
        public async Task<IActionResult> UserGetAll([FromQuery] BookQueryDto query)
        {
            query.SearchTerm = query.SearchTerm?.Trim().ToLower();
            var books = await _bookService.UserGetAllBooksAsync(query);
            return Ok(books);
        }

        [HttpPost]
        //[Authorize(Roles = "Librarian")]
        public async Task<IActionResult> AddBook(BookDto request)
        {
            var result = await _bookService.AddBookAsync(request);
            return Ok(result);
        }

        [HttpGet("{id}")]
        //[Authorize]
        public async Task<IActionResult> AdminGetBookById(int id)
        {
            var result = await _bookService.AdminGetBookById(id);
            return Ok(result);
        }

        [HttpPut("update-book/{bookId}")]
        //[Authorize(Roles = "Librarian")]
        public async Task<IActionResult> UpdateBook(int bookId, BookDto request)
        {
            var result = await _bookService.UpdateBookAsync(bookId, request);
            return Ok(result);
        }

        [HttpDelete("delete-book/{bookId}")]
        //[Authorize(Roles = "Librarian")]
        public async Task<IActionResult> DeleteBook(int bookId)
        {
            var result = await _bookService.DeleteBookAsync(bookId);
            return Ok(result);
        }

        //[HttpPost("bulk-add")]
        ////[Authorize(Roles = "Librarian")]
        //public async Task<IActionResult> BulkAddBooks(List<AddBookDto> books)
        //{
        //    var result = await _bookService.BulkAddBooksAsync(books);
        //    return Ok(result);
        //}

        [HttpPost("add-books")]
        //[Authorize(Roles = "Librarian")]
        public async Task<IActionResult> AddBooks(List<AddBookDto> books)
        {
            var result = await _bookService.AddBooksAsync(books);
            return Ok(result);
        }
    }
}
