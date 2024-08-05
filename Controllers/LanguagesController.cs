using LibraryAPI.Models.DTOs.Request;
using LibraryAPI.Models.DTOs.Response;
using LibraryAPI.Models.Enums;
using LibraryAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LanguagesController : ControllerBase
    {
        private readonly ILanguageService _languageService;

        public LanguagesController(ILanguageService languageService)
        {
            _languageService = languageService;
        }

        /// <summary>
        /// Retrieves all active languages.
        /// </summary>
        /// <returns>A list of active languages.</returns>
        [HttpGet] // GET: api/Languages
        [Authorize(Roles = "Member,Librarian")]
        public async Task<ActionResult<IEnumerable<LanguageResponse>>> GetLanguages()
        {
            var result = await _languageService.GetAllLanguagesAsync();

            if (!result.Success)
            {
                return Problem(result.ErrorMessage);
            }

            return Ok(result.Data);
        }

        /// <summary>
        /// Retrieves a specific language by its ID.
        /// </summary>
        /// <param name="id">The ID of the language.</param>
        /// <returns>The language details.</returns>
        [HttpGet("{id}")] // GET: api/Languages/5
        [Authorize(Roles = "Member,Librarian")]
        public async Task<ActionResult<LanguageResponse>> GetLanguage(short id)
        {
            var result = await _languageService.GetLanguageByIdAsync(id);

            if (!result.Success)
            {
                return NotFound(result.ErrorMessage);
            }

            return Ok(result.Data);
        }

        /// <summary>
        /// Retrieves books associated with a specific language.
        /// </summary>
        /// <param name="id">The ID of the language.</param>
        /// <returns>A list of books associated with the language.</returns>
        [HttpGet("{id}/books")] // GET: api/Languages/5/books
        [Authorize(Roles = "Member,Librarian")]
        public async Task<ActionResult<IEnumerable<BookResponse>>> GetBooksByLanguage(short id)
        {
            var result = await _languageService.GetBooksByLanguageIdAsync(id);

            if (!result.Success)
            {
                return NotFound(result.ErrorMessage);
            }

            return Ok(result.Data);
        }

        /// <summary>
        /// Adds a new language to the database.
        /// </summary>
        /// <param name="languageRequest">The language details.</param>
        /// <returns>A success message if the language is created successfully.</returns>
        [HttpPost] // POST: api/Languages
        [Authorize(Roles = "Librarian")]
        public async Task<ActionResult<string>> PostLanguage([FromBody] LanguageRequest languageRequest)
        {
            var result = await _languageService.AddLanguageAsync(languageRequest);

            if (!result.Success)
            {
                return BadRequest(result.ErrorMessage);
            }

            return Ok("Language successfully created.");
        }

        /// <summary>
        /// Updates an existing language's details.
        /// </summary>
        /// <param name="id">The ID of the language to update.</param>
        /// <param name="languageRequest">The updated language details.</param>
        /// <returns>A success message if the language is updated successfully.</returns>
        [HttpPut("{id}")] // PUT: api/Languages/5
        [Authorize(Roles = "Librarian")]
        public async Task<ActionResult<string>> PutLanguage(short id, [FromBody] LanguageRequest languageRequest)
        {
            var result = await _languageService.UpdateLanguageAsync(id, languageRequest);

            if (!result.Success)
            {
                return BadRequest(result.ErrorMessage);
            }

            return Ok("Language successfully updated.");
        }

        /// <summary>
        /// Sets the language and its associated books to active status.
        /// </summary>
        /// <param name="id">The ID of the language.</param>
        /// <returns>A success message if the language is set to active status.</returns>
        [HttpPatch("{id}/status/active")] // PATCH: api/Languages/5/status/active
        [Authorize(Roles = "Librarian")]
        public async Task<ActionResult<string>> SetLanguageActiveStatus(short id)
        {
            var result = await _languageService.SetLanguageStatusAsync(id, Status.Active.ToString());
            if (!result.Success)
            {
                return BadRequest(result.ErrorMessage);
            }
            return Ok("The language and its books are set to active status.");
        }

        /// <summary>
        /// Sets the language and its associated books to inactive status.
        /// </summary>
        /// <param name="id">The ID of the language.</param>
        /// <returns>A success message if the language is set to inactive status.</returns>
        [HttpPatch("{id}/status/InActive")] // PATCH: api/Languages/5/status/InActive
        [Authorize(Roles = "Librarian")]
        public async Task<ActionResult<string>> InActiveLanguage(short id)
        {
            var result = await _languageService.SetLanguageStatusAsync(id, Status.InActive.ToString());
            if (!result.Success)
            {
                return BadRequest(result.ErrorMessage);
            }
            return Ok("The language and its books are set to inactive status.");
        }
    }
}
