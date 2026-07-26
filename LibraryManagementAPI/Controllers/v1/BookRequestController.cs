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
    //[Authorize]
    public class BookRequestController : ControllerBase
    {
        private readonly IBookRequestService _bookRequestService;

        public BookRequestController(IBookRequestService bookRequestService)
        {
            _bookRequestService = bookRequestService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateBookRequest(WishlistRequestDto request)
        {
            return Ok(await _bookRequestService.RequestBookAsync(request.UserId, request.BookId));
        }

        [HttpGet]
        public async Task<IActionResult> GetRequestedBooks(int userId)
        {
            var requestedBooks = await _bookRequestService.GetRequestedBooksAsync(userId);
            return Ok(requestedBooks);
        }

        [HttpPost("user/pending-requests")]
        public async Task<IActionResult> GetUserBookRequests(GeneralQueryDto query)
        {
            var requestedBooks = await _bookRequestService.GetUserBookRequestsAsync(query);
            return Ok(requestedBooks);
        }

        [HttpGet("reject")]
        public async Task<IActionResult> RejectBookRequest(int requestId)
        {
            try
            {
                var result = await _bookRequestService.RejectBookRequest(requestId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("undo")]
        public async Task<IActionResult> UndoBookRequest(UndoBookRequestDto request)
        {
            var result = await _bookRequestService.UndoRequest(request.UserId, request.BookId, request.RemoveFromWishlist);
            return Ok(result);
        }

        [HttpPost("user-history")]
        public async Task<IActionResult> GetBookRequestHistory(GeneralQueryDto query)
        {
            var history = await _bookRequestService.GetBookRequestHistoryByUser(query);
            return Ok(history);
        }

        [HttpPost("admin/pending-requests")]
        public async Task<IActionResult> AdminGetPendingRequests(GeneralQueryDto query)
        {
            var pendingRequests = await _bookRequestService.AdminGetPendingRequests(query);
            return Ok(pendingRequests);
        }

        [HttpPost("admin/request-history")]
        public async Task<IActionResult> AdminGetRequestHistory(GeneralQueryDto query)
        {
            var requestHistory = await _bookRequestService.AdminGetRequestHistory(query);
            return Ok(requestHistory);
        }
    }
}
