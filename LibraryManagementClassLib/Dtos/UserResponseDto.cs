using LibraryManagementClassLib.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementClassLib.Dtos
{
    public class UserResponseDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public Role Role { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Phone { get; set; }
        public bool Status { get; set; }
    }
}
