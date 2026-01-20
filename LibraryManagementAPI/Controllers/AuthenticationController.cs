using LibraryManagementClassLib.Dtos;
using LibraryManagementClassLib.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LibraryManagementAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthenticationController : ControllerBase
{
    private readonly IConfiguration _config;
    public AuthenticationController(IConfiguration config)
    {
        _config = config;
    }

    User user = new User()
    {
        FirstName = "John",
        LastName = "Doe",
        Email = "test@gmail.com",
        Role = Role.Member,
        PasswordHash = new PasswordHasher<User>().HashPassword(null, "testpassword"),
        CreatedAt = DateTime.Now,
        Phone = "1234567890"
    };


    [HttpPost("register")]
    public ActionResult<User> Register(UserDto request)
    {
        var hashedPassword = new PasswordHasher<User>()
            .HashPassword(user, request.Password);

        user.Email = request.Email;
        user.PasswordHash = hashedPassword;

        return Ok(user);
    }

    [HttpPost("login")]
    public ActionResult<string> Login(LoginDto request)
    {
        if (user.Email != request.Email)
        {
            return BadRequest("User not found");
        }
        if (new PasswordHasher<User>().VerifyHashedPassword(user, user.PasswordHash, request.Password) == PasswordVerificationResult.Failed)
        {
            return BadRequest("Wrong password");
        }

        string token = CreateToken(user);

        return Ok(token);
    }

    private string CreateToken(User user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
            new Claim(ClaimTypes.Email,user.Email),
            new Claim(ClaimTypes.Role,user.Role.ToString())
        };

        var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetValue<string>("Authentication:SecretKey")!));

        var signingCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

        var tokenDescriptor = new JwtSecurityToken(
            issuer: _config.GetValue<string>("Authentication:Issuer"),
            audience: _config.GetValue<string>("Authentication:Audience"),
            claims: claims,
            expires: DateTime.Now.AddMinutes(1),
            signingCredentials: signingCredentials
            );

        return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
    }
}
