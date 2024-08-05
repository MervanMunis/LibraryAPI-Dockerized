using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using LibraryAPI.Services.Interfaces;
using LibraryAPI.Models.Enums;
using LibraryAPI.Models.DTOs.Request;
using LibraryAPI.Models.DTOs.Response;

namespace LibraryAPI.Controllers
{
    /// <summary>
    /// Controller for managing authors.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AuthorsController : ControllerBase
    {
        private readonly IAuthorService _authorService;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthorsController"/> class.
        /// </summary>
        public AuthorsController(IAuthorService authorService)
        {
            _authorService = authorService;
        }

        /// <summary>
        /// Retrieves all authors.
        /// </summary>
        /// <returns>A list of all authors.</returns>
        [HttpGet] // GET: api/Authors
        [Authorize(Roles = "Member, Librarian")]
        public async Task<ActionResult<IEnumerable<AuthorResoponse>>> GetAuthors()
        {
            var result = await _authorService.GetAllAuthorsAsync();

            if (!result.Success)
            {
                return Problem(result.ErrorMessage);
            }

            return Ok(result.Data);
        }

        /// <summary>
        /// Retrieves all active authors.
        /// </summary>
        /// <returns>A list of active authors.</returns>
        [HttpGet("active")] // GET: api/Authors/active
        [Authorize(Roles = "Member, Librarian")]
        public async Task<IActionResult> GetAllActiveAuthors()
        {
            var result = await _authorService.GetAllActiveAuthorsAsync();
            if (result.Success)
            {
                return Ok(result.Data);
            }
            return BadRequest(result.ErrorMessage);
        }


        /// <summary>
        /// Retrieves all inactive authors.
        /// </summary>
        /// <returns>A list of inactive authors.</returns>
        [HttpGet("inactive")] // GET: api/Authors/inactive
        [Authorize(Roles = "Member, Librarian")]
        public async Task<IActionResult> GetAllInactiveAuthors()
        {
            var result = await _authorService.GetAllInActiveAuthorsAsync();
            if (result.Success)
            {
                return Ok(result.Data);
            }
            return BadRequest(result.ErrorMessage);
        }

        /// <summary>
        /// Retrieves all inactive authors.
        /// </summary>
        /// <returns>A list of inactive authors.</returns>
        [HttpGet("banned")] // GET: api/Authors/inactive
        [Authorize(Roles = "Member, Librarian")]
        public async Task<IActionResult> GetAllBannedAuthors()
        {
            var result = await _authorService.GetAllBannedAuthorsAsync();
            if (result.Success)
            {
                return Ok(result.Data);
            }
            return BadRequest(result.ErrorMessage);
        }

        /// <summary>
        /// Retrieves an author by their ID.
        /// </summary>
        /// <param name="id">The ID of the author to retrieve.</param>
        /// <returns>The author details if found.</returns>
        [HttpGet("{id}")] // GET: api/Authors/5
        [Authorize(Roles = "Member, Librarian")]
        public async Task<ActionResult<AuthorResoponse>> GetAuthor(long id)
        {
            var result = await _authorService.GetAuthorByIdAsync(id);

            if (!result.Success)
            {
                return NotFound(result.ErrorMessage);
            }

            return Ok(result.Data);
        }

        /// <summary>
        /// Adds a new author.
        /// </summary>
        /// <param name="authorRequest">The author data to add.</param>
        /// <returns>A success message if the author is added successfully.</returns>
        [HttpPost] // POST: api/Authors
        [Authorize(Roles = "Librarian")] 
        public async Task<ActionResult<string>> PostAuthor([FromBody] AuthorRequest authorRequest)
        {
            var result = await _authorService.AddAuthorAsync(authorRequest);

            if (!result.Success)
            {
                return BadRequest(result.ErrorMessage);
            }

            return Ok("The author is successfully created.");
        }

        /// <summary>
        /// Updates an existing author's details.
        /// </summary>
        /// <param name="id">The ID of the author to update.</param>
        /// <param name="authorRequest">The new author data.</param>
        /// <returns>A success message if the author is updated successfully.</returns>
        [HttpPut("{id}")] // PUT: api/Authors/5
        [Authorize(Roles = "Librarian")]
        public async Task<ActionResult<string>> PutAuthor(long id, [FromBody] AuthorRequest authorRequest)
        {
            var result = await _authorService.UpdateAuthorAsync(id, authorRequest);

            if (!result.Success)
            {
                return BadRequest(result.ErrorMessage);
            }

            return Ok("The author is updated successfully.");
        }

        /// <summary>
        /// Updates the status of an author.
        /// </summary>
        /// <param name="id">The ID of the author whose status is to be updated.</param>
        /// <param name="status">The new status of the author.</param>
        /// <returns>A success message if the author's status is updated successfully.</returns>
        [HttpPatch("{id}/status/inactive")] // PATCH: api/Authors/5
        [Authorize(Roles = "Librarian")]
        public async Task<ActionResult<string>> InActiveAuthor(long id)
        {
            var result = await _authorService.SetAuthorStatusAsync(id, Status.InActive.ToString());

            if (!result.Success)
            {
                return NotFound(result.ErrorMessage);
            }

            return Ok("The author is set to inactive.");
        }

        /// <summary>
        /// Updates the status of an author.
        /// </summary>
        /// <param name="id">The ID of the author whose status is to be updated.</param>
        /// <param name="status">The new status of the author.</param>
        /// <returns>A success message if the author's status is updated successfully.</returns>
        [HttpPatch("{id}/status/banned")] // PATCH: api/Authors/5/banned
        [Authorize(Roles = "Librarian")]
        public async Task<IActionResult> BannedAuthor(long id)
        {
            var result = await _authorService.SetAuthorStatusAsync(id, Status.Banned.ToString());

            if (!result.Success)
            {
                return NotFound(result.ErrorMessage);
            }

            return Ok("The author is banned.");
        }

        /// <summary>
        /// Updates the status of an author.
        /// </summary>
        /// <param name="id">The ID of the author whose status is to be updated.</param>
        /// <param name="status">The new status of the author.</param>
        /// <returns>A success message if the author's status is updated successfully.</returns>
        [HttpPatch("{id}/status/active")] // PATCH: api/Authors/5/status/active
        [Authorize(Roles = "Librarian")]
        public async Task<ActionResult<string>> SetAuthorActiveStatus(long id)
        {
            var result = await _authorService.SetAuthorStatusAsync(id, Status.Active.ToString());
            if (!result.Success)
            {
                return BadRequest(result.ErrorMessage);
            }
            return Ok("The author and their books are set to active status.");
        }

        /// <summary>
        /// Updates the image of an author.
        /// </summary>
        /// <param name="id">The ID of the author whose image is to be updated.</param>
        /// <param name="image">The new image file.</param>
        /// <returns>A success message if the author's image is updated successfully.</returns>
        [HttpPatch("{id}/image")] // PATCH: api/Authors/5/image
        [Authorize(Roles = "Librarian")]
        public async Task<ActionResult<string>> UpdateAuthorImage(long id, IFormFile image)
        {
            if (image == null)
            {
                return BadRequest("Image file is required");
            }

            var result = await _authorService.UpdateAuthorImageAsync(id, image);

            if (!result.Success)
            {
                return BadRequest(result.ErrorMessage);
            }

            return Ok("The author's image updated successfully.");
        }

        /// <summary>
        /// Retrieves the image of an author by author ID.
        /// </summary>
        /// <param name="authorId">The ID of the author.</param>
        /// <returns>The image as a byte array.</returns>
        [HttpGet("{authorId}/image")]
        [Authorize(Roles = "Member, Librarian")]
        public async Task<IActionResult> GetAuthorImage(long authorId)
        {
            var result = await _authorService.GetAuthorImageAsync(authorId);

            if (!result.Success)
            {
                return NotFound(result.ErrorMessage);
            }

            return File(result.Data!, "image/jpeg"); // Assuming the images are JPEG. Adjust as necessary.
        }
    }
}
