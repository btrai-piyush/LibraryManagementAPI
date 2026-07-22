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

        [HttpPost("get-all")]
        public async Task<IActionResult> GetAllRequestedBooks(GeneralQueryDto query)
        {
            var requestedBooks = await _bookRequestService.GetAllRequestedBooksAsync(query);
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
    }
}
