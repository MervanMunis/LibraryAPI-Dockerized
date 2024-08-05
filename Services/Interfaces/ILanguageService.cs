using LibraryAPI.Exceptions;
using LibraryAPI.Models.DTOs.Request;
using LibraryAPI.Models.DTOs.Response;

namespace LibraryAPI.Services.Interfaces
{
    public interface ILanguageService
    {
        Task<ServiceResult<IEnumerable<LanguageResponse>>> GetAllLanguagesAsync();
        Task<ServiceResult<LanguageResponse>> GetLanguageByIdAsync(short id);
        Task<ServiceResult<string>> AddLanguageAsync(LanguageRequest languageRequest);
        Task<ServiceResult<bool>> UpdateLanguageAsync(short id, LanguageRequest languageRequest);
        Task<ServiceResult<bool>> SetLanguageStatusAsync(short id, string status);
        Task<ServiceResult<IEnumerable<BookResponse>>> GetBooksByLanguageIdAsync(short languageId);
    }
}
