using LibraryAPI.Data;
using LibraryAPI.DTOs.Request;
using LibraryAPI.DTOs.Response;
using LibraryAPI.Entities.Enums;
using LibraryAPI.Entities.Models;
using LibraryAPI.Exceptions;
using LibraryAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol;
using static System.Reflection.Metadata.BlobBuilder;

namespace LibraryAPI.Services.impl
{
    public class PublisherService : IPublisherService
    {
        private readonly LibraryAPIContext _context;

        public PublisherService(LibraryAPIContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves all publishers from the database.
        /// </summary>
        /// <returns>A list of active publishers.</returns>
        public async Task<ServiceResult<IEnumerable<PublisherResponse>>> GetAllPublishersAsync()
        {
            var publishers = await _context.Publishers!
                .Where(publisher => publisher.PublisherStatus == Status.Active.ToString())
                .Include(publisher => publisher.Books)
                .Include(publisher => publisher.Addresses)
                .Select(p => new PublisherResponse()
                {
                    PublisherId = p.PublisherId,
                    Name = p.Name,
                    Phone = p.Phone,
                    Email = p.Email,
                    ContactPerson = p.ContactPerson,
                    PublisherStatus = p.PublisherStatus,
                    Addresses = p.Addresses!.Select(a => new PublisherAddressResponse
                    {
                        PublisherAddressId = a.PublisherAddressId,
                        Street = a.Street,
                        City = a.City,
                        Country = a.Country,
                        PostalCode = a.PostalCode
                    }).ToList(),
                    Books = p.Books!.Select(b => b.Title).ToList()
                })
                .ToListAsync();

            return ServiceResult<IEnumerable<PublisherResponse>>.SuccessResult(publishers);
        }

        /// <summary>
        /// Retrieves a specific publisher by their ID.
        /// </summary>
        /// <param name="id">The ID of the publisher.</param>
        /// <returns>The details of the publisher.</returns>
        public async Task<ServiceResult<PublisherResponse>> GetPublisherByIdAsync(long id)
        {
            var publisher = await _context.Publishers!
                .Include(p => p.Books)
                .Include(p => p.Addresses)
                .Select(p => new PublisherResponse()
                {
                    PublisherId = p.PublisherId,
                    Name = p.Name,
                    Phone = p.Phone,
                    Email = p.Email,
                    ContactPerson = p.ContactPerson,
                    PublisherStatus = p.PublisherStatus,
                    Addresses = p.Addresses!.Select(a => new PublisherAddressResponse
                    {
                        PublisherAddressId = a.PublisherAddressId,
                        Street = a.Street,
                        City = a.City,
                        Country = a.Country,
                        PostalCode = a.PostalCode
                    }).ToList(),
                    Books = p.Books!.Select(b => b.Title).ToList()
                })
                .FirstOrDefaultAsync(p => p.PublisherId == id);

            if (publisher == null)
            {
                return ServiceResult<PublisherResponse>.FailureResult("Publisher not found");
            }

            if (publisher.PublisherStatus != Status.Active.ToString())
            {
                return ServiceResult<PublisherResponse>.FailureResult("The publisher is not active anymore!");
            }

            return ServiceResult<PublisherResponse>.SuccessResult(publisher);
        }

        /// <summary>
        /// Adds a new publisher to the database.
        /// </summary>
        /// <param name="publisherRequest">The details of the publisher to be added.</param>
        /// <returns>A success message if the publisher is added successfully.</returns>
        public async Task<ServiceResult<string>> AddPublisherAsync(PublisherRequest publisherRequest)
        {
            Publisher publisher = new Publisher()
            {
                Name = publisherRequest.Name,
                Phone = publisherRequest.Phone,
                Email = publisherRequest.Email,
                ContactPerson = publisherRequest.ContactPerson,

            };
            await _context.Publishers!.AddAsync(publisher);
            await _context.SaveChangesAsync();

            return ServiceResult<string>.SuccessMessageResult("Publisher successfully created!");
        }

        /// <summary>
        /// Updates an existing publisher's details.
        /// </summary>
        /// <param name="id">The ID of the publisher to be updated.</param>
        /// <param name="publisherRequest">The new details of the publisher.</param>
        /// <returns>A boolean indicating the success of the update.</returns>
        public async Task<ServiceResult<bool>> UpdatePublisherAsync(long id, PublisherRequest publisherRequest)
        {
            var existingPublisher = await _context.Publishers!.FindAsync(id);

            if (existingPublisher == null)
            {
                return ServiceResult<bool>.FailureResult("Publisher not found");
            }

            existingPublisher.Name = publisherRequest.Name;
            existingPublisher.Phone = publisherRequest.Phone;
            existingPublisher.Email = publisherRequest.Email;
            existingPublisher.ContactPerson = publisherRequest.ContactPerson;

            _context.Entry(existingPublisher).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return ServiceResult<bool>.SuccessResult(true);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Publishers.AnyAsync(p => p.PublisherId == id))
                {
                    return ServiceResult<bool>.FailureResult("Publisher not found");
                }
                else
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// Sets the status of a publisher.
        /// </summary>
        /// <param name="id">The ID of the publisher whose status is to be set.</param>
        /// <param name="status">The new status of the publisher.</param>
        /// <returns>A boolean indicating the success of the operation.</returns>
        public async Task<ServiceResult<bool>> SetPublisherStatusAsync(long id, string status)
        {
            var publisher = await _context.Publishers!.FindAsync(id);
            if (publisher == null)
            {
                return ServiceResult<bool>.FailureResult("Publisher not found");
            }

            if (status == Status.InActive.ToString() && (await _context.Books!.AnyAsync(b => b.PublisherId == id)))
            {
                return ServiceResult<bool>.FailureResult("Cannot deactivate publisher with active books.");
            }

            publisher.PublisherStatus = status;
            _context.Entry(publisher).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return ServiceResult<bool>.SuccessResult(true);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Publishers.AnyAsync(e => e.PublisherId == id))
                {
                    return ServiceResult<bool>.FailureResult("Publisher not found");
                }
                else
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// Retrieves all books published by a specific publisher.
        /// </summary>
        /// <param name="publisherId">The ID of the publisher.</param>
        /// <returns>A list of books published by the specified publisher.</returns>
        public async Task<ServiceResult<IEnumerable<CategoryBookResponse>>> GetBooksByPublisherIdAsync(long publisherId)
        {
            var books = await _context.Books!
                .Where(b => b.PublisherId == publisherId && b.BookStatus == Status.Active.ToString())
                .Select(b => new CategoryBookResponse()
                {
                    BookId = b.BookId,
                    ISBN = b.ISBN,
                    Title = b.Title,
                    SubCategoryNames = b.BookSubCategories!.Select(bs => bs.SubCategory!.Name).ToList()
                }).ToListAsync();

            return ServiceResult<IEnumerable<CategoryBookResponse>>.SuccessResult(books);
        }

        /// <summary>
        /// Retrieves the address of a specific publisher.
        /// </summary>
        /// <param name="publisherId">The ID of the publisher.</param>
        /// <returns>The address details of the specified publisher.</returns>
        public async Task<ServiceResult<PublisherAddress>> GetPublisherAddressAsync(long publisherId)
        {
            var address = await _context.PublisherAddresses!
                .FirstOrDefaultAsync(a => a.PublisherId == publisherId);

            if (address == null)
            {
                return ServiceResult<PublisherAddress>.FailureResult("Address not found");
            }

            return ServiceResult<PublisherAddress>.SuccessResult(address);
        }

        /// <summary>
        /// Adds or updates the address of a specific publisher.
        /// </summary>
        /// <param name="publisherId">The ID of the publisher whose address is to be added or updated.</param>
        /// <param name="address">The new address details of the publisher.</param>
        /// <returns>A success message if the address is added or updated successfully.</returns>
        public async Task<ServiceResult<string>> AddOrUpdatePublisherAddressAsync(long publisherId, PublisherAddressRequest address)
        {
            PublisherAddress publisherAddress = new PublisherAddress()
            {
                Street = address.Street,
                City = address.City,
                Country = address.Country,
                PostalCode = address.PostalCode,
                PublisherId = publisherId
            };
            await _context.PublisherAddresses!.AddAsync(publisherAddress);
            await _context.SaveChangesAsync();

            return ServiceResult<string>.SuccessMessageResult("Publisher address successfully added or updated!");
        }

    }
}
