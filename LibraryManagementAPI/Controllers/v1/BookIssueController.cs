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

        [HttpPost("user/active")]
        public async Task<IActionResult> GetActiveBookIssuesByUserId(GeneralQueryDto query)
        {
            try
            {
                var result = await _bookIssueService.GetActiveBookIssuesByUserIdAsync(query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("user/history")]
        public async Task<IActionResult> GetBookIssuesHistoryByUserId(GeneralQueryDto query)
        {
            try
            {
                var result = await _bookIssueService.GetBookIssuesHistoryByUserIdAsync(query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("admin/active")]
        public async Task<IActionResult> AdminGetActiveIssues(GeneralQueryDto query)
        {
            try
            {
                var result = await _bookIssueService.AdminGetActiveIssues(query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("admin/history")]
        public async Task<IActionResult> AdminGetIssuesHistory(GeneralQueryDto query)
        {
            try
            {
                var result = await _bookIssueService.AdminGetIssuesHistory(query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
