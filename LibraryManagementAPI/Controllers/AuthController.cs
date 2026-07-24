using Asp.Versioning;
using LibraryManagementClassLib.Dtos;
using LibraryManagementClassLib.Entities;
using LibraryManagementClassLib.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
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
[ApiVersionNeutral]
public class AuthController : ControllerBase
{
    private string AccessTokenCookieName = "accessToken";
    private string RefreshTokenCookieName = "refreshToken";

    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpGet("debug")]
    public IActionResult Debug()
    {
        return Ok(new
        {
            Cookies = Request.Cookies.Keys.ToList(),
            HasAccessToken = Request.Cookies.ContainsKey("accessToken"),
            AccessToken = Request.Cookies["accessToken"]?.Substring(0, 20)
        });
    }

    [HttpPost("register")]
    public async Task<ActionResult> Register(RegisterDto request)
    {
        var user = await _authService.RegisterAsync(request);
        if (!user)
        {
            return BadRequest("User already exists");
        }

        return Ok("User created successfully");
    }

    [HttpPost("login")]
    public async Task<ActionResult<TokenResponseDto>> Login(LoginDto request)
    {
        try
        {
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
            var userAgent = Request.Headers["User-Agent"].ToString();
            var result = await _authService.LoginAsync(request, ip, userAgent);
            if (result == null)
            {
                return BadRequest("Invalid credentials");
            }

            SetCookies(result.AccessToken, result.RefreshToken, request.RememberMe);

            return Ok(new { message = "Login successful" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpGet("logout")]
    public async Task<IActionResult> Logout()
    {
        var refreshToken = Request.Cookies[RefreshTokenCookieName];
        var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

        if (!string.IsNullOrEmpty(refreshToken))
        {
            await _authService.Logout(refreshToken, ip);
        }

        ClearAuthCookies();

        return Ok(new { message = "Logged out successfully" });
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> Refresh()
    {
        var token = Request.Cookies[RefreshTokenCookieName];
        if (string.IsNullOrEmpty(token))
            return Unauthorized();

        var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
        var userAgent = Request.Headers["User-Agent"].ToString();

        var result = await _authService.RefreshAsync(token, ip, userAgent);
        if (result == null)
        {
            return Unauthorized();
        }

        SetCookies(result.AccessToken, result.RefreshToken, true);

        return Ok();
    }

    [Authorize]
    [HttpGet("me")]
    public IActionResult Me()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var email = User.FindFirst(ClaimTypes.Email)?.Value;
        var role = User.FindFirst(ClaimTypes.Role)?.Value;

        return Ok(new
        {
            userId,
            email,
            role
        });
    }

    private void ClearAuthCookies()
    {
        var deleteCookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Path = "/",
            Secure = false,
            SameSite = SameSiteMode.Lax
        };

        Response.Cookies.Delete(AccessTokenCookieName, deleteCookieOptions);
        Response.Cookies.Delete(RefreshTokenCookieName, deleteCookieOptions);
    }

    private void SetCookies(string accessToken, string refreshToken, bool rememberMe)
    {
        var accessOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = false,
            SameSite = SameSiteMode.Lax,
            Path = "/",
            Expires = DateTime.UtcNow.AddDays(7)
        };

        var refreshOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = false,
            SameSite = SameSiteMode.Lax,
            Path = "/",
            Expires = rememberMe
                ? DateTime.UtcNow.AddDays(7)
                : DateTime.UtcNow.AddDays(1)
        };

        Response.Cookies.Append(AccessTokenCookieName, accessToken, accessOptions);
        Response.Cookies.Append(RefreshTokenCookieName, refreshToken, refreshOptions);
    }
}