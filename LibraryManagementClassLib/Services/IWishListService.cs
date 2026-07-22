using LibraryManagementClassLib.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementClassLib.Services
{
    public interface IWishListService
    {
        Task<string> AddBookToWishListAsync(WishlistRequestDto request);
        Task<string> RemoveBookFromWishListAsync(WishlistRequestDto request);
        Task<List<BookDto>> GetWishListBooksAsync(int userId);
    }
}
