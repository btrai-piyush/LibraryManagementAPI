using LibraryManagementClassLib.Services;
using LibraryManagementClassLib.Implementation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementAPI.Controllers.v1
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class GoogleBooksController : ControllerBase
    {
        private readonly IGoogleBooksService _googleBooksService;

        public GoogleBooksController(IGoogleBooksService googleBooksService)
        {
            _googleBooksService = googleBooksService;
        }

        [HttpGet("lookup/{isbn}")]
        public async Task<IActionResult> GetBookByIsbn(string isbn)
        {
            var result = await _googleBooksService.GetBookByIsbnAsync(isbn);

            return Ok(result);
        }
    }
}
