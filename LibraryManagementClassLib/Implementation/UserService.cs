using LibraryManagementClassLib.Data;
using LibraryManagementClassLib.Dtos;
using LibraryManagementClassLib.Entities;
using LibraryManagementClassLib.Repository;
using LibraryManagementClassLib.Repository.IRepository;
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
    private readonly IGenericRepository<User> _genericRepository;

    public UserService(LibraryManagementAPIDbContext context,IGenericRepository<User> genericRepository) 
    {
        _context = context;
        _genericRepository = genericRepository;
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
                Role = user.Role.ToString(),
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
            Role = user.Role.ToString(),
            CreatedAt = user.CreatedAt,
            Phone = user.Phone,
            Status = user.Status
        };
    }

    public async Task<UserResponseDto> GetByEmailAsync(string email)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (user == null)
        {
            throw new Exception("User not found");
        }
        return new UserResponseDto
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            Role = user.Role.ToString(),
            CreatedAt = user.CreatedAt,
            Phone = user.Phone,
            Status = user.Status
        };
    }

    public Task<UserResponseDto> GetStudentDetails(int studentId)
    {
        var student = _context.Users.Include(u => u.StudentDetail)
            .ThenInclude(sd => sd.Course)
            .FirstOrDefault(u => u.Id == studentId);

        if (student == null)
        {
            throw new Exception("Student not found");
        }

        return Task.FromResult(new UserResponseDto
        {
            Id = student.Id,
            FirstName = student.FirstName,
            LastName = student.LastName,
            Email = student.Email,
            Role = student.Role.ToString(),
            CreatedAt = student.CreatedAt,
            Phone = student.Phone,
            Status = student.Status,
            StudentDetail = new StudentDetailDto
            {
                CourseName = student.StudentDetail.Course.Name,
                Semester = student.StudentDetail.Semester
            }
        });
    }
}
