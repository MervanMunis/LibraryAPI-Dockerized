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
    public class LocationService : ILocationService
    {
        private readonly LibraryAPIContext _context;

        public LocationService(LibraryAPIContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves all active locations from the database.
        /// </summary>
        /// <returns>A list of active locations.</returns>
        public async Task<ServiceResult<IEnumerable<LocationResponse>>> GetAllLocationsAsync()
        {
            var locations = await _context.Locations!
                .Where(location => location.LocationStatus == Status.Active.ToString())
                .Select(l => new LocationResponse()
                {
                    LocationId = l.LocationId,
                    SectionCode = l.SectionCode,
                    AisleCode = l.AisleCode,
                    ShelfNumber = l.ShelfNumber,
                    LocationStatus = l.LocationStatus
                })
                .ToListAsync();

            return ServiceResult<IEnumerable<LocationResponse>>.SuccessResult(locations);
        }

        /// <summary>
        /// Retrieves a specific location by its ID.
        /// </summary>
        /// <param name="id">The ID of the location.</param>
        /// <returns>The location details.</returns>
        public async Task<ServiceResult<LocationResponse>> GetLocationByIdAsync(int id)
        {
            var location = await _context.Locations!
                .Include(location => location.Books)
                .Select(l => new LocationResponse()
                {
                    LocationId = l.LocationId,
                    SectionCode = l.SectionCode,
                    AisleCode = l.AisleCode,
                    ShelfNumber = l.ShelfNumber,
                    LocationStatus = l.LocationStatus
                })
                .FirstOrDefaultAsync(location => location.LocationId == id);

            if (location == null || location.LocationStatus != Status.Active.ToString())
            {
                return ServiceResult<LocationResponse>.FailureResult("Location not found");
            }

            return ServiceResult<LocationResponse>.SuccessResult(location);
        }

        /// <summary>
        /// Adds a new location to the database.
        /// </summary>
        /// <param name="locationRequest">The location details.</param>
        /// <returns>A success message if the location is created successfully.</returns>
        public async Task<ServiceResult<string>> AddLocationAsync(LocationRequest locationRequest)
        {
            Location location = new Location()
            {
                SectionCode = locationRequest.SectionCode,
                AisleCode = locationRequest.AisleCode,
                ShelfNumber = locationRequest.ShelfNumber,
            };
            await _context.Locations!.AddAsync(location);
            await _context.SaveChangesAsync();

            return ServiceResult<string>.SuccessMessageResult("Location successfully created!");
        }

        /// <summary>
        /// Updates an existing location's details.
        /// </summary>
        /// <param name="id">The ID of the location to update.</param>
        /// <param name="locationRequest">The location update details.</param>
        /// <returns>A success message if the location is updated successfully.</returns>
        public async Task<ServiceResult<bool>> UpdateLocationAsync(int id, LocationRequest locationRequest)
        {
            var existingLocation = await _context.Locations!.FindAsync(id);

            if (existingLocation == null)
            {
                return ServiceResult<bool>.FailureResult("Location not found");
            }

            existingLocation.SectionCode = locationRequest.SectionCode;
            existingLocation.AisleCode = locationRequest.AisleCode;
            existingLocation.ShelfNumber = locationRequest.ShelfNumber;

            _context.Update(existingLocation).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return ServiceResult<bool>.SuccessResult(true);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Locations.AnyAsync(location => location.LocationId == id))
                {
                    return ServiceResult<bool>.FailureResult("Location not found");
                }
                else
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// Sets the status of a location.
        /// </summary>
        /// <param name="id">The ID of the location to update.</param>
        /// <param name="status">The new status of the location.</param>
        /// <returns>A success message if the location status is updated successfully.</returns>
        public async Task<ServiceResult<bool>> SetLocationStatusAsync(int id, Status status)
        {
            var location = await _context.Locations!.FindAsync(id);
            if (location == null)
            {
                return ServiceResult<bool>.FailureResult("Location not found");
            }

            if (status == Status.InActive)
            {
                var booksInLocation = await _context.Books!
                    .Where(book => book.LocationId == id && book.BookStatus == Status.Active.ToString())
                    .ToListAsync();

                if (booksInLocation.Any())
                {
                    return ServiceResult<bool>.FailureResult("Location cannot be set to not active as there are books in this location.");
                }
            }

            location.LocationStatus = status.ToString();
            _context.Update(location).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return ServiceResult<bool>.SuccessResult(true);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Locations.AnyAsync(e => e.LocationId == id))
                {
                    return ServiceResult<bool>.FailureResult("Location not found");
                }
                else
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// Retrieves books by section code.
        /// </summary>
        /// <param name="sectionCode">The section code to filter by.</param>
        /// <returns>A list of books in the specified section code.</returns>
        public async Task<ServiceResult<IEnumerable<BookResponse>>> GetBooksBySectionCodeAsync(string sectionCode)
        {
            var books = await _context.Books!
                .Include(book => book.Location)
                .Where(book => book.Location!.SectionCode == sectionCode && book.BookStatus == Status.Active.ToString())
                .Select(b => new BookResponse()
                {
                    BookId = b.BookId,
                    ISBN = b.ISBN,
                    Title = b.Title
                })
                .ToListAsync();

            if (!books.Any())
            {
                return ServiceResult<IEnumerable<BookResponse>>.FailureResult("No books found for this section code");
            }

            return ServiceResult<IEnumerable<BookResponse>>.SuccessResult(books);
        }

        /// <summary>
        /// Retrieves books by aisle code.
        /// </summary>
        /// <param name="aisleCode">The aisle code to filter by.</param>
        /// <returns>A list of books in the specified aisle code.</returns>
        public async Task<ServiceResult<IEnumerable<BookResponse>>> GetBooksByAisleCodeAsync(string aisleCode)
        {
            var books = await _context.Books!
                .Include(book => book.Location)
                .Where(book => book.Location!.AisleCode == aisleCode && book.BookStatus == Status.Active.ToString())
                .Select(b => new BookResponse()
                {
                    BookId = b.BookId,
                    ISBN = b.ISBN,
                    Title = b.Title
                })
                .ToListAsync();

            if (!books.Any())
            {
                return ServiceResult<IEnumerable<BookResponse>>.FailureResult("No books found for this aisle code");
            }

            return ServiceResult<IEnumerable<BookResponse>>.SuccessResult(books);
        }

        /// <summary>
        /// Retrieves books by shelf number.
        /// </summary>
        /// <param name="shelfNumber">The shelf number to filter by.</param>
        /// <returns>A list of books in the specified shelf number.</returns>
        public async Task<ServiceResult<IEnumerable<BookResponse>>> GetBooksByShelfNumberAsync(string shelfNumber)
        {
            var books = await _context.Books!
                .Include(book => book.Location)
                .Where(book => book.Location!.ShelfNumber == shelfNumber && book.BookStatus == Status.Active.ToString())
                .Select(b => new BookResponse()
                {
                    BookId = b.BookId,
                    ISBN = b.ISBN,
                    Title = b.Title
                })
                .ToListAsync();

            if (!books.Any())
            {
                return ServiceResult<IEnumerable<BookResponse>>.FailureResult("No books found for this shelf number");
            }

            return ServiceResult<IEnumerable<BookResponse>>.SuccessResult(books);
        }
    }
}
