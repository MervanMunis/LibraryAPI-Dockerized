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
    public class NationalityService : INationalityService
    {
        private readonly LibraryAPIContext _context;

        public NationalityService(LibraryAPIContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves all nationalities from the database.
        /// </summary>
        /// <returns>A list of nationality responses.</returns>
        public async Task<ServiceResult<IEnumerable<NationalityResponse>>> GetAllNationalitiesAsync()
        {
            var nationalities = await _context.Nationalities!
                .Select(n => new NationalityResponse()
                {
                    NationalityId = n.NationalityId,
                    Name = n.Name,
                    NationalityCode = n.NationalityCode
                })
                .ToListAsync();
            return ServiceResult<IEnumerable<NationalityResponse>>.SuccessResult(nationalities);
        }

        /// <summary>
        /// Retrieves a specific nationality by its ID.
        /// </summary>
        /// <param name="id">The ID of the nationality.</param>
        /// <returns>The nationality details.</returns>
        public async Task<ServiceResult<NationalityResponse>> GetNationalityByIdAsync(short id)
        {
            var nationality = await _context.Nationalities!
                .Select(n => new NationalityResponse()
                {
                    NationalityId = n.NationalityId,
                    Name = n.Name,
                    NationalityCode = n.NationalityCode
                })
                .FirstOrDefaultAsync(nationality => nationality.NationalityId == id);

            if (nationality == null)
            {
                return ServiceResult<NationalityResponse>.FailureResult("Nationality not found");
            }

            return ServiceResult<NationalityResponse>.SuccessResult(nationality);
        }

        /// <summary>
        /// Adds a new nationality to the database.
        /// </summary>
        /// <param name="nationalityRequest">The nationality details.</param>
        /// <returns>A success message if the nationality is created successfully.</returns>
        public async Task<ServiceResult<string>> AddNationalityAsync(NationalityRequest nationalityRequest)
        {
            if (await _context.Nationalities!.AnyAsync(n => n.Name == nationalityRequest.Name || n.NationalityCode == nationalityRequest.NationalityCode))
            {
                return ServiceResult<string>.FailureResult("Nationality with the specified name or code already exists");
            }

            Nationality nationality = new Nationality()
            {
                Name = nationalityRequest.Name,
                NationalityCode = nationalityRequest.NationalityCode
            };


            _context.Nationalities!.Add(nationality);
            await _context.SaveChangesAsync();
            return ServiceResult<string>.SuccessMessageResult("Nationality successfully created!");
        }

        /// <summary>
        /// Updates an existing nationality's details.
        /// </summary>
        /// <param name="id">The ID of the nationality to update.</param>
        /// <param name="nationalityRequest">The updated nationality details.</param>
        /// <returns>A success result if the nationality is updated successfully.</returns>
        public async Task<ServiceResult<bool>> UpdateNationalityAsync(short id, NationalityRequest nationalityRequest)
        {
            var existingNationality = await _context.Nationalities!.FindAsync(id);
            if (existingNationality == null)
            {
                return ServiceResult<bool>.FailureResult("Nationality not found");
            }

            existingNationality.Name = nationalityRequest.Name;
            existingNationality.NationalityCode = nationalityRequest.NationalityCode;

            _context.Entry(existingNationality).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return ServiceResult<bool>.SuccessResult(true);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Nationalities.AnyAsync(n => n.NationalityId == id))
                {
                    return ServiceResult<bool>.FailureResult("Nationality not found");
                }
                else
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// Deletes a nationality by its ID.
        /// </summary>
        /// <param name="id">The ID of the nationality to delete.</param>
        /// <returns>A success result if the nationality is deleted successfully.</returns>
        public async Task<ServiceResult<bool>> DeleteNationalityAsync(short id)
        {
            var nationality = await _context.Nationalities!.FindAsync(id);

            if (nationality == null)
            {
                return ServiceResult<bool>.FailureResult("Nationality not found");
            }

            var hasAuthors = await _context.Authors!.AnyAsync(a => a.Language!.NationalityId == id);

            if (hasAuthors)
            {
                return ServiceResult<bool>.FailureResult("Cannot delete nationality with associated authors.");
            }

            _context.Nationalities.Remove(nationality);
            await _context.SaveChangesAsync();

            return ServiceResult<bool>.SuccessResult(true);
        }

        /// <summary>
        /// Retrieves authors by their nationality ID.
        /// </summary>
        /// <param name="id">The ID of the nationality.</param>
        /// <returns>A list of authors associated with the specified nationality.</returns>
        public async Task<ServiceResult<IEnumerable<AuthorResoponse>>> GetAuthorsByNationalityIdAsync(short id)
        {
            var authors = await _context.Authors!
                .Include(ab => ab.AuthorBooks)!
                    .ThenInclude(b => b.Book)
                .Where(a => a.Language!.NationalityId == id)
                .Select(author => new AuthorResoponse()
                {
                    AuthorId = author.AuthorId,
                    FullName = author.FullName,
                    Biography = author.Biography,
                    BirthYear = author.BirthYear,
                    DeathYear = author.DeathYear.ToString(),
                    Title = author.AuthorBooks!.Select(b => b.Book!.Title).ToList()
                })
                .ToListAsync();

            if (!authors.Any())
            {
                return ServiceResult<IEnumerable<AuthorResoponse>>.FailureResult("No authors found for this nationality");
            }

            return ServiceResult<IEnumerable<AuthorResoponse>>.SuccessResult(authors);
        }
    }
}
