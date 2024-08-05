using LibraryAPI.Exceptions;
using LibraryAPI.Models.DTOs.Request;
using LibraryAPI.Models.DTOs.Response;

namespace LibraryAPI.Services.Interfaces
{
    public interface IWantedBookService
    {
        Task<ServiceResult<IEnumerable<WantedBookResponse>>> GetAllWantedBooksAsync();
        Task<ServiceResult<WantedBookResponse>> GetWantedBookByIdAsync(int id);
        Task<ServiceResult<string>> AddWantedBookAsync(WantedBookRequest wantedBookRequest);
        Task<ServiceResult<bool>> DeleteWantedBookAsync(int id);
    }
}
