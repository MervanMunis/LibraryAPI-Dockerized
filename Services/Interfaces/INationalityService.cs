using LibraryAPI.Exceptions;
using LibraryAPI.Models.DTOs.Request;
using LibraryAPI.Models.DTOs.Response;

namespace LibraryAPI.Services.Interfaces
{
    public interface INationalityService
    {
        Task<ServiceResult<IEnumerable<NationalityResponse>>> GetAllNationalitiesAsync();
        Task<ServiceResult<NationalityResponse>> GetNationalityByIdAsync(short id);
        Task<ServiceResult<string>> AddNationalityAsync(NationalityRequest nationalityRequest);
        Task<ServiceResult<bool>> UpdateNationalityAsync(short id, NationalityRequest nationalityRequest);
        Task<ServiceResult<bool>> DeleteNationalityAsync(short id);
        Task<ServiceResult<IEnumerable<AuthorResoponse>>> GetAuthorsByNationalityIdAsync(short id);
    }
}
