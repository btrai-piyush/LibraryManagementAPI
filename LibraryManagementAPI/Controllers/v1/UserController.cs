using Asp.Versioning;
using LibraryManagementClassLib.Dtos;
using LibraryManagementClassLib.Implementation;
using LibraryManagementClassLib.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LibraryManagementAPI.Controllers.v1
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    //[Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [Authorize(Roles = "Librarian")]
        [HttpGet]
        public async Task<ActionResult<List<UserResponseDto>>> GetAllStudents(StudentQueryDto queryDto)
        {
            var response = await _userService.GetAllStudentsAsync(queryDto);
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

        [HttpGet("{email}")]
        public async Task<ActionResult<UserResponseDto>> GetByEmail(string email)
        {
            try
            {
                var response = await _userService.GetByEmailAsync(email);
                return Ok(response);
            }
            catch (Exception e)
            {
                return NotFound(e.Message);
            }
        }

        [HttpGet("student-details/{studentId}")]
        public async Task<ActionResult<UserResponseDto>> GetStudentDetails([FromRoute] int studentId)
        {
            try
            {
                var userId = int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int parsedUserId);
                var role = User.FindFirst(ClaimTypes.Role)?.Value.ToLower();

                if (role!="admin" && parsedUserId != studentId)
                {
                    return Unauthorized("You are not authorized to access this resource.");
                }
                var response = await _userService.GetStudentDetails(studentId);
                return Ok(response);
            }
            catch (Exception e)
            {
                return NotFound(e.Message);
            }
        }
    }
}
