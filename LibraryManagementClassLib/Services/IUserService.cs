using LibraryManagementClassLib.Dtos;
using LibraryManagementClassLib.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementClassLib.Services
{
    public interface IUserService
    {
        Task<List<UserResponseDto>> GetAllStudentsAsync(StudentQueryDto queryDto);
        Task<UserResponseDto> GetUserByIdAsync(int userId);
        Task<bool> DeleteAsync(int userId);
        Task<UserResponseDto> GetByEmailAsync(string email);
        Task<UserResponseDto> GetStudentDetails(int studentId);
        Task<AdminStudentViewDto> GetAdminStudentViewAsync(int studentId);
    }
}
