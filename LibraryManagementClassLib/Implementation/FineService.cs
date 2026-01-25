using LibraryManagementClassLib.Data;
using LibraryManagementClassLib.Entities;
using LibraryManagementClassLib.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementClassLib.Implementation
{
    public class FineService : IFineService
    {
        private readonly LibraryManagementAPIDbContext _context;

        public FineService(LibraryManagementAPIDbContext context)
        {
            _context = context;
        }

        public async Task<Fine> GetFineAsync(int issueId)
        {
            var fineAmount = 0.0m;
            var fine = await _context.Fines.Where(f => f.BookIssueId == issueId).FirstOrDefaultAsync();
            if (fine is null)
            {
                fineAmount = await CalculateFineAsync(issueId);

                if ((int)fineAmount != 0)
                {
                    _context.Fines.Add(new Fine
                    {
                        BookIssueId = issueId,
                        Amount = fineAmount,
                        Status = PaidStatus.Unpaid
                    });
                }
            }
            else if (fine.Status == PaidStatus.Unpaid)
            {
                fineAmount = await CalculateFineAsync(issueId);
                fine.Amount = fineAmount;
                _context.Fines.Update(fine);
            }
            await _context.SaveChangesAsync();
            return fine;
        }

        private async Task<decimal> CalculateFineAsync(int issueId)
        {
            var bookIssue = await _context.BookIssues
                .Where(bi => bi.Id == issueId).FirstAsync();
            decimal finePerDay = 10.0m;
            var overdueDays = bookIssue.DueDate < DateTime.Now ? (DateTime.Now - bookIssue.DueDate).Days : 0;
            var totalFine = overdueDays * finePerDay;
            return totalFine;
        }
    }
}
