using LibraryManagementClassLib.Dtos;
using LibraryManagementClassLib.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementClassLib.Services
{
    public interface IAuthService
    {
        Task<bool> RegisterAsync(RegisterDto request);
        Task<TokenResponseDto?> LoginAsync(LoginDto request,string ip,string userAgent);
        Task<TokenResponseDto?> RefreshAsync(string refreshToken, string ip, string userAgent);
        Task Logout(string refreshToken, string ip);
    }
}
