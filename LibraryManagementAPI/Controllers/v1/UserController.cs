using Asp.Versioning;
using LibraryManagementClassLib.Dtos;
using LibraryManagementClassLib.Implementation;
using LibraryManagementClassLib.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementAPI.Controllers.v1
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [Authorize(Roles = "Librarian")]
        [HttpGet("librarian/get-all")]
        public async Task<ActionResult<List<UserResponseDto>>> GetAll()
        {
            var response = await _userService.GetAllUsersAsync();
            return Ok(response);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<UserResponseDto>> GetById(int id)
        {
            var response = await _userService.GetUserByIdAsync(id);
            if (response == null)
            {
                return NotFound();
            }
            return Ok(response);
        }
    }
}
