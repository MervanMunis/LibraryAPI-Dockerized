using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LibraryAPI.Entities.Models;
using LibraryAPI.Entities.Enums;
using Microsoft.AspNetCore.Authorization;
using LibraryAPI.Services.Interfaces;
using LibraryAPI.DTOs.Request;
using LibraryAPI.DTOs.Response;

namespace LibraryAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        /// <summary>
        /// Retrieves all categories.
        /// </summary>
        /// <returns>A list of all categories.</returns>
        [HttpGet] // GET: api/Categories
        [Authorize(Roles = "Member, Librarian")]
        public async Task<ActionResult<IEnumerable<Category>>> GetCategories()
        {
            var result = await _categoryService.GetAllCategoriesAsync();
            if (!result.Success)
            {
                return Problem(result.ErrorMessage);
            }
            return Ok(result.Data);
        }

        /// <summary>
        /// Retrieves a specific category by its ID.
        /// </summary>
        /// <param name="id">The ID of the category to retrieve.</param>
        /// <returns>The details of the category.</returns>
        [HttpGet("{id}")] // GET: api/Categories/5
        [Authorize(Roles = "Member,Librarian")]
        public async Task<ActionResult<Category>> GetCategory(short id)
        {
            var result = await _categoryService.GetCategoryByIdAsync(id);
            if (!result.Success)
            {
                return NotFound(result.ErrorMessage);
            }
            return Ok(result.Data);
        }

        /// <summary>
        /// Retrieves all books associated with a specific category.
        /// </summary>
        /// <param name="id">The ID of the category whose books to retrieve.</param>
        /// <returns>A list of books in the specified category.</returns>
        [HttpGet("{id}/books")] // GET: api/Categories/5/books
        [Authorize(Roles = "Member, Librarian")]
        public async Task<ActionResult<IEnumerable<CategoryBookResponse>>> GetBooksByCategory(short id)
        {
            var result = await _categoryService.GetBooksByCategoryIdAsync(id);
            if (!result.Success)
            {
                return NotFound(result.ErrorMessage);
            }
            return Ok(result.Data);
        }

        /// <summary>
        /// Adds a new category.
        /// </summary>
        /// <param name="categoryRequest">The request body containing details of the category to add.</param>
        /// <returns>A message indicating the success or failure of the operation.</returns>
        [HttpPost]
        [Authorize(Roles = "Librarian")] // POST: api/Categories
        public async Task<ActionResult<string>> PostCategory([FromBody] CategoryRequest categoryRequest)
        {
            var result = await _categoryService.AddCategoryAsync(categoryRequest);
            if (!result.Success)
            {
                return BadRequest(result.ErrorMessage);
            }
            return Ok("The category is successfully created.");
        }

        /// <summary>
        /// Updates an existing category.
        /// </summary>
        /// <param name="id">The ID of the category to update.</param>
        /// <param name="categoryRequest">The request body containing updated details of the category.</param>
        /// <returns>A message indicating the success or failure of the operation.</returns>
        [HttpPut("{id}")] // PUT: api/Categories/5
        [Authorize(Roles = "Librarian")]
        public async Task<ActionResult<string>> PutCategory(short id, [FromBody] CategoryRequest categoryRequest)
        {
            var result = await _categoryService.UpdateCategoryAsync(id, categoryRequest);
            if (!result.Success)
            {
                return BadRequest(result.ErrorMessage);
            }
            return Ok("The category is updated successfully.");
        }

        /// <summary>
        /// Sets a category to inactive status.
        /// </summary>
        /// <param name="id">The ID of the category to update.</param>
        /// <returns>A message indicating the success or failure of the operation.</returns>
        [HttpPatch("{id}/status/inactive")] // PATCH: api/Categories/5
        [Authorize(Roles = "Librarian")]
        public async Task<ActionResult<string>> InActiveCategory(short id)
        {
            var result = await _categoryService.SetCategoryStatusAsync(id, Status.InActive);
            if (!result.Success)
            {
                return NotFound(result.ErrorMessage);
            }
            return Ok("The category is set to inactive.");
        }

        /// <summary>
        /// Sets a category to active status.
        /// </summary>
        /// <param name="id">The ID of the category to update.</param>
        /// <returns>A message indicating the success or failure of the operation.</returns>
        [HttpPatch("{id}/status/active")] // PATCH: api/Categories/5/status/active
        [Authorize(Roles = "Librarian")]
        public async Task<ActionResult<string>> SetCategoryActiveStatus(short id)
        {
            var result = await _categoryService.SetCategoryStatusAsync(id, Status.Active);
            if (!result.Success)
            {
                return BadRequest(result.ErrorMessage);
            }
            return Ok("The category is set to active.");
        }
    }
}
