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
    public class LanguageService : ILanguageService
    {
        private readonly LibraryAPIContext _context;

        public LanguageService(LibraryAPIContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves all active languages.
        /// </summary>
        /// <returns>A list of active languages.</returns>
        public async Task<ServiceResult<IEnumerable<LanguageResponse>>> GetAllLanguagesAsync()
        {
            var languages = await _context.Languages!
                .Where(language => language.LanguageStatus == Status.Active.ToString())
                .Include(language => language.Nationality)
                .Select(l => new LanguageResponse()
                {
                    LanguageId = l.LanguageId,
                    Name = l.Name,
                    LanguageStatus = l.LanguageStatus,
                    NationalityName = l.Nationality!.Name
                })
                .ToListAsync();

            return ServiceResult<IEnumerable<LanguageResponse>>.SuccessResult(languages);
        }

        /// <summary>
        /// Retrieves a specific language by its ID.
        /// </summary>
        /// <param name="id">The ID of the language.</param>
        /// <returns>The language details.</returns>
        public async Task<ServiceResult<LanguageResponse>> GetLanguageByIdAsync(short id)
        {
            var language = await _context.Languages!
                .Where(language => language.LanguageStatus == Status.Active.ToString())
                .Include(l => l.Nationality)
                .Select(l => new LanguageResponse()
                {
                    LanguageId = l.LanguageId,
                    Name = l.Name,
                    LanguageStatus = l.LanguageStatus,
                    NationalityName = l.Nationality!.Name
                })
                .FirstOrDefaultAsync(l => l.LanguageId == id);

            if (language == null)
            {
                return ServiceResult<LanguageResponse>.FailureResult("Language not found");
            }

            if (language.LanguageStatus != Status.Active.ToString())
            {
                return ServiceResult<LanguageResponse>.FailureResult("The language is not active anymore!");
            }

            return ServiceResult<LanguageResponse>.SuccessResult(language);
        }

        /// <summary>
        /// Adds a new language to the database.
        /// </summary>
        /// <param name="languageRequest">The language details.</param>
        /// <returns>A success message if the language is created successfully.</returns>
        public async Task<ServiceResult<string>> AddLanguageAsync(LanguageRequest languageRequest)
        {
            if (await _context.Languages!.AnyAsync(l => l.Name == languageRequest.Name))
            {
                return ServiceResult<string>.FailureResult("Language with the specified name already exists!");
            }

            Language language = new Language()
            {
                Name = languageRequest.Name,
                NationalityId = languageRequest.NationalityId,
            };

            await _context.Languages!.AddAsync(language);
            await _context.SaveChangesAsync();

            return ServiceResult<string>.SuccessMessageResult("Language successfully created!");
        }

        /// <summary>
        /// Updates an existing language's details.
        /// </summary>
        /// <param name="id">The ID of the language to update.</param>
        /// <param name="languageRequest">The updated language details.</param>
        /// <returns>A success result if the update is successful.</returns>
        public async Task<ServiceResult<bool>> UpdateLanguageAsync(short id, LanguageRequest languageRequest)
        {
            var existingLanguage = await _context.Languages!.FindAsync(id);

            if (existingLanguage == null)
            {
                return ServiceResult<bool>.FailureResult("Language not found");
            }

            existingLanguage.Name = languageRequest.Name;
            existingLanguage.NationalityId = languageRequest.NationalityId;

            _context.Entry(existingLanguage).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return ServiceResult<bool>.SuccessResult(true);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Languages.AnyAsync(l => l.LanguageId == id))
                {
                    return ServiceResult<bool>.FailureResult("Language not found");
                }
                else
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// Sets the status of a language.
        /// </summary>
        /// <param name="id">The ID of the language to update.</param>
        /// <param name="status">The new status of the language.</param>
        /// <returns>A success result if the update is successful.</returns>
        public async Task<ServiceResult<bool>> SetLanguageStatusAsync(short id, string status)
        {
            var language = await _context.Languages!.FindAsync(id);
            if (language == null)
            {
                return ServiceResult<bool>.FailureResult("Language not found");
            }

            language.LanguageStatus = status;
            _context.Entry(language).State = EntityState.Modified;

            var bookLanguages = await _context.BookLanguage!
                .Where(bl => bl.LanguagesId == id)
                .Include(bl => bl.Book)
                .ToListAsync();

            foreach (var bookLanguage in bookLanguages)
            {
                bookLanguage.Book!.BookStatus = status;
                _context.Entry(bookLanguage.Book).State = EntityState.Modified;
            }

            try
            {
                await _context.SaveChangesAsync();
                return ServiceResult<bool>.SuccessResult(true);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Languages.AnyAsync(l => l.LanguageId == id))
                {
                    return ServiceResult<bool>.FailureResult("Language not found");
                }
                else
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// Retrieves books associated with a specific language.
        /// </summary>
        /// <param name="languageId">The ID of the language.</param>
        /// <returns>A list of books associated with the language.</returns>
        public async Task<ServiceResult<IEnumerable<BookResponse>>> GetBooksByLanguageIdAsync(short languageId)
        {
            var books = await _context.Books!
                .Include(b => b.BookLanguages)!
                    .ThenInclude(bl => bl.Language)
                .Where(b => b.BookLanguages!.Any(bl => bl.LanguagesId == languageId))
                .Select(b => new BookResponse()
                {
                    BookId = b.BookId,
                    ISBN = b.ISBN,
                    Title = b.Title
                })
                .ToListAsync();

            if (!books.Any())
            {
                return ServiceResult<IEnumerable<BookResponse>>.FailureResult("No books found for this language");
            }

            return ServiceResult<IEnumerable<BookResponse>>.SuccessResult(books);
        }
    }
}
