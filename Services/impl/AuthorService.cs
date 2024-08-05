using Microsoft.EntityFrameworkCore;
using LibraryAPI.Data;
using LibraryAPI.Exceptions;
using LibraryAPI.Services.Interfaces;
using System.Linq.Expressions;
using LibraryAPI.Models.Enums;
using LibraryAPI.Models.DTOs.Request;
using LibraryAPI.Models.DTOs.Response;
using LibraryAPI.Models.Entities;

namespace LibraryAPI.Services.impl
{
    /// <summary>
    /// Service class for handling author-related operations.
    /// </summary>
    public class AuthorService : IAuthorService
    {
        private readonly LibraryAPIContext _context;
        private readonly IFileService _fileService;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthorService"/> class.
        /// </summary>
        /// <param name="context">The database context for accessing the database.</param>
        /// <param name="fileService">The service for handling file operations.</param>
        public AuthorService(LibraryAPIContext context, IFileService fileService)
        {
            _context = context;
            _fileService = fileService;
        }

        /// <summary>
        /// Retrieves all authors.
        /// </summary>
        /// <returns>A service result containing a collection of all authors.</returns>
        public async Task<ServiceResult<IEnumerable<AuthorResoponse>>> GetAllAuthorsAsync()
        {
            var authors = await GetAuthorsAsync(_ => true);
            return ServiceResult<IEnumerable<AuthorResoponse>>.SuccessResult(authors);
        }

        /// <summary>
        /// Retrieves all active authors.
        /// </summary>
        /// <returns>A service result containing a collection of active authors.</returns>
        public async Task<ServiceResult<IEnumerable<AuthorResoponse>>> GetAllActiveAuthorsAsync()
        {
            var authors = await GetAuthorsAsync(_ => _.AuthroStatus == Status.Active.ToString());
            return ServiceResult<IEnumerable<AuthorResoponse>>.SuccessResult(authors);
        }

        /// <summary>
        /// Retrieves all inactive authors.
        /// </summary>
        /// <returns>A service result containing a collection of inactive authors.</returns>
        public async Task<ServiceResult<IEnumerable<AuthorResoponse>>> GetAllInActiveAuthorsAsync()
        {
            var authors = await GetAuthorsAsync(_ => _.AuthroStatus == Status.InActive.ToString());
            return ServiceResult<IEnumerable<AuthorResoponse>>.SuccessResult(authors);
        }

        /// <summary>
        /// Retrieves all banned authors.
        /// </summary>
        /// <returns>A service result containing a collection of inactive authors.</returns>
        public async Task<ServiceResult<IEnumerable<AuthorResoponse>>> GetAllBannedAuthorsAsync()
        {
            var authors = await GetAuthorsAsync(_ => _.AuthroStatus == Status.Banned.ToString());
            return ServiceResult<IEnumerable<AuthorResoponse>>.SuccessResult(authors);
        }

        /// <summary>
        /// Retrieves an author by their ID.
        /// </summary>
        /// <param name="id">The ID of the author to retrieve.</param>
        /// <returns>A service result containing the author details if found, otherwise an error message.</returns>
        public async Task<ServiceResult<AuthorResoponse>> GetAuthorByIdAsync(long id)
        {
            var author = await _context.Authors!
                .Where(a => a.AuthorId == id)
                .Include(a => a.Language)
                .Include(a => a.AuthorBooks)!
                    .ThenInclude(ab => ab.Book)
                .Select(a => new AuthorResoponse
                {
                    AuthorId = a.AuthorId,
                    FullName = a.FullName,
                    Biography = a.Biography,
                    BirthYear = a.BirthYear,
                    DeathYear = a.DeathYear.ToString(),
                    Title = a.AuthorBooks!.Select(b => b.Book!.Title).ToList()
                }).FirstOrDefaultAsync();

            if (author == null)
            {
                return ServiceResult<AuthorResoponse>.FailureResult("Author not found");
            }

            var authorEntity = await _context.Authors!.FindAsync(id);
            if (authorEntity!.AuthroStatus != Status.Active.ToString())
            {
                return ServiceResult<AuthorResoponse>.FailureResult("The author is not active anymore!");
            }

            return ServiceResult<AuthorResoponse>.SuccessResult(author);
        }

        /// <summary>
        /// Adds a new author to the database.
        /// </summary>
        /// <param name="authorRequest">The author data to add.</param>
        /// <returns>A service result indicating success or failure of the operation.</returns>
        public async Task<ServiceResult<string>> AddAuthorAsync(AuthorRequest authorRequest)
        {
            // Validate that the death year is not in the future
            if (authorRequest.DeathYear.HasValue && authorRequest.DeathYear > DateTime.Now.Year)
            {
                return ServiceResult<string>.FailureResult("The author's death year cannot be in the future!");
            }

            // Create a new Author entity
            Author author = new(
                authorRequest.FullName,
                authorRequest.Biography,
                authorRequest.BirthYear,
                authorRequest.DeathYear,
                authorRequest.LanguageId
                );

            // Add and save the new author
            await _context.Authors!.AddAsync(author);
            await _context.SaveChangesAsync();

            return ServiceResult<string>.SuccessMessageResult("Author successfully created!");
        }

