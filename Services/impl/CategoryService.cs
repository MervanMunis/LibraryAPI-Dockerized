using LibraryAPI.Data;
using LibraryAPI.Exceptions;
using LibraryAPI.Models.DTOs.Request;
using LibraryAPI.Models.DTOs.Response;
using LibraryAPI.Models.Entities;
using LibraryAPI.Models.Enums;
using LibraryAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LibraryAPI.Services.impl
{
    /// <summary>
    /// Service class for handling category-related operations.
    /// </summary>
    public class CategoryService : ICategoryService
    {
        private readonly LibraryAPIContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="CategoryService"/> class.
        /// </summary>
        /// <param name="context">The database context for accessing the database.</param>
        public CategoryService(LibraryAPIContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves all active categories, including their subcategories.
        /// </summary>
        /// <returns>A service result containing a list of active categories.</returns>
        public async Task<ServiceResult<IEnumerable<CategoryResponse>>> GetAllCategoriesAsync()
        {
            var categories = await _context.Categories!
                .Where(c => c.CategoryStatus == Status.Active.ToString())
                .Include(c => c.SubCategories)
                .Select(c => new CategoryResponse()
                {
                    CategoryId = c.CategoryId,
                    Name = c.Name,
                    CategoryStatus = c.CategoryStatus,
                    SubCategoryNamesAndIds = c.SubCategories!.Select(sc => sc.SubCategoryId + ": " + sc.Name).ToList()
                })
                .ToListAsync();
            return ServiceResult<IEnumerable<CategoryResponse>>.SuccessResult(categories);
        }

        /// <summary>
        /// Retrieves a category by its ID.
        /// </summary>
        /// <param name="id">The ID of the category to retrieve.</param>
        /// <returns>A service result containing the category details.</returns>
        public async Task<ServiceResult<CategoryResponse>> GetCategoryByIdAsync(short id)
        {
            var category = await _context.Categories!
                .Include(c => c.SubCategories)
                .Select(c => new CategoryResponse()
                {
                    CategoryId = c.CategoryId,
                    Name = c.Name,
                    CategoryStatus = c.CategoryStatus,
                    SubCategoryNamesAndIds = c.SubCategories!.Select(sc => sc.SubCategoryId + ": " + sc.Name).ToList()
                })
                .FirstOrDefaultAsync(c => c.CategoryId == id);

            if (category == null || category.CategoryStatus != Status.Active.ToString())
            {
                return ServiceResult<CategoryResponse>.FailureResult("Category not found");
            }

            return ServiceResult<CategoryResponse>.SuccessResult(category);
        }


        /// <summary>
        /// Adds a new category.
        /// </summary>
        /// <param name="categoryRequest">The request body containing details of the category to add.</param>
        /// <returns>A service result indicating success or failure.</returns>
        public async Task<ServiceResult<string>> AddCategoryAsync(CategoryRequest categoryRequest)
        {
            if (await _context.Categories!.AnyAsync(c => c.Name == categoryRequest.Name)) 
            {
                return ServiceResult<string>.FailureResult("The category name already exists!");
            }

            Category category = new Category()
            {
                Name = categoryRequest.Name,
            };

            await _context.Categories!.AddAsync(category);
            await _context.SaveChangesAsync();

            return ServiceResult<string>.SuccessMessageResult("Category successfully created.");
        }

        /// <summary>
        /// Updates an existing category.
        /// </summary>
        /// <param name="id">The ID of the category to update.</param>
        /// <param name="categoryRequest">The request body containing updated details of the category.</param>
        /// <returns>A service result indicating success or failure.</returns>
        public async Task<ServiceResult<bool>> UpdateCategoryAsync(short id, CategoryRequest categoryRequest)
        {
            var existingCategory = await _context.Categories!.FindAsync(id);
            if (existingCategory == null)
            {
                return ServiceResult<bool>.FailureResult("Category not found");
            }

            existingCategory.Name = categoryRequest.Name;
            _context.Update(existingCategory).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return ServiceResult<bool>.SuccessResult(true);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Categories.AnyAsync(c => c.CategoryId == id))
                {
                    return ServiceResult<bool>.FailureResult("Category not found");
                }
                else
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// Sets the status of a category and its related subcategories and books.
        /// </summary>
        /// <param name="id">The ID of the category to update.</param>
        /// <param name="status">The new status to set.</param>
        /// <returns>A service result indicating success or failure.</returns>
        public async Task<ServiceResult<bool>> SetCategoryStatusAsync(short id, Status status)
        {
            var category = await _context.Categories!.FindAsync(id);
            if (category == null)
            {
                return ServiceResult<bool>.FailureResult("Category not found");
            }

            category.CategoryStatus = status.ToString();
            _context.Update(category).State = EntityState.Modified;

            var subCategories = await _context.SubCategories!
                .Where(sc => sc.CategoryId == id)
                .ToListAsync();

            foreach (var subCategory in subCategories)
            {
                subCategory.SubCategoryStatus = status.ToString();
                _context.Update(subCategory).State = EntityState.Modified;

                if (status == Status.InActive)
                {
                    var bookSubCategories = await _context.BookSubCategory!
                        .Where(bsc => bsc.SubCategoriesId == subCategory.SubCategoryId)
                        .Include(bsc => bsc.Book)
                        .ToListAsync();

                    foreach (var bookSubCategory in bookSubCategories)
                    {
                        bookSubCategory.Book!.BookStatus = status.ToString();
                        _context.Update(bookSubCategory.Book).State = EntityState.Modified;
                    }
                }
            }

            try
            {
                await _context.SaveChangesAsync();
                return ServiceResult<bool>.SuccessResult(true);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Categories.AnyAsync(c => c.CategoryId == id))
                {
                    return ServiceResult<bool>.FailureResult("Category not found");
                }
                else
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// Retrieves all books associated with a specific category.
        /// </summary>
        /// <param name="categoryId">The ID of the category whose books to retrieve.</param>
        /// <returns>A service result containing a list of books in the category.</returns>
        public async Task<ServiceResult<IEnumerable<CategoryBookResponse>>> GetBooksByCategoryIdAsync(short categoryId)
        {
            var books = await _context.Books!
                .Include(b => b.BookSubCategories)!
                    .ThenInclude(bsc => bsc.SubCategory)!
                        .ThenInclude(sc => sc!.Category)
                .Where(b => b.BookSubCategories!.Any(bsc => bsc.SubCategory!.CategoryId == categoryId))
                .Select(b => new CategoryBookResponse()
                {
                    BookId = b.BookId,
                    ISBN = b.ISBN,
                    Title = b.Title,
                    SubCategoryNames = b.BookSubCategories!.Select(bs => bs.SubCategory!.Name).ToList(),
                }).ToListAsync();

            if (!books.Any())
            {
                return ServiceResult<IEnumerable<CategoryBookResponse>>.FailureResult("No books found for this category");
            }

            return ServiceResult<IEnumerable<CategoryBookResponse>>.SuccessResult(books);
        }
    }
}
