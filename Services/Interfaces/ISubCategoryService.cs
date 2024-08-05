using LibraryAPI.Exceptions;
using LibraryAPI.Models.DTOs.Request;
using LibraryAPI.Models.DTOs.Response;

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
