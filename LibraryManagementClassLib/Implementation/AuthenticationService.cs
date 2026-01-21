using LibraryManagementClassLib.Data;
using LibraryManagementClassLib.Dtos;
using LibraryManagementClassLib.Entities;
using LibraryManagementClassLib.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementClassLib.Implementation;

public class AuthService : IAuthService
{
    private readonly LibraryManagementAPIDbContext _context;
    private readonly IConfiguration _config;

    public AuthService(LibraryManagementAPIDbContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
    }
    public async Task<User?> RegisterAsync(UserDto request)
    {
        bool emailMatch = await _context.Users
            .AnyAsync(e => e.Email == request.Email);

        if (emailMatch)
        {
            return null;
        }

        var user = new User
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            Role = Role.Member,
            CreatedAt = DateTime.Now,
            Phone = request.Phone,
            Status = true
        };

        var hashedPassword = new PasswordHasher<User>()
            .HashPassword(user, request.Password);
        user.PasswordHash = hashedPassword;

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return user;
    }
    public async Task<string?> LoginAsync(LoginDto request)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email);

        if (user == null)
        {
            return null;
        }

        if (new PasswordHasher<User>()
            .VerifyHashedPassword(user, user.PasswordHash, request.Password) == PasswordVerificationResult.Failed)
        {
            return null;
        }

        return CreateToken(user);
    }

    public string CreateToken(User user)
    {
        var claims = new List<Claim>
        {
        new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
        new Claim(ClaimTypes.Email,user.Email),
        new Claim(ClaimTypes.Role,user.Role.ToString())
        };

        var secretKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_config.GetValue<string>("Authentication:SecretKey")!));

        var creds = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

        var tokenDescriptor = new JwtSecurityToken(
            issuer: _config.GetValue<string>("Authentication:Issuer"),
            audience: _config.GetValue<string>("Authentication:Audience"),
            claims: claims,
            expires: DateTime.UtcNow.AddDays(1),
            signingCredentials: creds
            );

        return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
    }
}
