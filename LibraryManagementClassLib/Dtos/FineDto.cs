using LibraryManagementClassLib.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementClassLib.Dtos
{
    public class FineDto
    {
        public int Id { get; set; }

        [Precision(8, 2)]
        public decimal Amount { get; set; }
        public string Status { get; set; }
        public DateTime? PaidDate { get; set; }

        public BookIssuesDto BookIssue { get; set; }
        public decimal TotalFineAmount { get; set; }
        public int TotalCount { get; set; }
    }
}
