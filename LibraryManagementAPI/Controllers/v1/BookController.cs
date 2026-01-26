using Asp.Versioning;
using LibraryManagementClassLib.Dtos;
using LibraryManagementClassLib.Services;
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

        [HttpPost("add-book")]
        public async Task<IActionResult> AddBook(BookDto request)
        {
            var result = await _bookService.AddBookAsync(request);
            return Ok(result);
        }

        [HttpGet("get-book")]
        public async Task<IActionResult> GetBookBySearchTerm([FromQuery] string searchTerm)
        {
            var result = await _bookService.SearchBookAsync(searchTerm);
            return Ok(result);
        }

        [HttpPut("update-book/{bookId}")]
        public async Task<IActionResult> UpdateBook(int bookId, BookDto request)
        {
            var result = await _bookService.UpdateBookAsync(bookId, request);
            return Ok(result);
        }
    }
}
