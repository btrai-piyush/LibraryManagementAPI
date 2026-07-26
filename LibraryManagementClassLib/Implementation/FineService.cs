using Azure;
using LibraryManagementClassLib.Data;
using LibraryManagementClassLib.Dtos;
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

            var bookIssue = await _context.BookIssues
                .Include(bi => bi.Fine)
                .FirstOrDefaultAsync(bi => bi.Id == issueId);

            if (bookIssue.Fine is null)
            {
                fineAmount = CalculateFine(bookIssue.DueDate);

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
            else if (bookIssue.Fine.Status == PaidStatus.Unpaid)
            {
                fineAmount = CalculateFine(bookIssue.DueDate);
                bookIssue.Fine.Amount = fineAmount;
                _context.Fines.Update(bookIssue.Fine);
            }
            await _context.SaveChangesAsync();
            return bookIssue.Fine;
        }

        public async Task<List<FineDto>> CalculateAllFines(GeneralQueryDto query)
        {
            var response = new List<FineDto>();

            var bookIssues = _context.BookIssues.
                Where(bi => bi.DueDate.Date < DateTime.Today.Date)
                .Include(bi => bi.Fine)
                .Include(bi => bi.Book)
                .Include(bi => bi.User)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.SearchTerm))
            {
                bookIssues = bookIssues.Where(bi => bi.Book.Title.Contains(query.SearchTerm)
                                                    || bi.User.FirstName.Contains(query.SearchTerm)
                                                    || bi.User.LastName.Contains(query.SearchTerm));
            }

            var queryCount = await bookIssues.CountAsync();

            bookIssues = bookIssues.OrderBy(bi => bi.DueDate);

            var pageNumber = query.PageNumber <= 0 ? 1 : query.PageNumber;
            var pageSize = query.PageSize <= 0 ? 10 : query.PageSize;
            var skip = (pageNumber - 1) * pageSize;
            bookIssues = bookIssues.Skip(skip).Take(pageSize);

            foreach (var bookIssue in bookIssues)
            {
                var fineAmount = CalculateFine(bookIssue.DueDate);

                if (bookIssue.Fine is null)
                {
                    bookIssue.Fine = new Fine
                    {
                        BookIssueId = bookIssue.Id,
                        Amount = fineAmount,
                        Status = PaidStatus.Unpaid
                    };
                    bookIssue.Status = IssueStatus.Overdue;
                }
                else if (bookIssue.Fine.Status == PaidStatus.Unpaid)
                {
                    bookIssue.Fine.Amount = fineAmount;
                }



                var fineDto = new FineDto()
                {
                    Id = bookIssue.Fine.Id,
                    Amount = bookIssue.Fine.Amount,
                    Status = bookIssue.Fine.Status.ToString(),
                    PaidDate = bookIssue.Fine.PaidDate,
                    BookIssue = new BookIssuesDto
                    {
                        Book = new BookDto
                        {
                            Id = bookIssue.Book.Id,
                            Title = bookIssue.Book.Title,
                            ISBN = bookIssue.Book.ISBN,
                        },
                        User = new UserDto
                        {
                            Id = bookIssue.User.Id,
                            FirstName = bookIssue.User.FirstName,
                            LastName = bookIssue.User.LastName,
                            Email = bookIssue.User.Email
                        },
                        DueDate = bookIssue.DueDate,
                    },
                    TotalCount = queryCount
                };

                response.Add(fineDto);

            }
            await _context.SaveChangesAsync();

            return response;
        }

        public async Task<List<FineDto>> GetUserFines(UserFineQueryDto query)
        {
            var finesQuery = _context.Fines
                .Include(f => f.BookIssue)
                .ThenInclude(bi => bi.User)
                .Where(f => f.BookIssue.UserId == query.UserId)
                .AsQueryable();

            if (!string.IsNullOrEmpty(query.Status))
            {
                if (query.Status.ToLower() == "paid")
                {
                    finesQuery = finesQuery.Where(f => f.Status == PaidStatus.Paid);

                }
                else if (query.Status.ToLower() == "unpaid")
                {
                    finesQuery = finesQuery.Where(f => f.Status == PaidStatus.Unpaid);
                }
            }

            if (!string.IsNullOrWhiteSpace(query.SearchTerm))
            {
                var searchTerm = query.SearchTerm.ToLower();
                finesQuery = finesQuery.Where(f => f.BookIssue.Book.Title.Contains(searchTerm)
                                        || f.BookIssue.User.FirstName.ToLower().Contains(searchTerm)
                                        || f.BookIssue.User.LastName.ToLower().Contains(searchTerm)
                                        || f.BookIssue.User.FullName.ToLower().Contains(searchTerm));
            }

            var queryCount = await finesQuery.CountAsync();

            finesQuery = finesQuery.OrderByDescending(f => f.BookIssue.DueDate);

            var pageNumber = query.PageNumber <= 0 ? 1 : query.PageNumber;
            var pageSize = query.PageSize <= 0 ? 10 : query.PageSize;
            var skip = (pageNumber - 1) * pageSize;

            finesQuery = finesQuery.Skip(skip).Take(pageSize);


            if (await finesQuery.CountAsync() == 0)
            {
                if(query.Status != null)
                {
                    throw new InvalidOperationException($"No {query.Status.ToLower()} fines found.");
                }

                throw new InvalidOperationException("No fines found for the specified user.");
            }

            var userFines = finesQuery.Select(f => new FineDto
            {
                Id = f.Id,
                Amount = f.Amount,
                Status = f.Status.ToString(),
                PaidDate = f.PaidDate,
                BookIssue = new BookIssuesDto
                {
                    Book = new BookDto
                    {
                        Id = f.BookIssue.Book.Id,
                        Title = f.BookIssue.Book.Title,
                        ISBN = f.BookIssue.Book.ISBN,
                    },
                    //User = new UserDto
                    //{
                    //    Id = f.BookIssue.User.Id,
                    //    FirstName = f.BookIssue.User.FirstName,
                    //    LastName = f.BookIssue.User.LastName,
                    //    Email = f.BookIssue.User.Email
                    //},
                    DueDate = f.BookIssue.DueDate,
                    TotalCount = queryCount
                }
            }).ToList();

            var totalFineAmount = userFines.Sum(f => f.Amount);

            userFines.ForEach(f => f.TotalFineAmount = totalFineAmount);

            return userFines;
        }

        public async Task<string> PayFineAsync(int fineId)
        {
            var fine = await _context.Fines.FindAsync(fineId);

            if (fine == null)
            {
                throw new InvalidOperationException("Fine not found.");
            }

            fine.Status= PaidStatus.Paid;
            fine.PaidDate= DateTime.Now;
            await _context.SaveChangesAsync();

            return "Fine paid successfully.";
        }

        private decimal CalculateFine(DateTime dueDate)
        {
            decimal finePerDay = 10.0m;
            var overdueDays = dueDate < DateTime.Now ? (DateTime.Now - dueDate).Days : 0;
            var totalFine = overdueDays * finePerDay;
            return totalFine;
        }

        public async Task<List<FineDto>> AdminGetFines(UserFineQueryDto query)
        {
            var finesQuery = _context.Fines
                .Include(f => f.BookIssue)
                .ThenInclude(bi => bi.User)
                .AsQueryable();

            if (!string.IsNullOrEmpty(query.Status))
            {
                if (query.Status.ToLower() == "paid")
                {
                    finesQuery = finesQuery.Where(f => f.Status == PaidStatus.Paid);

                }
                else if (query.Status.ToLower() == "unpaid")
                {
                    finesQuery = finesQuery.Where(f => f.Status == PaidStatus.Unpaid);
                }
            }

            if (!string.IsNullOrWhiteSpace(query.SearchTerm))
            {
                var searchTerm = query.SearchTerm.ToLower();
                finesQuery = finesQuery.Where(f => f.BookIssue.Book.Title.Contains(searchTerm)
                                        || f.BookIssue.User.FirstName.ToLower().Contains(searchTerm)
                                        || f.BookIssue.User.LastName.ToLower().Contains(searchTerm)
                                        || f.BookIssue.User.FullName.ToLower().Contains(searchTerm));
            }

            var queryCount = await finesQuery.CountAsync();

            finesQuery = finesQuery.OrderByDescending(f => f.BookIssue.DueDate);

            var pageNumber = query.PageNumber <= 0 ? 1 : query.PageNumber;
            var pageSize = query.PageSize <= 0 ? 10 : query.PageSize;
            var skip = (pageNumber - 1) * pageSize;

            finesQuery = finesQuery.Skip(skip).Take(pageSize);


            if (await finesQuery.CountAsync() == 0)
            {
                if (query.Status != null)
                {
                    throw new InvalidOperationException($"No {query.Status.ToLower()} fines found.");
                }

                throw new InvalidOperationException("No fines found for the specified user.");
            }

            var userFines = finesQuery.Select(f => new FineDto
            {
                Id = f.Id,
                Amount = f.Amount,
                Status = f.Status.ToString(),
                PaidDate = f.PaidDate,
                BookIssue = new BookIssuesDto
                {
                    Book = new BookDto
                    {
                        Id = f.BookIssue.Book.Id,
                        Title = f.BookIssue.Book.Title,
                        ISBN = f.BookIssue.Book.ISBN,
                    },
                    User = new UserDto
                    {
                        Id = f.BookIssue.User.Id,
                        FirstName = f.BookIssue.User.FirstName,
                        LastName = f.BookIssue.User.LastName,
                        Email = f.BookIssue.User.Email
                    },
                    DueDate = f.BookIssue.DueDate
                },
                TotalCount = queryCount
            }).ToList();

            return userFines;
        }
    }
}
