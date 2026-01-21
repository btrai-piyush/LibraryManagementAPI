using LibraryManagementClassLib.Dtos;
using LibraryManagementClassLib.Entities;
using LibraryManagementClassLib.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthenticationController : ControllerBase
{
    private readonly IAuthService _authService;
    public AuthenticationController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<User>> Register(UserDto request)
    {
        var user = await _authService.RegisterAsync(request);
        if (user == null)
        {
            return BadRequest("User already exists");
        }

        return Ok(user);
    }

    [HttpPost("login")]
    public async Task<ActionResult<string>> Login(LoginDto request)
    {
        var token = await _authService.LoginAsync(request);
        if (token == null)
        {
            return BadRequest("Invalid credentials");
        }

        return Ok(token);
    }
}
