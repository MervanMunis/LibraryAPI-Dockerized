using LibraryAPI.DTOs;
using LibraryAPI.DTOs.Request;
using LibraryAPI.DTOs.Response;
using LibraryAPI.Entities.Models;
using LibraryAPI.Exceptions;

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
