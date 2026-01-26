using Asp.Versioning;
using LibraryManagementClassLib.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementAPI.Controllers.v1
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    public class BookIssueController : ControllerBase
    {
        private readonly IBookIssueService _bookIssueService;

        public BookIssueController(IBookIssueService bookIssueService)
        {
            _bookIssueService = bookIssueService;
        }

        [HttpPost("borrow")]
        public async Task<IActionResult> BorrowBook(int userId, int bookId)
        {
            var result = await _bookIssueService.ConfirmBorrowAsync(userId, bookId);
            return Ok(result);
        }
    }
}
