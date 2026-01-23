using LibraryManagementClassLib.Dtos;
using LibraryManagementClassLib.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private readonly IBookService _bookService;

        public BookController(IBookService bookService)
        {
            _bookService = bookService;
        }

        [HttpPost("add-book")]
        public async Task<IActionResult> AddBook(AddBookDto request)
        {
            var result = await _bookService.AddBookAsync(request);
            return Ok(result);
        }
    }
}
