using LibraryAPI.Exceptions;
using LibraryAPI.Models.DTOs.Request;
using LibraryAPI.Models.DTOs.Response;
using LibraryAPI.Models.Enums;

namespace LibraryAPI.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<ServiceResult<IEnumerable<CategoryResponse>>> GetAllCategoriesAsync();

        Task<ServiceResult<CategoryResponse>> GetCategoryByIdAsync(short id);

        Task<ServiceResult<string>> AddCategoryAsync(CategoryRequest categoryRequest);

        Task<ServiceResult<bool>> UpdateCategoryAsync(short id, CategoryRequest categoryRequest);

        Task<ServiceResult<bool>> SetCategoryStatusAsync(short id, Status status);

        Task<ServiceResult<IEnumerable<CategoryBookResponse>>> GetBooksByCategoryIdAsync(short categoryId);
    }
}
