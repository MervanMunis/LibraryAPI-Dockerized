using LibraryAPI.Entities.Models;
using Microsoft.AspNetCore.Mvc;
using LibraryAPI.Entities.Enums;
using Microsoft.AspNetCore.Authorization;
using LibraryAPI.Services.Interfaces;
using LibraryAPI.DTOs.Response;
using LibraryAPI.DTOs.Request;

namespace LibraryAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PublishersController : ControllerBase
    {
        private readonly IPublisherService _publisherService;

        public PublishersController(IPublisherService publisherService)
        {
            _publisherService = publisherService;
        }

        /// <summary>
        /// Retrieves all publishers.
        /// </summary>
        /// <returns>A list of publishers.</returns>
        [HttpGet] // GET: api/Publishers
        [Authorize(Roles = "Librarian, Member")]
        public async Task<ActionResult<IEnumerable<PublisherResponse>>> GetPublishers()
        {
            var result = await _publisherService.GetAllPublishersAsync();

            if (!result.Success)
            {
                return Problem(result.ErrorMessage);
            }

            return Ok(result.Data);
        }

        /// <summary>
        /// Retrieves a specific publisher by ID.
        /// </summary>
        /// <param name="id">The ID of the publisher.</param>
        /// <returns>The details of the publisher.</returns>
        [HttpGet("{id}")] // GET: api/Publishers/5
        [Authorize(Roles = "Librarian, Member")]
        public async Task<ActionResult<PublisherResponse>> GetPublisher(long id)
        {
            var result = await _publisherService.GetPublisherByIdAsync(id);

            if (!result.Success)
            {
                return NotFound(result.ErrorMessage);
            }

            return Ok(result.Data);
        }


        /// <summary>
        /// Retrieves all books published by a specific publisher.
        /// </summary>
        /// <param name="id">The ID of the publisher.</param>
        /// <returns>A list of books published by the specified publisher.</returns>
        [HttpGet("{id}/books")] // GET: api/Publishers/5/books
        [Authorize(Roles = "Librarian, Member")]
        public async Task<ActionResult<IEnumerable<CategoryBookResponse>>> GetBooksByPublisher(long id)
        {
            var result = await _publisherService.GetBooksByPublisherIdAsync(id);

            if (!result.Success)
            {
                return NotFound(result.ErrorMessage);
            }

            return Ok(result.Data);
        }

        /// <summary>
        /// Retrieves the address of a specific publisher.
        /// </summary>
        /// <param name="id">The ID of the publisher.</param>
        /// <returns>The address details of the specified publisher.</returns>
        [HttpGet("{id}/address")] // GET: api/Publishers/5/address
        [Authorize(Roles = "Librarian, Member")]
        public async Task<ActionResult<PublisherAddress>> GetPublisherAddress(long id)
        {
            var result = await _publisherService.GetPublisherAddressAsync(id);

            if (!result.Success)
            {
                return NotFound(result.ErrorMessage);
            }

            return Ok(result.Data);
        }


        /// <summary>
        /// Adds a new publisher.
        /// </summary>
        /// <param name="publisherRequest">The details of the publisher to be added.</param>
        /// <returns>A success message if the publisher is added successfully.</returns>
        [HttpPost] // POST: api/Publishers
        [Authorize(Roles = "Librarian")]
        public async Task<ActionResult<string>> PostPublisher([FromBody] PublisherRequest publisherRequest)
        {
            var result = await _publisherService.AddPublisherAsync(publisherRequest);

            if (!result.Success)
            {
                return BadRequest(result.ErrorMessage);
            }

            return Ok("The publisher is successfully created.");
        }


        /// <summary>
        /// Updates an existing publisher's details.
        /// </summary>
        /// <param name="id">The ID of the publisher to be updated.</param>
        /// <param name="publisherRequest">The new details of the publisher.</param>
        /// <returns>A success message if the publisher is updated successfully.</returns>
        [HttpPut("{id}")] // PUT: api/Publishers/5
        [Authorize(Roles = "Librarian")]
        public async Task<ActionResult<string>> PutPublisher(long id, [FromBody] PublisherRequest publisherRequest)
        {
            var result = await _publisherService.UpdatePublisherAsync(id, publisherRequest);

            if (!result.Success)
            {
                return BadRequest(result.ErrorMessage);
            }

            return Ok("The publisher is updated successfully.");
        }

        /// <summary>
        /// Sets a publisher's status to active.
        /// </summary>
        /// <param name="id">The ID of the publisher whose status is to be set to active.</param>
        /// <returns>A success message if the publisher is set to active status.</returns>
        [HttpPatch("{id}/status/active")] // PATCH: api/Publishers/5/status/active
        [Authorize(Roles = "Librarian")]
        public async Task<ActionResult<string>> ActivePublisher(long id)
        {
            var result = await _publisherService.SetPublisherStatusAsync(id, Status.Active.ToString());

            if (!result.Success)
            {
                return BadRequest(result.ErrorMessage);
            }

            return Ok("The publisher and their books are set to active status.");
        }

        /// <summary>
        /// Sets a publisher's status to inactive.
        /// </summary>
        /// <param name="id">The ID of the publisher whose status is to be set to inactive.</param>
        /// <returns>A success message if the publisher is set to inactive status.</returns>
        [HttpPatch("{id}/status/InActive")] // PATCH: api/Publishers/5/status/InActive
        [Authorize(Roles = "Librarian")]
        public async Task<ActionResult<string>> InActivePublisher(long id)
        {
            var result = await _publisherService.SetPublisherStatusAsync(id, Status.InActive.ToString());

            if (!result.Success)
            {
                return BadRequest(result.ErrorMessage);
            }

            return Ok("The publisher is set to not active status.");
        }


        /// <summary>
        /// Adds or updates the address of a specific publisher.
        /// </summary>
        /// <param name="id">The ID of the publisher whose address is to be added or updated.</param>
        /// <param name="address">The new address details of the publisher.</param>
        /// <returns>A success message if the address is added or updated successfully.</returns>
        [HttpPut("{id}/address")] // PUT: api/Publishers/5/address
        [Authorize(Roles = "Librarian")]
        public async Task<ActionResult<string>> AddOrUpdatePublisherAddress(long id, [FromBody] PublisherAddressRequest address)
        {
            var result = await _publisherService.AddOrUpdatePublisherAddressAsync(id, address);

            if (!result.Success)
            {
                return BadRequest(result.ErrorMessage);
            }

            return Ok("The publisher's address is successfully added/updated.");
        }
    }
}
