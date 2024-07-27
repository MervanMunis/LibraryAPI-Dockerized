using LibraryAPI.DTOs;
using LibraryAPI.DTOs.Request;
using LibraryAPI.DTOs.Response;
using LibraryAPI.Entities.Enums;
using LibraryAPI.Entities.Models;
using LibraryAPI.Exceptions;

namespace LibraryAPI.Services.Interfaces
{
    public interface ISubCategoryService
    {
        Task<ServiceResult<IEnumerable<SubCategoryResponse>>> GetAllSubCategoriesAsync();
        Task<ServiceResult<SubCategoryResponse>> GetSubCategoryByIdAsync(short id);
        Task<ServiceResult<string>> AddSubCategoryAsync(SubCategoryRequest subCategoryRequest);
        Task<ServiceResult<bool>> UpdateSubCategoryAsync(short id, SubCategoryRequest subCategoryRequest);
        Task<ServiceResult<bool>> SetSubCategoryStatusAsync(short id, string status);
        Task<ServiceResult<IEnumerable<BookResponse>>> GetBooksBySubCategoryIdAsync(short subCategoryId);
    }
}
