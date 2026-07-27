using LibraryManagementClassLib.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementClassLib.Services
{
    public interface IRecommendationService
    {
        Task<List<BookDto>?> GetUserRecommendations(int userId);
    }
}
