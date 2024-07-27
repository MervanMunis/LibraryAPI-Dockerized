using System.Security.Claims;
using LibraryAPI.DTOs;
using LibraryAPI.DTOs.Request;
using LibraryAPI.DTOs.Response;
using LibraryAPI.Entities.Enums;
using LibraryAPI.Entities.Models;
using LibraryAPI.Exceptions;
using LibraryAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class BooksController : ControllerBase
{
    private readonly IBookService _bookService;

    public BooksController(IBookService bookService)
    {
        _bookService = bookService;
    }

    /// <summary>
    /// Retrieves all books.
    /// </summary>
    [HttpGet] // GET: api/Books
    [Authorize(Roles = "Member, Librarian")]
    public async Task<ActionResult<IEnumerable<BookResponse>>> GetBooks()
    {
        var result = await _bookService.GetAllBooksAsync();

        if (!result.Success)
        {
            return Problem(result.ErrorMessage);
        }

        return Ok(result.Data);
    }

    /// <summary>
    /// Retrieves all active books.
    /// </summary>
    /// <returns>A service result containing a list of active books.</returns>
    [HttpGet("active")] // GET: api/Books/active
    [Authorize(Roles = "Member, Librarian")]
    public async Task<ActionResult<ServiceResult<IEnumerable<BookResponse>>>> GetAllActiveBooks()
    {
        var result = await _bookService.GetAllActiveBooksAsync();

        if (!result.Success)
        {
            return Problem(result.ErrorMessage);
        }

        return Ok(result);
    }

    /// <summary>
    /// Retrieves all inactive books.
    /// </summary>
    /// <returns>A service result containing a list of inactive books.</returns>
    [HttpGet("inactive")] // GET: api/Books/inactive
    [Authorize(Roles = "Member, Librarian")]
    public async Task<ActionResult<ServiceResult<IEnumerable<BookResponse>>>> GetAllInActiveBooks()
    {
        var result = await _bookService.GetAllInActiveBooksAsync();

        if (!result.Success)
        {
            return Problem(result.ErrorMessage);
        }

        return Ok(result);
    }

    /// <summary>
    /// Retrieves all banned books.
    /// </summary>
    /// <returns>A service result containing a list of banned books.</returns>
    [HttpGet("banned")] // GET: api/Books/banned
    [Authorize(Roles = "Member, Librarian")]
    public async Task<ActionResult<ServiceResult<IEnumerable<BookResponse>>>> GetAllBannedBooks()
    {
        var result = await _bookService.GetAllBannedBooksAsync();

        if (!result.Success)
        {
            return Problem(result.ErrorMessage);
        }

        return Ok(result);
    }

    /// <summary>
    /// Retrieves all borrowed books.
    /// </summary>
    /// <returns>A service result containing a list of borrowed books.</returns>
    [HttpGet("bookCopy/active")] // GET: api/Books/bookCopy/active
    [Authorize(Roles = "Member, Librarian")]
    public async Task<ActionResult<ServiceResult<IEnumerable<BookResponse>>>> GetAllActiveBookCopies()
    {
        var result = await _bookService.GetAllActiveBookCopiesAsync();

        if (!result.Success)
        {
            return Problem(result.ErrorMessage);
        }

        return Ok(result);
    }

    /// <summary>
    /// Retrieves all borrowed books.
    /// </summary>
    /// <returns>A service result containing a list of borrowed books.</returns>
    [HttpGet("bookCopy/inactive")] // GET: api/Books/bookCopy/inactive
    [Authorize(Roles = "Member, Librarian")]
    public async Task<ActionResult<ServiceResult<IEnumerable<BookResponse>>>> GetAllInActiveBookCopies()
    {
        var result = await _bookService.GetAllInActiveBookCopiesAsync();

        if (!result.Success)
        {
            return Problem(result.ErrorMessage);
        }

        return Ok(result);
    }

    /// <summary>
    /// Retrieves all borrowed books.
    /// </summary>
    /// <returns>A service result containing a list of borrowed books.</returns>
    [HttpGet("bookCopy/borrowed")] // GET: api/Books/bookCopy/borrowed
    [Authorize(Roles = "Member, Librarian")]
    public async Task<ActionResult<ServiceResult<IEnumerable<BookResponse>>>> GetAllBorrowedBookCopies()
    {
        var result = await _bookService.GetAllBorrowedBookCopiesAsync();

        if (!result.Success)
        {
            return Problem(result.ErrorMessage);
        }

        return Ok(result);
    }

    /// <summary>
    /// Retrieves a specific book by its ID.
    /// </summary>
    /// <param name="id">The ID of the book to retrieve.</param>
    /// <returns>The details of the book.</returns>
    [HttpGet("{id}")] // GET: api/Books/5
    [Authorize(Roles = "Member, Librarian")]
    public async Task<ActionResult<BookResponse>> GetBook(long id)
    {
        var result = await _bookService.GetBookByIdAsync(id);

        if (!result.Success)
        {
            return NotFound(result.ErrorMessage);
        }

        return Ok(result.Data);
    }


    /// <summary>
    /// Adds a new book.
    /// </summary>
    /// <param name="bookRequest">The request body containing details of the book to add.</param>
    /// <returns>Confirmation message upon successful addition.</returns>
    [HttpPost] // POST: api/Books
    [Authorize(Roles = "Librarian")]
    public async Task<ActionResult<string>> PostBook([FromBody] BookRequest bookRequest)
    {
        var result = await _bookService.AddBookAsync(bookRequest);

        if (!result.Success)
        {
            return BadRequest(result.ErrorMessage);
        }

        return Ok("The book is added successfully.");
    }

    /// <summary>
    /// Updates an existing book.
    /// </summary>
    /// <param name="id">The ID of the book to update.</param>
    /// <param name="bookRequest">The request body containing updated details of the book.</param>
    /// <returns>Confirmation message upon successful update.</returns>
    [HttpPut("{id}")] // PUT: api/Books/5
    [Authorize(Roles = "Librarian")]
    public async Task<ActionResult<string>> PutBook(long id, [FromBody] BookRequest bookRequest)
    {
        var result = await _bookService.UpdateBookAsync(id, bookRequest);

        if (!result.Success)
        {
            return BadRequest(result.ErrorMessage);
        }

        return Ok("The book is updated successfully.");
    }

    /// <summary>
    /// Sets the status of a book to inactive.
    /// </summary>
    /// <param name="id">The ID of the book to update.</param>
    /// <returns>Confirmation message upon successful status change.</returns>
    [HttpPatch("{id}/InActive")] // PATCH: api/Books/5/InActive
    [Authorize(Roles = "Librarian")]
    public async Task<ActionResult<string>> InActiveBook(long id)
    {
        var result = await _bookService.SetBookStatusAsync(id, Status.InActive.ToString());

        if (!result.Success)
        {
            return NotFound(result.ErrorMessage);
        }

        return Ok("The status of the book is now inactive.");
    }

    /// <summary>
    /// Sets the status of a book to active.
    /// </summary>
    /// <param name="id">The ID of the book to update.</param>
    /// <returns>Confirmation message upon successful status change.</returns>
    [HttpPatch("{id}/Active")] // PATCH: api/Books/5/Active
    [Authorize(Roles = "Librarian")]
    public async Task<ActionResult<string>> ActiveBook(long id)
    {
        var result = await _bookService.SetBookStatusAsync(id, Status.Active.ToString());

        if (!result.Success)
        {
            return NotFound(result.ErrorMessage);
        }

        return Ok("The status of the book is now active.");
    }

    /// <summary>
    /// Sets the status of a book to banned.
    /// </summary>
    /// <param name="id">The ID of the book to update.</param>
    /// <returns>Confirmation message upon successful status change.</returns>
    [HttpPatch("{id}/Banned")] // PATCH: api/Books/5/Banned
    [Authorize(Roles = "Librarian")]
    public async Task<IActionResult> BannedBook(long id)
    {
        var result = await _bookService.SetBookStatusAsync(id, Status.Banned.ToString());

        if (!result.Success)
        {
            return NotFound(result.ErrorMessage);
        }

        return Ok("The status of the book is now banned.");
    }

    /// <summary>
    /// Updates the cover image of a book.
    /// </summary>
    /// <param name="id">The ID of the book to update.</param>
    /// <param name="coverImage">The new cover image file.</param>
    /// <returns>Confirmation message upon successful update.</returns>
    [HttpPatch("{id}/image")] // PATCH: api/Books/5/image
    [Authorize(Roles = "Librarian")]
    public async Task<ActionResult<string>> UpdateBookImage(long id, IFormFile coverImage)
    {
        if (coverImage == null)
        {
            return BadRequest("Image file is required");
        }

        var result = await _bookService.UpdateBookImageAsync(id, coverImage);

        if (!result.Success)
        {
            return BadRequest(result.ErrorMessage);
        }

        return Ok("The book's image has been updated successfully.");
    }

    /// <summary>
    /// Retrieves the image of a book by book ID.
    /// </summary>
    /// <param name="bookId">The ID of the book.</param>
    /// <returns>The image as a byte array.</returns>
    [HttpGet("{bookId}/image")]
    [Authorize(Roles = "Member, Librarian")]
    public async Task<IActionResult> GetBookImage(long bookId)
    {
        var result = await _bookService.GetBookImageAsync(bookId);

        if (!result.Success)
        {
            return NotFound(result.ErrorMessage);
        }

        return File(result.Data!, "image/jpeg"); // Assuming the images are JPEG. Adjust as necessary.
    }

    /// <summary>
    /// Updates the rating of a book.
    /// </summary>
    /// <param name="id">The ID of the book to rate.</param>
    /// <param name="ratingRequest">The request body containing the new rating and member ID.</param>
    /// <returns>Confirmation message upon successful update.</returns>
    [HttpPut("{id}/rating")]// PATCH: api/Books/5/rating
    [Authorize(Roles = "Member")]
    public async Task<ActionResult<string>> UpdateBookRating(long id, [FromBody] BookRatingRequest ratingRequest)
    {
        ratingRequest.MemberId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var result = await _bookService.UpdateBookRatingAsync(id, ratingRequest.GivenRating, ratingRequest.MemberId);

        if (!result.Success)
        {
            return BadRequest(result.ErrorMessage);
        }

        return Ok(result.SuccessMessage);
    }

    /// <summary>
    /// Updates the number of copies of a book.
    /// </summary>
    /// <param name="id">The ID of the book to update.</param>
    /// <param name="change">The number of copies to add or remove.</param>
    /// <returns>Confirmation message upon successful update.</returns>
    [HttpPut("{id}/copies")] // PUT: api/Books/5/copies
    [Authorize(Roles = "Librarian")]
    public async Task<ActionResult<string>> UpdateBookCopies(long id, [FromBody] short change)
    {
        var result = await _bookService.UpdateBookCopiesAsync(id, change);

        if (!result.Success)
        {
            return BadRequest(result.ErrorMessage);
        }

        return Ok("The book copies have been updated successfully.");
    }
}