        /// <summary>
        /// Updates an existing author's details.
        /// </summary>
        /// <param name="id">The ID of the author to update.</param>
        /// <param name="authorRequest">The new author data.</param>
        /// <returns>A service result indicating success or failure of the operation.</returns>
        public async Task<ServiceResult<bool>> UpdateAuthorAsync(long id, AuthorRequest authorRequest)
        {
            var existingAuthor = await _context.Authors!.FindAsync(id);

            if (existingAuthor == null)
            {
                return ServiceResult<bool>.FailureResult("Author not found");
            }

            // Validate that the death year is not in the future
            if (authorRequest.DeathYear.HasValue && authorRequest.DeathYear > DateTime.Now.Year)
            {
                return ServiceResult<bool>.FailureResult("The author's death year cannot be in the future!");
            }

            // Update the author details
            existingAuthor.FullName = authorRequest.FullName;
            existingAuthor.Biography = authorRequest.Biography;
            existingAuthor.BirthYear = authorRequest.BirthYear;
            existingAuthor.DeathYear = authorRequest.DeathYear;
            existingAuthor.LanguageId = authorRequest.LanguageId;

            _context.Update(existingAuthor).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return ServiceResult<bool>.SuccessResult(true);
            }
            catch (DbUpdateConcurrencyException)
            {
                // Handle concurrency issues and validate if the author still exists
                if (!await _context.Authors.AnyAsync(a => a.AuthorId == id))
                {
                    return ServiceResult<bool>.FailureResult("Author not found");
                }
                else
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// Updates the status of an author and related books.
        /// </summary>
        /// <param name="id">The ID of the author whose status is to be updated.</param>
        /// <param name="status">The new status of the author.</param>
        /// <returns>A service result indicating success or failure of the operation.</returns>
        public async Task<ServiceResult<bool>> SetAuthorStatusAsync(long id, string status)
        {
            var author = await _context.Authors!.FindAsync(id);
            if (author == null)
            {
                return ServiceResult<bool>.FailureResult("Author not found");
            }

            // Update the author's status
            author.AuthroStatus = status;
            _context.Update(author).State = EntityState.Modified;

            // Update the status of related books
            var authorBooks = await _context.AuthorBook!
                .Where(ab => ab.AuthorsId == id)
                .Include(ab => ab.Book)
                .ToListAsync();

            foreach (var authorBook in authorBooks)
            {
                authorBook.Book!.BookStatus = status;
                _context.Update(authorBook.Book).State = EntityState.Modified;
            }

            try
            {
                await _context.SaveChangesAsync();
                return ServiceResult<bool>.SuccessResult(true);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Authors.AnyAsync(e => e.AuthorId == id))
                {
                    return ServiceResult<bool>.FailureResult("Author not found");
                }
                else
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// Updates the image associated with an author.
        /// </summary>
        /// <param name="id">The ID of the author whose image is to be updated.</param>
        /// <param name="image">The new image file.</param>
        /// <returns>A service result indicating success or failure of the operation.</returns>
        public async Task<ServiceResult<bool>> UpdateAuthorImageAsync(long id, IFormFile image)
        {
            var author = await _context.Authors!.FindAsync(id);
            if (author == null)
            {
                return ServiceResult<bool>.FailureResult("Author not found");
            }

            // Save the new image file
            var fileName = await _fileService.SaveFileAsync(image, "AuthorImages");

            // Update the author's image file name
            author.ImageFileName = fileName;
            _context.Update(author).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return ServiceResult<bool>.SuccessResult(true);
            }
            catch (DbUpdateConcurrencyException)
            {
                // Handle concurrency issues and validate if the author still exists
                if (!await _context.Authors.AnyAsync(e => e.AuthorId == id))
                {
                    return ServiceResult<bool>.FailureResult("Author not found");
                }
                else
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// Retrieves the image of an author by author ID.
        /// </summary>
        /// <param name="authorId">The ID of the author.</param>
        /// <returns>The image as a byte array.</returns>
        public async Task<ServiceResult<byte[]>> GetAuthorImageAsync(long authorId)
        {
            try
            {
                var imageBytes = await _fileService.GetImageByAuthorIdAsync(authorId);
                return ServiceResult<byte[]>.SuccessResult(imageBytes);
            }
            catch (FileNotFoundException ex)
            {
                return ServiceResult<byte[]>.FailureResult(ex.Message);
            }
        }

        /// <summary>
        /// Retrieves authors based on a specified filter.
        /// </summary>
        /// <param name="filterPredicate">The predicate to filter authors.</param>
        /// <returns>A collection of authors matching the filter.</returns>
        private async Task<IEnumerable<AuthorResoponse>> GetAuthorsAsync(Expression<Func<Author, bool>> filterPredicate)
        {
            var authors = await _context.Authors!
                .Where(filterPredicate)
                .Include(a => a.Language)
                .Include(a => a.AuthorBooks)!
                    .ThenInclude(a => a.Book)
                .Select(a => new AuthorResoponse
                {
                    AuthorId = a.AuthorId,
                    FullName = a.FullName,
                    Biography = a.Biography,
                    BirthYear = a.BirthYear,
                    DeathYear = a.DeathYear.HasValue ? a.DeathYear.ToString() : "Still Lives.",
                    AuthroStatus = a.AuthroStatus,
                    Title = a.AuthorBooks!.Select(b => b.Book!.Title).ToList()
                }).ToListAsync();

            return authors;
        }
    }
}
