using LibraryAPI.DTOs.Request;
using LibraryAPI.DTOs.Response;
using LibraryAPI.Entities.Enums;
using LibraryAPI.Entities.Models;
using LibraryAPI.Exceptions;

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
