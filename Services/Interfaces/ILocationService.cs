using LibraryAPI.Exceptions;
using LibraryAPI.Models.DTOs.Request;
using LibraryAPI.Models.DTOs.Response;
using LibraryAPI.Models.Enums;

namespace LibraryAPI.Services.Interfaces
{
    public interface ILocationService
    {
        Task<ServiceResult<IEnumerable<LocationResponse>>> GetAllLocationsAsync();
        Task<ServiceResult<LocationResponse>> GetLocationByIdAsync(int id);
        Task<ServiceResult<string>> AddLocationAsync(LocationRequest locationRequest);
        Task<ServiceResult<bool>> UpdateLocationAsync(int id, LocationRequest locationRequest);
        Task<ServiceResult<bool>> SetLocationStatusAsync(int id, Status status);
        Task<ServiceResult<IEnumerable<BookResponse>>> GetBooksBySectionCodeAsync(string sectionCode);
        Task<ServiceResult<IEnumerable<BookResponse>>> GetBooksByAisleCodeAsync(string aisleCode);
        Task<ServiceResult<IEnumerable<BookResponse>>> GetBooksByShelfNumberAsync(string shelfNumber);
    }
}
