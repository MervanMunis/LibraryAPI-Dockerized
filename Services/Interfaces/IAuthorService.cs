using LibraryAPI.DTOs;
using LibraryAPI.DTOs.Request;
using LibraryAPI.Entities.Enums;
using LibraryAPI.Entities.Models;
using LibraryAPI.Exceptions;

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
