using LibraryAPI.Data;
using LibraryAPI.DTOs;
using LibraryAPI.DTOs.Request;
using LibraryAPI.DTOs.Response;
using LibraryAPI.Entities.Models;
using LibraryAPI.Exceptions;
using LibraryAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LibraryAPI.Services.impl
{
    public class WantedBookService : IWantedBookService
    {
        private readonly LibraryAPIContext _context;

        public WantedBookService(LibraryAPIContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves all wanted books.
        /// </summary>
        /// <returns>A list of all wanted books.</returns>
        public async Task<ServiceResult<IEnumerable<WantedBookResponse>>> GetAllWantedBooksAsync()
        {
            var wantedBooks = await _context.WantedBooks!
                .Include(wb => wb.Language)
                .Include(wb => wb.Category)
                .Select(wbr => new WantedBookResponse()
                {
                    Title = wbr.Title,
                    Description = wbr.Description,
                    Languages = wbr.Language!.Name,
                    SubCategories = wbr.Category!.SubCategories!.Select(c => c.Name).ToList()

                })
                .ToListAsync();
            return ServiceResult<IEnumerable<WantedBookResponse>>.SuccessResult(wantedBooks);
        }

        /// <summary>
        /// Retrieves a specific wanted book by its ID.
        /// </summary>
        /// <param name="id">The ID of the wanted book to retrieve.</param>
        /// <returns>The requested wanted book.</returns>
        public async Task<ServiceResult<WantedBookResponse>> GetWantedBookByIdAsync(int id)
        {
            var wantedBook = await _context.WantedBooks!
                .Include(wb => wb.Language)
                .Include(wb => wb.Category)
                .Select(wbr => new WantedBookResponse()
                {
                    Title = wbr.Title,
                    Description = wbr.Description,
                    Languages = wbr.Language!.Name,
                    SubCategories = wbr.Category!.SubCategories!.Select(c => c.Name).ToList()

                })
                .FirstOrDefaultAsync();

            if (wantedBook == null)
            {
                return ServiceResult<WantedBookResponse>.FailureResult("Wanted book not found");
            }

            return ServiceResult<WantedBookResponse>.SuccessResult(wantedBook);
        }

        /// <summary>
        /// Adds a new wanted book.
        /// </summary>
        /// <param name="wantedBookRequest">The details of the wanted book to add.</param>
        /// <returns>A success message if the wanted book is added successfully.</returns>
        public async Task<ServiceResult<string>> AddWantedBookAsync(WantedBookRequest wantedBookRequest)
        {
            var newWantedBook = new WantedBook
            {
                Title = wantedBookRequest.Title,
                Description = wantedBookRequest.Description,
                LanguageId = wantedBookRequest.LanguageId,
                CategoryId = wantedBookRequest.CategoryId
            };

            await _context.WantedBooks!.AddAsync(newWantedBook);
            await _context.SaveChangesAsync();

            return ServiceResult<string>.SuccessMessageResult("Wanted book successfully created!");
        }

        /// <summary>
        /// Deletes a specific wanted book by its ID.
        /// </summary>
        /// <param name="id">The ID of the wanted book to delete.</param>
        /// <returns>A success message if the wanted book is deleted successfully.</returns>
        public async Task<ServiceResult<bool>> DeleteWantedBookAsync(int id)
        {
            var wantedBook = await _context.WantedBooks!.FindAsync(id);
            if (wantedBook == null)
            {
                return ServiceResult<bool>.FailureResult("Wanted book not found");
            }

            _context.WantedBooks.Remove(wantedBook);
            await _context.SaveChangesAsync();

            return ServiceResult<bool>.SuccessResult(true);
        }
    }
}
