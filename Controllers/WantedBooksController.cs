using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using LibraryAPI.Services.Interfaces;
using LibraryAPI.Models.DTOs.Request;
using LibraryAPI.Models.DTOs.Response;

namespace LibraryAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WantedBooksController : ControllerBase
    {
        private readonly IWantedBookService _wantedBookService;

        public WantedBooksController(IWantedBookService wantedBookService)
        {
            _wantedBookService = wantedBookService;
        }

        /// <summary>
        /// Retrieves all wanted books.
        /// </summary>
        /// <returns>A list of all wanted books.</returns>
        [HttpGet] // GET: api/WantedBooks
        [Authorize(Roles = "Librarian, Member")]
        public async Task<ActionResult<IEnumerable<WantedBookResponse>>> GetWantedBooks()
        {
            var result = await _wantedBookService.GetAllWantedBooksAsync();

            if (!result.Success)
            {
                return Problem(result.ErrorMessage);
            }

            return Ok(result.Data);
        }

        /// <summary>
        /// Retrieves a specific wanted book by its ID.
        /// </summary>
        /// <param name="id">The ID of the wanted book to retrieve.</param>
        /// <returns>The requested wanted book.</returns>        
        [HttpGet("{id}")] // GET: api/WantedBooks/5
        [Authorize(Roles = "Librarian, Member")]
        public async Task<ActionResult<WantedBookResponse>> GetWantedBook(int id)
        {
            var result = await _wantedBookService.GetWantedBookByIdAsync(id);

            if (!result.Success)
            {
                return NotFound(result.ErrorMessage);
            }

            return Ok(result.Data);
        }

        /// <summary>
        /// Adds a new wanted book.
        /// </summary>
        /// <param name="wantedBookRequest">The details of the wanted book to add.</param>
        /// <returns>A success message if the wanted book is added successfully.</returns>
        [HttpPost] // POST: api/WantedBooks
        [Authorize(Roles = "Member")]
        public async Task<ActionResult<string>> PostWantedBook([FromBody] WantedBookRequest wantedBookRequest)
        {
            var result = await _wantedBookService.AddWantedBookAsync(wantedBookRequest);

            if (!result.Success)
            {
                return BadRequest(result.ErrorMessage);
            }

            return Ok("The wanted book is successfully created.");
        }

        /// <summary>
        /// Deletes a specific wanted book by its ID.
        /// </summary>
        /// <param name="id">The ID of the wanted book to delete.</param>
        /// <returns>A success message if the wanted book is deleted successfully.</returns>
        [HttpDelete("{id}")] // DELETE: api/WantedBooks/5
        [Authorize(Roles = "Librarian")]
        public async Task<ActionResult<string>> DeleteWantedBook(int id)
        {
            var result = await _wantedBookService.DeleteWantedBookAsync(id);

            if (!result.Success)
            {
                return NotFound(result.ErrorMessage);
            }

            return Ok("The wanted book is deleted.");
        }
    }
}
