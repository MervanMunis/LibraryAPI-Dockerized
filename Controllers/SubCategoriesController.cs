using Microsoft.AspNetCore.Mvc;
using LibraryAPI.Entities.Models;
using LibraryAPI.Entities.Enums;
using Microsoft.AspNetCore.Authorization;
using LibraryAPI.Services.Interfaces;
using LibraryAPI.DTOs.Response;
using LibraryAPI.DTOs.Request;

namespace LibraryAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SubCategoriesController : ControllerBase
    {
        private readonly ISubCategoryService _subCategoryService;

        public SubCategoriesController(ISubCategoryService subCategoryService)
        {
            _subCategoryService = subCategoryService;
        }

        /// <summary>
        /// Retrieves all subcategories.
        /// </summary>
        /// <returns>A list of all subcategories.</returns>    
        [HttpGet] // GET: api/SubCategories
        [Authorize(Roles = "Librarian, Member")]
        public async Task<ActionResult<IEnumerable<SubCategoryResponse>>> GetSubCategories()
        {
            var result = await _subCategoryService.GetAllSubCategoriesAsync();
            if (!result.Success)
            {
                return Problem(result.ErrorMessage);
            }
            return Ok(result.Data);
        }

        /// <summary>
        /// Retrieves a specific subcategory by its ID.
        /// </summary>
        /// <param name="id">The ID of the subcategory to retrieve.</param>
        /// <returns>The requested subcategory.</returns>
        [HttpGet("{id}")] // GET: api/SubCategories/5
        [Authorize(Roles = "Librarian, Member")]
        public async Task<ActionResult<SubCategoryResponse>> GetSubCategory(short id)
        {
            var result = await _subCategoryService.GetSubCategoryByIdAsync(id);
            if (!result.Success)
            {
                return NotFound(result.ErrorMessage);
            }
            return Ok(result.Data);
        }

        /// <summary>
        /// Retrieves all books associated with a specific subcategory.
        /// </summary>
        /// <param name="id">The ID of the subcategory whose books are to be retrieved.</param>
        /// <returns>A list of books associated with the subcategory.</returns>
        [HttpGet("{id}/books")] // GET: api/SubCategories/5/books
        [Authorize(Roles = "Librarian, Member")]
        public async Task<ActionResult<IEnumerable<BookResponse>>> GetBooksBySubCategory(short id)
        {
            var result = await _subCategoryService.GetBooksBySubCategoryIdAsync(id);
            if (!result.Success)
            {
                return NotFound(result.ErrorMessage);
            }
            return Ok(result.Data);
        }

        /// <summary>
        /// Adds a new subcategory.
        /// </summary>
        /// <param name="subCategoryRequest">The details of the subcategory to add.</param>
        /// <returns>A success message if the subcategory is added successfully.</returns>
        [HttpPost] // POST: api/SubCategories
        [Authorize(Roles = "Librarian")]
        public async Task<ActionResult<string>> PostSubCategory([FromBody] SubCategoryRequest subCategoryRequest)
        {
            var result = await _subCategoryService.AddSubCategoryAsync(subCategoryRequest);
            if (!result.Success)
            {
                return BadRequest(result.ErrorMessage);
            }
            return Ok("The subcategory is successfully created.");
        }

        /// <summary>
        /// Updates an existing subcategory.
        /// </summary>
        /// <param name="id">The ID of the subcategory to update.</param>
        /// <param name="subCategoryRequest">The new details of the subcategory.</param>
        /// <returns>A success message if the subcategory is updated successfully.</returns>
        [HttpPut("{id}")] // PUT: api/SubCategories/5
        [Authorize(Roles = "Librarian")]
        public async Task<ActionResult<string>> PutSubCategory(short id, [FromBody] SubCategoryRequest subCategoryRequest)
        {
            var result = await _subCategoryService.UpdateSubCategoryAsync(id, subCategoryRequest);
            if (!result.Success)
            {
                return BadRequest(result.ErrorMessage);
            }
            return Ok("The subcategory is updated successfully.");
        }

        /// <summary>
        /// Sets a subcategory to inactive status.
        /// </summary>
        /// <param name="id">The ID of the subcategory to set to inactive.</param>
        /// <returns>A success message if the subcategory is set to inactive successfully.</returns>
        [HttpPatch("{id}/status/inactive")] // PATCH: api/SubCategories/5 
        [Authorize(Roles = "Librarian")]
        public async Task<ActionResult<string>> InActiveSubCategory(short id)
        {
            var result = await _subCategoryService.SetSubCategoryStatusAsync(id, Status.InActive.ToString());
            if (!result.Success)
            {
                return NotFound(result.ErrorMessage);
            }
            return Ok("The subcategory is set to not active.");
        }

        /// <summary>
        /// Sets a subcategory to active status.
        /// </summary>
        /// <param name="id">The ID of the subcategory to set to active.</param>
        /// <returns>A success message if the subcategory is set to active successfully.</returns>
        [HttpPatch("{id}/status/active")] // PUT: api/SubCategories/5/status/active
        [Authorize(Roles = "Librarian")]
        public async Task<ActionResult<string>> ActiveSubCategory(short id)
        {
            var result = await _subCategoryService.SetSubCategoryStatusAsync(id, Status.Active.ToString());
            if (!result.Success)
            {
                return BadRequest(result.ErrorMessage);
            }
            return Ok("The subcategory is set to active.");
        }
    }
}

