using LibraryManagementClassLib.Dtos;
using LibraryManagementClassLib.Entities;
using LibraryManagementClassLib.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementClassLib.Implementation
{
    public class AuthenticationService : IAuthenticationService
    {
        public User Register(UserDto request)
        {
            return new User();
        }
    }
}
