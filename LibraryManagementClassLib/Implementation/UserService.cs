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

    public UserService(LibraryManagementAPIDbContext context, IGenericRepository<User> genericRepository)
    {
        _context = context;
        _genericRepository = genericRepository;
    }

    public Task<bool> DeleteAsync(int userId)
    {
        throw new NotImplementedException();
    }

    public async Task<List<UserResponseDto>> GetAllStudentsAsync(StudentQueryDto queryDto)
    {
        var usersQuery = _context.Users
            .Include(u => u.StudentDetail)
            .ThenInclude(sd => sd.Course)
            .Where(u => u.Role == Role.user)
            .AsQueryable();

        if (!string.IsNullOrEmpty(queryDto.CourseCode))
        {
            usersQuery = usersQuery.Where(u => u.StudentDetail != null && u.StudentDetail.Course.Code == queryDto.CourseCode);
        }

        if (!string.IsNullOrEmpty(queryDto.SearchTerm))
        {
            usersQuery = usersQuery.Where(u => u.FullName.Contains(queryDto.SearchTerm));
        }

        var totalCount = await usersQuery.CountAsync();

        var pageNumber = queryDto.PageNumber <= 0 ? 1 : queryDto.PageNumber;
        var pageSize = queryDto.PageSize <= 0 ? 10 : queryDto.PageSize;

        var skipNumber = (pageNumber - 1) * pageSize;
        usersQuery = usersQuery.Skip(skipNumber).Take(pageSize);

        var userResponses = await usersQuery.Select(user => new UserResponseDto
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            Role = user.Role.ToString(),
            CreatedAt = user.CreatedAt,
            Phone = user.Phone,
            Status = user.Status,
            StudentDetail = user.StudentDetail != null ? new StudentDetailDto
            {
                CourseName = user.StudentDetail.Course.Name,
                Semester = user.StudentDetail.Semester
            } : null,
            TotalCount = totalCount
        }).ToListAsync();

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

    public async Task<AdminStudentViewDto> GetAdminStudentViewAsync(int studentId)
    {
        var student = await GetStudentDetails(studentId);
        
        var bookIssues = await _context.BookIssues
            .Where(bi => bi.UserId == studentId)
            .Select(bi => new BookIssuesDto
            {
                BookIssueId = bi.Id,
                Book = new BookDto
                {
                    Id = bi.Book.Id,
                    Title = bi.Book.Title,
                    ISBN = bi.Book.ISBN
                },
                Status = bi.Status.ToString(),
                IssuedDate = bi.IssueDate,
                DueDate = bi.DueDate,
                ReturnedDate = bi.ReturnDate
            })
            .Take(5)
            .ToListAsync();

        var fines = await _context.Fines.Include(f => f.BookIssue)
            .Where(f => f.BookIssue.UserId == studentId)
            .Select(f => new FineDto
            {
                Id = f.Id,
                Amount = f.Amount,
                Status = f.Status.ToString(),
                PaidDate = f.PaidDate,
                BookIssue = new BookIssuesDto
                {
                    BookIssueId = f.BookIssue.Id,
                    Book = new BookDto
                    {
                        Id = f.BookIssue.Book.Id,
                        Title = f.BookIssue.Book.Title,
                    },
                    DueDate = f.BookIssue.DueDate
                }
            })
            .Take(5)
            .ToListAsync();

        var response= new AdminStudentViewDto
        {
            User = student,
            BookIssues = bookIssues,
            Fines = fines
        };

        return response;
    }
}
