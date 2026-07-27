using LibraryManagementClassLib.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementClassLib.Services
{
    public interface IGoogleBooksService
    {
        Task<BookDto?> GetBookByIsbnAsync(string isbn);
    }
}
