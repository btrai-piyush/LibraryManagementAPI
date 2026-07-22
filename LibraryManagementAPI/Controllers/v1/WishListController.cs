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
    [Authorize]
    public class WishListController : ControllerBase
    {
        private readonly IWishListService _wishListService;

        public WishListController(IWishListService wishListService)
        {
            _wishListService = wishListService;
        }

        [HttpGet]
        public async Task<IActionResult> GetWishList(int userId)
        {
            var books = await _wishListService.GetWishListBooksAsync(userId);
            return Ok(books);
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddToWishList(WishlistRequestDto request)
        {
            try
            {
                var result = await _wishListService.AddBookToWishListAsync(request);
                return Ok(result);

            } catch (Exception ex)
            {
                return new JsonResult(new { message = ex.Message }) { StatusCode = 400 };
            }

            
        }

        [HttpDelete("remove")]
        public async Task<IActionResult> RemoveFromWishList(WishlistRequestDto request)
        {
            try
            {
                var result = await _wishListService.RemoveBookFromWishListAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return new JsonResult(new { message = ex.Message }) { StatusCode = 400 };
            }
        }
    }
}
