using LibraryManagementClassLib.Data;
using LibraryManagementClassLib.Dtos;
using LibraryManagementClassLib.Entities;
using LibraryManagementClassLib.Helpers;
using LibraryManagementClassLib.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

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

    public async Task<bool> RegisterAsync(RegisterDto request)
    {
        bool emailMatch = await _context.Users
            .AnyAsync(e => e.Email == request.Email);

        if (emailMatch)
        {
            return false;
        }

        var user = new User
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            Role = Role.user,
            CreatedAt = DateTime.Now,
            Phone = request.Phone,
            Status = true
        };

        var hashedPassword = new PasswordHasher<User>()
            .HashPassword(user, request.Password);
        user.PasswordHash = hashedPassword;

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<TokenResponseDto?> LoginAsync(LoginDto request, string ip, string userAgent)
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

        var accessToken = TokenHelper.CreateToken(user, _config);

        var refreshToken = TokenHelper.GenerateRefreshToken();
        var hashedToken = TokenHelper.HashToken(refreshToken);

        await _context.RefreshTokens.AddAsync(new RefreshToken
        {
            UserId = user.Id,
            TokenHash = hashedToken,
            ExpiresAtUtc = request.RememberMe ? DateTime.UtcNow.AddDays(7) : DateTime.UtcNow.AddDays(1),
            CreatedAtUtc = DateTime.UtcNow,
            CreatedByIp = ip,
            UserAgent = userAgent
        });
        await _context.SaveChangesAsync();

        return new TokenResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
    }

    public async Task<TokenResponseDto?> RefreshAsync(string refreshToken, string ip, string userAgent)
    {
        var existingToken = await ValidateRefreshTokenAsync(refreshToken, ip);
        if (existingToken == null)
            return null;

        var newRefreshToken = TokenHelper.GenerateRefreshToken();
        var newHashedToken = TokenHelper.HashToken(newRefreshToken);

        // Token Rotation: Revoke the existing token and issue a new one
        existingToken.ReplacedByTokenHash = newHashedToken;
        existingToken.RevokedByIp = ip;
        existingToken.RevokedAtUtc = DateTime.UtcNow;
        _context.RefreshTokens.Update(existingToken);

        _context.RefreshTokens.Add(new RefreshToken
        {
            UserId = existingToken.UserId,
            TokenHash = newHashedToken,
            ExpiresAtUtc = existingToken.ExpiresAtUtc,
            CreatedAtUtc = DateTime.UtcNow,
            CreatedByIp = ip,
            UserAgent = userAgent
        });

        await _context.SaveChangesAsync();

        var user = await _context.Users.FindAsync(existingToken.UserId);
        var newAccessToken = TokenHelper.CreateToken(user!, _config);

        return new TokenResponseDto
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken
        };
    }

    public async Task Logout(string refreshToken, string ip)
    {
        var hashedToken = TokenHelper.HashToken(refreshToken);

        var existingToken = await _context.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.TokenHash == hashedToken);

        if (existingToken == null) return;

        if (!existingToken.IsRevoked)
        {
            existingToken.RevokedAtUtc = DateTime.UtcNow;
            existingToken.RevokedByIp = ip;
            _context.RefreshTokens.Update(existingToken);
        }

        await _context.SaveChangesAsync();
    }

    private async Task<RefreshToken?> ValidateRefreshTokenAsync(string refreshToken, string ip)
    {
        var hashedToken = TokenHelper.HashToken(refreshToken);

        var existingToken = await _context.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.TokenHash == hashedToken);
        if (existingToken == null)
            return null;

        if (existingToken.IsRevoked)
        {
            existingToken.RevokedAtUtc= DateTime.UtcNow;
            existingToken.RevokedByIp=ip;
            _context.RefreshTokens.Update(existingToken);
            await _context.SaveChangesAsync();
        }

        if(!existingToken.IsActive)
            return null;

        return existingToken;
    }
}
