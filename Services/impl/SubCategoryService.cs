using LibraryAPI.Data;
using LibraryAPI.DTOs.Request;
using LibraryAPI.DTOs.Response;
using LibraryAPI.Entities.Enums;
using LibraryAPI.Entities.Models;
using LibraryAPI.Exceptions;
using LibraryAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LibraryAPI.Services.impl
{
    public class SubCategoryService : ISubCategoryService
    {
        private readonly LibraryAPIContext _context;

        public SubCategoryService(LibraryAPIContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves all active subcategories from the database.
        /// </summary>
        public async Task<ServiceResult<IEnumerable<SubCategoryResponse>>> GetAllSubCategoriesAsync()
        {
            var subCategories = await _context.SubCategories!
                .Where(sc => sc.SubCategoryStatus == Status.Active.ToString())
                .Select(sb => new SubCategoryResponse()
                {
                    SubCategoryId = sb.SubCategoryId,
                    Name = sb.Name,
                    SubCategoryStatus = sb.SubCategoryStatus,
                })
                .ToListAsync();

            return ServiceResult<IEnumerable<SubCategoryResponse>>.SuccessResult(subCategories);
        }

        /// <summary>
        /// Retrieves a specific subcategory by its ID.
        /// </summary>
        /// <param name="id">The ID of the subcategory to retrieve.</param>
        public async Task<ServiceResult<SubCategoryResponse>> GetSubCategoryByIdAsync(short id)
        {
            var subCategory = await _context.SubCategories!
                .Where(sc => sc.SubCategoryStatus == Status.Active.ToString())
                .Select(sb => new SubCategoryResponse()
                {
                    SubCategoryId = sb.SubCategoryId,
                    Name = sb.Name,
                    SubCategoryStatus = sb.SubCategoryStatus,
                })
                .FirstOrDefaultAsync(sc => sc.SubCategoryId == id);

            if (subCategory == null || subCategory.SubCategoryStatus != Status.Active.ToString())
            {
                return ServiceResult<SubCategoryResponse>.FailureResult("SubCategory not found");
            }

            return ServiceResult<SubCategoryResponse>.SuccessResult(subCategory);
        }

        /// <summary>
        /// Adds a new subcategory to the database.
        /// </summary>
        /// <param name="subCategoryRequest">The details of the subcategory to add.</param>
        public async Task<ServiceResult<string>> AddSubCategoryAsync(SubCategoryRequest subCategoryRequest)
        {
            SubCategory subCategory = new SubCategory()
            {
                Name = subCategoryRequest.Name,
                CategoryId = subCategoryRequest.CategoryId
            };
            await _context.SubCategories!.AddAsync(subCategory);
            await _context.SaveChangesAsync();
            return ServiceResult<string>.SuccessMessageResult("SubCategory successfully created!");
        }

        /// <summary>
        /// Updates an existing subcategory's details.
        /// </summary>
        /// <param name="id">The ID of the subcategory to update.</param>
        /// <param name="subCategoryRequest">The new details of the subcategory.</param>
        public async Task<ServiceResult<bool>> UpdateSubCategoryAsync(short id, SubCategoryRequest subCategoryRequest)
        {
            var existingSubCategory = await _context.SubCategories!.FindAsync(id);
            if (existingSubCategory == null)
            {
                return ServiceResult<bool>.FailureResult("SubCategory not found");
            }

            existingSubCategory.Name = subCategoryRequest.Name;
            existingSubCategory.CategoryId = subCategoryRequest.CategoryId;
            _context.Entry(existingSubCategory).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return ServiceResult<bool>.SuccessResult(true);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.SubCategories.AnyAsync(sc => sc.SubCategoryId == id))
                {
                    return ServiceResult<bool>.FailureResult("SubCategory not found");
                }
                else
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// Sets the status of a subcategory.
        /// </summary>
        /// <param name="id">The ID of the subcategory whose status is to be updated.</param>
        /// <param name="status">The new status of the subcategory.</param>
        public async Task<ServiceResult<bool>> SetSubCategoryStatusAsync(short id, string status)
        {
            var subCategory = await _context.SubCategories!.FindAsync(id);
            if (subCategory == null)
            {
                return ServiceResult<bool>.FailureResult("SubCategory not found");
            }

            subCategory.SubCategoryStatus = status;
            _context.Entry(subCategory).State = EntityState.Modified;

            if (status == Status.InActive.ToString())
            {
                var bookSubCategories = await _context.BookSubCategory!
                    .Where(bsc => bsc.SubCategoriesId == id)
                    .Include(bsc => bsc.Book)
                    .ToListAsync();

                foreach (var bookSubCategory in bookSubCategories)
                {
                    bookSubCategory.Book!.BookStatus = status;
                    _context.Entry(bookSubCategory.Book).State = EntityState.Modified;
                }
            }

            try
            {
                await _context.SaveChangesAsync();
                return ServiceResult<bool>.SuccessResult(true);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.SubCategories.AnyAsync(e => e.SubCategoryId == id))
                {
                    return ServiceResult<bool>.FailureResult("SubCategory not found");
                }
                else
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// Retrieves all books associated with a specific subcategory.
        /// </summary>
        /// <param name="subCategoryId">The ID of the subcategory whose books are to be retrieved.</param>

        public async Task<ServiceResult<IEnumerable<BookResponse>>> GetBooksBySubCategoryIdAsync(short subCategoryId)
        {
            var books = await _context.Books!
                .Include(b => b.BookSubCategories)!
                    .ThenInclude(bsc => bsc.SubCategory)
                .Where(b => b.BookSubCategories!.Any(bsc => bsc.SubCategoriesId == subCategoryId))
                .Select(b => new BookResponse()
                {
                    BookId = b.BookId,
                    ISBN = b.ISBN,
                    Title = b.Title,
                    CategoryName = b.BookSubCategories!.Select(sb => sb.SubCategory!.Category!.Name).First()
                })
                .ToListAsync();

            if (!books.Any())
            {
                return ServiceResult<IEnumerable<BookResponse>>.FailureResult("No books found for this subcategory");
            }

            return ServiceResult<IEnumerable<BookResponse>>.SuccessResult(books);
        }
    }
}
