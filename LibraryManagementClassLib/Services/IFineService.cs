using LibraryManagementClassLib.Dtos;
using LibraryManagementClassLib.Entities;

namespace LibraryManagementClassLib.Services
{
    public interface IFineService
    {
        Task<Fine> GetFineAsync(int isssueId);
        Task<List<FineDto>> CalculateAllFines(GeneralQueryDto query);
        Task<List<FineDto>> GetUserFines(UserFineQueryDto query);
        Task<List<FineDto>> AdminGetFines(UserFineQueryDto query);
        Task<string> PayFineAsync(int fineId);
    }
}
