using LibraryAPI.Models.DTOs.Request;
using LibraryAPI.Models.DTOs.Response;
using LibraryAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NationalitiesController : ControllerBase
    {
        private readonly INationalityService _nationalityService;

        public NationalitiesController(INationalityService nationalityService)
        {
            _nationalityService = nationalityService;
        }


        /// <summary>
        /// Retrieves all nationalities.
        /// </summary>
        /// <returns>A list of nationality responses.</returns>
        [HttpGet] // GET: api/Nationalities
        [Authorize(Roles = "Librarian, Member")]
        public async Task<ActionResult<IEnumerable<NationalityResponse>>> GetNationalities()
        {
            var result = await _nationalityService.GetAllNationalitiesAsync();

            if (!result.Success)
            {
                return Problem(result.ErrorMessage);
            }

            return Ok(result.Data);
        }

        /// <summary>
        /// Retrieves a specific nationality by its ID.
        /// </summary>
        /// <param name="id">The ID of the nationality.</param>
        /// <returns>The nationality details.</returns>
        [HttpGet("{id}")] // GET: api/Nationalities/5
        [Authorize(Roles = "Librarian, Member")]
        public async Task<ActionResult<NationalityResponse>> GetNationality(short id)
        {
            var result = await _nationalityService.GetNationalityByIdAsync(id);

            if (!result.Success)
            {
                return NotFound(result.ErrorMessage);
            }

            return Ok(result.Data);
        }

        /// <summary>
        /// Retrieves authors by their nationality ID.
        /// </summary>
        /// <param name="id">The ID of the nationality.</param>
        /// <returns>A list of authors associated with the specified nationality.</returns>
        [HttpGet("{id}/authors")] // GET: api/Nationalities/5/authors
        [Authorize(Roles = "Librarian, Member")]
        public async Task<ActionResult<IEnumerable<AuthorResoponse>>> GetAuthorsByNationalityId(short id)
        {
            var result = await _nationalityService.GetAuthorsByNationalityIdAsync(id);

            if (!result.Success)
            {
                return NotFound(result.ErrorMessage);
            }

            return Ok(result.Data);
        }

        /// <summary>
        /// Adds a new nationality.
        /// </summary>
        /// <param name="nationalityRequest">The nationality details.</param>
        /// <returns>A success message if the nationality is created successfully.</returns>
        [HttpPost] // POST: api/Nationalities
        [Authorize(Roles = "Librarian")]
        public async Task<ActionResult<string>> PostNationality([FromBody] NationalityRequest nationalityRequest)
        {
            var result = await _nationalityService.AddNationalityAsync(nationalityRequest);

            if (!result.Success)
            {
                return BadRequest(result.ErrorMessage);
            }

            return Ok("Nationality successfully created.");
        }

        /// <summary>
        /// Updates an existing nationality's details.
        /// </summary>
        /// <param name="id">The ID of the nationality to update.</param>
        /// <param name="nationalityRequest">The updated nationality details.</param>
        /// <returns>A success message if the nationality is updated successfully.</returns>
        [HttpPut("{id}")] // PUT: api/Nationalities/5
        [Authorize(Roles = "Librarian")]
        public async Task<ActionResult<string>> PutNationality(short id, [FromBody] NationalityRequest nationalityRequest)
        {
            var result = await _nationalityService.UpdateNationalityAsync(id, nationalityRequest);

            if (!result.Success)
            {
                return BadRequest(result.ErrorMessage);
            }

            return Ok("Nationality successfully updated.");
        }

        /// <summary>
        /// Deletes a nationality by its ID.
        /// </summary>
        /// <param name="id">The ID of the nationality to delete.</param>
        /// <returns>A success message if the nationality is deleted successfully.</returns>
        [HttpDelete("{id}")] // DELETE: api/Nationalities/5
        [Authorize(Roles = "Librarian")]
        public async Task<ActionResult<string>> DeleteNationality(short id)
        {
            var result = await _nationalityService.DeleteNationalityAsync(id);

            if (!result.Success)
            {
                return BadRequest(result.ErrorMessage);
            }

            return Ok("Nationality successfully deleted.");
        }
    }
}
