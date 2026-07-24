using LibraryManagementClassLib.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementClassLib.Dtos
{
    public class ActivityLogDto
    {
        public int UserId { get; set; }
        public string ActivityType { get; set; }
        public string Description { get; set; }
        public string? MetaData { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? ReferenceId { get; set; }
        public User? User { get; set; }

    }
}
