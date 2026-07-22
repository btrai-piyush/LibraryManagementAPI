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
    //[Authorize(Roles = "admin")]
    public class BookIssueController : ControllerBase
    {
        private readonly IBookIssueService _bookIssueService;

        public BookIssueController(IBookIssueService bookIssueService)
        {
            _bookIssueService = bookIssueService;
        }

        [HttpPatch("update-status")]
        public async Task<IActionResult> UpdateBookIssueStatus()
        {
            try
            {
                await _bookIssueService.UpdateBookIssueStatus();
                return Ok("Book issue status updated successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("issue")]
        public async Task<IActionResult> IssueBook([FromBody] IssueBookDto issueBookDto)
        {
            try
            {
                var result = await _bookIssueService.IssueBookAsync(issueBookDto.RequestId, issueBookDto.DueDate);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("get-all")]
        public async Task<IActionResult> GetAllBookIssues(GeneralQueryDto query)
        {
            try
            {
                var result = await _bookIssueService.GetAllBookIssuesAsync(query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPatch("return")]
        public async Task<IActionResult> ReturnBook(int issueId)
        {
            try
            {
                var result = await _bookIssueService.ReturnBookAsync(issueId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetBorrowedBooks(int userId)
        {
            try
            {
                var result = await _bookIssueService.GetBookIssuesByUserIdAsync(userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


    }
}
