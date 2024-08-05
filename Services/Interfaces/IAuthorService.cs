using LibraryAPI.Exceptions;
using LibraryAPI.Models.DTOs.Request;
using LibraryAPI.Models.DTOs.Response;

namespace LibraryAPI.Services.Interfaces
{
    public interface IAuthorService
    {
        Task<ServiceResult<IEnumerable<AuthorResoponse>>> GetAllAuthorsAsync();

        Task<ServiceResult<IEnumerable<AuthorResoponse>>> GetAllActiveAuthorsAsync();

        Task<ServiceResult<IEnumerable<AuthorResoponse>>> GetAllInActiveAuthorsAsync();

        Task<ServiceResult<IEnumerable<AuthorResoponse>>> GetAllBannedAuthorsAsync();

        Task<ServiceResult<AuthorResoponse>> GetAuthorByIdAsync(long id);

        Task<ServiceResult<string>> AddAuthorAsync(AuthorRequest author);

        Task<ServiceResult<bool>> UpdateAuthorAsync(long id, AuthorRequest author);

        Task<ServiceResult<bool>> SetAuthorStatusAsync(long id, string status);

        Task<ServiceResult<bool>> UpdateAuthorImageAsync(long id, IFormFile image);

        Task<ServiceResult<byte[]>> GetAuthorImageAsync(long authorId);
    }
}
