using LibraryAPI.DTOs.Request;
using LibraryAPI.DTOs.Response;
using LibraryAPI.Entities.Enums;
using LibraryAPI.Entities.Models;
using LibraryAPI.Exceptions;

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
