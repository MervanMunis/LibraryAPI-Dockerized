using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using LibraryAPI.Services.Interfaces;
using LibraryAPI.Models.Enums;
using LibraryAPI.Models.DTOs.Request;
using LibraryAPI.Models.DTOs.Response;
using LibraryAPI.Models.Entities;

namespace LibraryAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LocationsController : ControllerBase
    {
        private readonly ILocationService _locationService;

        public LocationsController(ILocationService locationService)
        {
            _locationService = locationService;
        }

        /// <summary>
        /// Retrieves all active locations.
        /// </summary>
        /// <returns>A list of active locations.</returns>
        [HttpGet] // GET: api/Locations
        [Authorize(Roles = "Librarian, Member")]
        public async Task<ActionResult<IEnumerable<LocationResponse>>> GetLocations()
        {
            var result = await _locationService.GetAllLocationsAsync();

            if (!result.Success)
            {
                return Problem(result.ErrorMessage);
            }

            return Ok(result.Data);
        }

        /// <summary>
        /// Retrieves a specific location by its ID.
        /// </summary>
        /// <param name="id">The ID of the location.</param>
        /// <returns>The location details.</returns>
        [HttpGet("{id}")] // GET: api/Locations/5
        [Authorize(Roles = "Librarian, Member")]
        public async Task<ActionResult<LocationResponse>> GetLocation(int id)
        {
            var result = await _locationService.GetLocationByIdAsync(id);

            if (!result.Success)
            {
                return NotFound(result.ErrorMessage);
            }

            return Ok(result.Data);
        }


        /// <summary>
        /// Adds a new location to the database.
        /// </summary>
        /// <param name="locationRequest">The location details.</param>
        /// <returns>A success message if the location is created successfully.</returns>
        [HttpPost] // POST: api/Locations
        [Authorize(Roles = "Librarian")]
        public async Task<ActionResult<string>> PostLocation([FromBody] LocationRequest locationRequest)
        {
            var result = await _locationService.AddLocationAsync(locationRequest);

            if (!result.Success)
            {
                return BadRequest(result.ErrorMessage);
            }

            return Ok("Location successfully created.");
        }

        /// <summary>
        /// Updates an existing location's details.
        /// </summary>
        /// <param name="id">The ID of the location to update.</param>
        /// <param name="locationRequest">The location update details.</param>
        /// <returns>A success message if the location is updated successfully.</returns>
        [HttpPut("{id}")] // PUT: api/Locations/5
        [Authorize(Roles = "Librarian")]
        public async Task<ActionResult<string>> PutLocation(int id, [FromBody] LocationRequest locationRequest)
        {
            var result = await _locationService.UpdateLocationAsync(id, locationRequest);

            if (!result.Success)
            {
                return BadRequest(result.ErrorMessage);
            }

            return Ok("Location successfully updated.");
        }

        /// <summary>
        /// Sets the status of a location to inactive.
        /// </summary>
        /// <param name="id">The ID of the location.</param>
        /// <returns>A success message if the location is set to inactive successfully.</returns>
        [HttpPatch("{id}/status/inactive")] // PATCH: api/Locations/5/status
        [Authorize(Roles = "Librarian")]
        public async Task<ActionResult<string>> InActiveLocation(int id)
        {
            var result = await _locationService.SetLocationStatusAsync(id, Status.InActive);

            if (!result.Success)
            {
                return NotFound(result.ErrorMessage);
            }

            return Ok("Location is set to inactive.");
        }

        /// <summary>
        /// Sets the status of a location to active.
        /// </summary>
        /// <param name="id">The ID of the location.</param>
        /// <returns>A success message if the location is set to active successfully.</returns>
        [HttpPatch("{id}/status/active")] // PATCH: api/Locations/5/status/active
        [Authorize(Roles = "Librarian")]
        public async Task<ActionResult<string>> ActiveLocation(int id)
        {
            var result = await _locationService.SetLocationStatusAsync(id, Status.Active);

            if (!result.Success)
            {
                return BadRequest(result.ErrorMessage);
            }

            return Ok("Location is set to active.");
        }


        /// <summary>
        /// Retrieves books by section code.
        /// </summary>
        /// <param name="sectionCode">The section code to filter by.</param>
        /// <returns>A list of books in the specified section code.</returns>
        [HttpGet("section/{sectionCode}")] // GET: api/Locations/section/{sectionCode}
        [Authorize(Roles = "Librarian, Member")]
        public async Task<ActionResult<IEnumerable<Book>>> GetBooksBySectionCode(string sectionCode)
        {
            var result = await _locationService.GetBooksBySectionCodeAsync(sectionCode);

            if (!result.Success)
            {
                return NotFound(result.ErrorMessage);
            }

            return Ok(result.Data);
        }

        /// <summary>
        /// Retrieves books by aisle code.
        /// </summary>
        /// <param name="aisleCode">The aisle code to filter by.</param>
        /// <returns>A list of books in the specified aisle code.</returns>
        [HttpGet("aisle/{aisleCode}")] // GET: api/Locations/aisle/{aisleCode}
        [Authorize(Roles = "Librarian, Member")]
        public async Task<ActionResult<IEnumerable<Book>>> GetBooksByAisleCode(string aisleCode)
        {
            var result = await _locationService.GetBooksByAisleCodeAsync(aisleCode);

            if (!result.Success)
            {
                return NotFound(result.ErrorMessage);
            }

            return Ok(result.Data);
        }


        /// <summary>
        /// Retrieves books by shelf number.
        /// </summary>
        /// <param name="shelfNumber">The shelf number to filter by.</param>
        /// <returns>A list of books in the specified shelf number.</returns>
        [HttpGet("shelf/{shelfNumber}")] // GET: api/Locations/shelf/{shelfNumber}
        [Authorize(Roles = "Librarian, Member")]
        public async Task<ActionResult<IEnumerable<Book>>> GetBooksByShelfNumber(string shelfNumber)
        {
            var result = await _locationService.GetBooksByShelfNumberAsync(shelfNumber);

            if (!result.Success)
            {
                return NotFound(result.ErrorMessage);
            }

            return Ok(result.Data);
        }
    }
}
