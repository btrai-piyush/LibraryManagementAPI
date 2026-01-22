using LibraryManagementClassLib.Data;
using LibraryManagementClassLib.Dtos;
using LibraryManagementClassLib.Entities;
using LibraryManagementClassLib.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementClassLib.Implementation;

public class UserService : IUserService
{
    private readonly LibraryManagementAPIDbContext _context;

    public UserService(LibraryManagementAPIDbContext context)
    {
        _context = context;
    }

    public Task<bool> DeleteAsync(int userId)
    {
        throw new NotImplementedException();
    }

    public async Task<List<UserResponseDto>> GetAllUsersAsync()
    {
        var users = await _context.Users.ToListAsync();
        List<UserResponseDto> userResponses = new List<UserResponseDto>();
        foreach (var user in users)
        {
            userResponses.Add(new UserResponseDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Role = user.Role,
                CreatedAt = user.CreatedAt,
                Phone = user.Phone,
                Status = user.Status
            });
        }
        return userResponses;
    }

    public async Task<UserResponseDto> GetUserByIdAsync(int userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
        {
            return null;
        }
        return new UserResponseDto
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            Role = user.Role,
            CreatedAt = user.CreatedAt,
            Phone = user.Phone,
            Status = user.Status
        };
    }
}
