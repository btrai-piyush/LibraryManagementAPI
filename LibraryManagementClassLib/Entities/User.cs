using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementClassLib.Entities
{
    public enum Role
    {
        Librarian,
        Member
    }
    public class User
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "First Name is required")]
        [MaxLength(50, ErrorMessage = "First Name cannot exceed 50 characters")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is required")]
        [MaxLength(50, ErrorMessage = "Last Name cannot exceed 50 characters")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }

        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
        public Role Role { get; set; }
        public string PasswordHash { get; set; }
        public DateTime CreatedAt { get; set; }

        [Phone(ErrorMessage = "Invalid Phone Number")]
        [MaxLength(15, ErrorMessage = "Phone number cannot exceed 15 characters")]
        public string Phone { get; set; }
        public bool Status { get; set; }

        public ICollection<BookIssue>? BookIssues { get; set; }
    }
}
