using System.Linq.Expressions;
using LibraryAPI.Data;
using LibraryAPI.Exceptions;
using LibraryAPI.Models.DTOs.Request;
using LibraryAPI.Models.DTOs.Response;
using LibraryAPI.Models.Entities;
using LibraryAPI.Models.Enums;
using LibraryAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LibraryAPI.Services.impl
{
    /// <summary>
    /// Service class for managing books.
    /// Implements methods to interact with book-related data.
    /// </summary>
    public class BookService : IBookService
    {
        private readonly LibraryAPIContext _context;
        private readonly IFileService _fileService;

        /// <summary>
        /// Initializes a new instance of the <see cref="BookService"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="fileService">The file service for handling file operations.</param>
        public BookService(LibraryAPIContext context, IFileService fileService)
        {
            _context = context;
            _fileService = fileService;
        }

        /// <summary>
        /// Retrieves all books.
        /// </summary>
        /// <returns>A service result containing a list of all books.</returns>
        public async Task<ServiceResult<IEnumerable<BookResponse>>> GetAllBooksAsync()
        {
            var books = await GetBooksAsync(_ => true);
            return ServiceResult<IEnumerable<BookResponse>>.SuccessResult(books);
        }

        /// <summary>
        /// Retrieves all active books.
        /// </summary>
        /// <returns>A service result containing a list of active books.</returns>
        public async Task<ServiceResult<IEnumerable<BookResponse>>> GetAllActiveBooksAsync()
        {
            var books = await GetBooksAsync(book => book.BookStatus == Status.Active.ToString());
            return ServiceResult<IEnumerable<BookResponse>>.SuccessResult(books);
        }

        /// <summary>
        /// Retrieves all inactive books.
        /// </summary>
        /// <returns>A service result containing a list of inactive books.</returns>
        public async Task<ServiceResult<IEnumerable<BookResponse>>> GetAllInActiveBooksAsync()
        {
            var books = await GetBooksAsync(book => book.BookStatus == Status.InActive.ToString());
            return ServiceResult<IEnumerable<BookResponse>>.SuccessResult(books);
        }

        /// <summary>
        /// Retrieves all banned books.
        /// </summary>
        /// <returns>A service result containing a list of banned books.</returns>
        public async Task<ServiceResult<IEnumerable<BookResponse>>> GetAllBannedBooksAsync()
        {
            var books = await GetBooksAsync(book => book.BookStatus == Status.Banned.ToString());
            return ServiceResult<IEnumerable<BookResponse>>.SuccessResult(books);
        }

        /// <summary>
        /// Retrieves all active book copies.
        /// </summary>
        /// <returns>A service result containing a list of borrowed book copies.</returns>
        public async Task<ServiceResult<IEnumerable<BookCopyResponse>>> GetAllActiveBookCopiesAsync()
        {
            var bookCopies = await GetBookCopiesAsync(bookCopy => bookCopy.BookCopyStatus == Status.Active.ToString());
            return ServiceResult<IEnumerable<BookCopyResponse>>.SuccessResult(bookCopies);
        }

        /// <summary>
        /// Retrieves all inactive book copies.
        /// </summary>
        /// <returns>A service result containing a list of borrowed book copies.</returns>
        public async Task<ServiceResult<IEnumerable<BookCopyResponse>>> GetAllInActiveBookCopiesAsync()
        {
            var bookCopies = await GetBookCopiesAsync(bookCopy => bookCopy.BookCopyStatus == Status.InActive.ToString());
            return ServiceResult<IEnumerable<BookCopyResponse>>.SuccessResult(bookCopies);
        }


        /// <summary>
         /// Retrieves all borrowed book copies.
         /// </summary>
         /// <returns>A service result containing a list of borrowed book copies.</returns>
        public async Task<ServiceResult<IEnumerable<BookCopyResponse>>> GetAllBorrowedBookCopiesAsync()
        {
            var bookCopies = await GetBookCopiesAsync(bookCopy => bookCopy.BookCopyStatus == Status.Borrowed.ToString());
            return ServiceResult<IEnumerable<BookCopyResponse>>.SuccessResult(bookCopies);
        }

        /// <summary>
        /// Retrieves a specific book by its ID.
        /// </summary>
        /// <param name="id">The ID of the book to retrieve.</param>
        /// <returns>A service result containing the book details.</returns>
        public async Task<ServiceResult<BookResponse>> GetBookByIdAsync(long id)
        {
            var book = await _context.Books!
                .Where(book => book.BookStatus == Status.Active.ToString())
                .Include(b => b.Publisher)
                .Include(b => b.Location)
                .Include(b => b.BookSubCategories)!
                    .ThenInclude(bsc => bsc.SubCategory)
                .Include(b => b.AuthorBooks)!
                    .ThenInclude(ab => ab.Author)
                .Include(b => b.BookLanguages)!
                    .ThenInclude(bl => bl.Language)
                .Include(b => b.BookCopies)
                .Select(b => new BookResponse
                {
                    BookId = b.BookId,
                    ISBN = b.ISBN,
                    Title = b.Title,
                    PageCount = b.PageCount,
                    PublishingYear = b.PublishingYear,
                    Description = b.Description,
                    PrintCount = b.PrintCount,
                    CopyCount = (short)b.BookCopies!.Where(bc => bc.BookCopyStatus == Status.Active.ToString()).ToList().Count,
                    BookStatus = b.BookStatus,
                    PublisherName = b.Publisher!.Name,
                    SubCategoryNames = b.BookSubCategories!.Select(sb => sb.SubCategory!.Name).ToList(),
                    CategoryName = b.BookSubCategories!.Select(sb => sb.SubCategory!.Category!.Name).First(),
                    LanguageNames = b.BookLanguages!.Select(bl => bl.Language!.Name).ToList(),
                    AuthorNames = b.AuthorBooks!.Select(ab => ab.Author!.FullName).ToList(),
                })
                .FirstOrDefaultAsync(b => b.BookId == id);

            if (book == null || book.BookStatus != Status.Active.ToString())
            {
                return ServiceResult<BookResponse>.FailureResult("Book not found");
            }

            return ServiceResult<BookResponse>.SuccessResult(book);
        }

        /// <summary>
        /// Adds a new book to the database.
        /// </summary>
        /// <param name="bookRequest">The book request containing book details.</param>
        /// <returns>A service result indicating success or failure.</returns>
        public async Task<ServiceResult<string>> AddBookAsync(BookRequest bookRequest)
        {
            if (await _context.Books!.AnyAsync(book => book.ISBN == bookRequest.ISBN))
            {
                return ServiceResult<string>.FailureResult("The book with the specified ISBN is already in the database!");
            }

            // Check if the location already has 50 books
            if (bookRequest.LocationId.HasValue)
            {
                var booksInLocation = await _context.BookCopies!
                    .Where(bookCopy => bookCopy.Book!.LocationId == bookRequest.LocationId.Value && 
                                       bookCopy.BookCopyStatus == Status.Active.ToString())
                    .CountAsync();

                if (booksInLocation >= 50)
                {
                    return ServiceResult<string>.FailureResult("The shelf already has 50 books. No more books can be added to this shelf.");
                }
            }

            var newBook = new Book
            {
                ISBN = bookRequest.ISBN,
                Title = bookRequest.Title,
                PageCount = bookRequest.PageCount,
                PublishingYear = bookRequest.PublishingYear,
                Description = bookRequest.Description,
                PrintCount = bookRequest.PrintCount,
                BookStatus = Status.Active.ToString(),
                PublisherId = bookRequest.PublisherId,
                LocationId = bookRequest.LocationId
            };

            newBook = CrossTable(bookRequest, newBook);

            await _context.Books!.AddAsync(newBook);
            await _context.SaveChangesAsync();

            var bookCopies = Enumerable.Range(0, bookRequest.CopyCount).Select(_ => new BookCopy
            {
                BookId = newBook.BookId,
                BookCopyStatus = Status.Active.ToString()
            });

            await _context.BookCopies!.AddRangeAsync(bookCopies);

            await _context.SaveChangesAsync();

            return ServiceResult<string>.SuccessMessageResult("Book successfully created!");
        }

        /// <summary>
        /// Updates the details of an existing book.
        /// </summary>
        /// <param name="id">The ID of the book to update.</param>
        /// <param name="bookRequest">The book request containing updated book details.</param>
        /// <returns>A service result indicating success or failure.</returns>
        public async Task<ServiceResult<bool>> UpdateBookAsync(long id, BookRequest bookRequest)
        {
            var book = await _context.Books!.FindAsync(id);

            if (book == null)
            {
                return ServiceResult<bool>.FailureResult("Book not found");
            }

            book.ISBN = bookRequest.ISBN;
            book.Title = bookRequest.Title;
            book.PageCount = bookRequest.PageCount;
            book.PublishingYear = bookRequest.PublishingYear;
            book.Description = bookRequest.Description;
            book.PrintCount = bookRequest.PrintCount;
            book.PublisherId = bookRequest.PublisherId;
            book.LocationId = bookRequest.LocationId;

            _context.Update(book).State = EntityState.Modified;

            var existingAuthorBooks = _context.AuthorBook!.Where(authorBook => authorBook.BooksId == id);
            _context.AuthorBook!.RemoveRange(existingAuthorBooks);

            book.AuthorBooks = bookRequest.AuthorIds.Select(authorId => new AuthorBook 
            { 
                AuthorsId = authorId,
                BooksId = book.BookId
            }).ToList();

            var existingBookLanguages = _context.BookLanguage!.Where(bookLanguage => bookLanguage.BooksId == id);
            _context.BookLanguage!.RemoveRange(existingBookLanguages);

            book.BookLanguages = bookRequest.LanguageIds.Select(languageId => new BookLanguage
            {
                LanguagesId = languageId,
                BooksId = book.BookId
            }).ToList();

            var existingBookSubCategories = _context.BookSubCategory!.Where(bsc => bsc.BooksId == id);
            _context.BookSubCategory!.RemoveRange(existingBookSubCategories);

            book.BookSubCategories = bookRequest.SubCategoryIds.Select(subCategoryId => new BookSubCategory
            {
                SubCategoriesId = subCategoryId,
                BooksId = book.BookId
            }).ToList();

            try
            {
                await _context.SaveChangesAsync();
                return ServiceResult<bool>.SuccessResult(true);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Books.AnyAsync(book => book.BookId == id))
                {
                    return ServiceResult<bool>.FailureResult("Book not found");
                }
                else
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// Updates the status of an existing book.
        /// </summary>
        /// <param name="id">The ID of the book to update.</param>
        /// <param name="status">The new status to set.</param>
        /// <returns>A service result indicating success or failure.</returns>
        public async Task<ServiceResult<bool>> SetBookStatusAsync(long id, string status)
        {
            var book = await _context.Books!.FindAsync(id);
            if (book == null)
            {
                return ServiceResult<bool>.FailureResult("Book not found");
            }

            book.BookStatus = status;
            _context.Update(book).State = EntityState.Modified;

            var bookCopies = await _context.BookCopies!
                .Where(bc => bc.BookId == id)
                .ToListAsync();

            foreach (var bookCopy in bookCopies)
            {
                bookCopy.BookCopyStatus = status;
                _context.Update(bookCopy).State = EntityState.Modified;
            }

            try
            {
                await _context.SaveChangesAsync();
                return ServiceResult<bool>.SuccessResult(true);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Books.AnyAsync(e => e.BookId == id))
                {
                    return ServiceResult<bool>.FailureResult("Book not found");
                }
                else
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// Updates the image associated with an book.
        /// </summary>
        /// <param name="id">The ID of the book whose image is to be updated.</param>
        /// <param name="image">The new image file.</param>
        /// <returns>A service result indicating success or failure of the operation.</returns>
        public async Task<ServiceResult<bool>> UpdateBookImageAsync(long id, IFormFile coverImage)
        {
            var book = await _context.Books!.FindAsync(id);
            if (book == null)
            {
                return ServiceResult<bool>.FailureResult("Book not found");
            }

            var filePath = await _fileService.SaveFileAsync(coverImage, "BookImages");

            book.CoverFileName = filePath;
            _context.Update(book).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return ServiceResult<bool>.SuccessResult(true);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Books.AnyAsync(e => e.BookId == id))
                {
                    return ServiceResult<bool>.FailureResult("Book not found");
                }
                else
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// Retrieves the image of a book by book ID.
        /// </summary>
        /// <param name="bookId">The ID of the book.</param>
        /// <returns>The image as a byte array.</returns>
        public async Task<ServiceResult<byte[]>> GetBookImageAsync(long bookId)
        {
            try
            {
                var imageBytes = await _fileService.GetImageByBookIdAsync(bookId);
                return ServiceResult<byte[]>.SuccessResult(imageBytes);
            }
            catch (FileNotFoundException ex)
            {
                return ServiceResult<byte[]>.FailureResult(ex.Message);
            }
        }

        /// <summary>
        /// Update book's rating.
        /// </summary>
        /// <param name="id">The ID of the book to rate.</param>
        /// <returns>A service result indicating success or failure.</returns>
        public async Task<ServiceResult<bool>> UpdateBookRatingAsync(long id, float rating, string memberId)
        {
            if (rating < 0 || rating > 5)
            {
                return ServiceResult<bool>.FailureResult("Rating must be between 0 and 5!");
            }

            var book = await _context.Books!.FindAsync(id);
            if (book == null)
            {
                return ServiceResult<bool>.FailureResult("Book not found");
            }

            var existingRating = await _context.BookRatings!
                .FirstOrDefaultAsync(br => br.BookId == id && br.MemberId == memberId);

            if (existingRating != null)
            {
                existingRating.GivenRating = rating;
                _context.Update(existingRating).State = EntityState.Modified;
                return ServiceResult<bool>.SuccessMessageResult("The book' rating is updated successfully.");
            }
            else
            {
                var bookRating = new BookRating
                {
                    BookId = id,
                    MemberId = memberId,
                    GivenRating = rating
                };
                await _context.BookRatings!.AddAsync(bookRating);
            }

            try
            {
                await _context.SaveChangesAsync();

                // Recalculate the average rating for the book
                var ratings = await _context.BookRatings!.Where(br => br.BookId == id).ToListAsync();
                book.Rating = ratings.Average(br => br.GivenRating);
                _context.Update(book).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return ServiceResult<bool>.SuccessMessageResult("The rating is given successfully.");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Books!.AnyAsync(e => e.BookId == id))
                {
                    return ServiceResult<bool>.FailureResult("Book not found");
                }
                else
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// Updates the number of book copies available in the library.
        /// </summary>
        /// <param name="id">The ID of the book to update.</param>
        /// <param name="change">The number of copies to add or remove. Positive values add copies, negative values remove copies.</param>
        /// <returns>A service result indicating success or failure.</returns>
        public async Task<ServiceResult<bool>> UpdateBookCopiesAsync(long id, short change)
        {
            // Retrieve the book from the database
            var book = await _context.Books!.FindAsync(id);

            if (book == null)
            {
                return ServiceResult<bool>.FailureResult("Book not found");
            }

            // Count the number of active copies for the book
            var activeCopiesCount = await _context.BookCopies!
                .Where(bc => bc.BookId == id && bc.BookCopyStatus == Status.Active.ToString())
                .CountAsync();

            // Check if the change in number of copies is valid
            if (activeCopiesCount + change < 0)
            {
                return ServiceResult<bool>.FailureResult("Not enough copies available");
            }

            // Add new copies if the change is positive
            if (change > 0)
            {
                var newCopies = Enumerable.Range(0, change).Select(_ => new BookCopy
                {
                    BookId = id,
                    BookCopyStatus = Status.Active.ToString()
                });
                await _context.BookCopies!.AddRangeAsync(newCopies);
            }

            // Mark copies as inactive if the change is negative
            else if (change < 0)
            {
                var copiesToRemove = await _context.BookCopies!
                    .Where(bc => bc.BookId == id && bc.BookCopyStatus == Status.Active.ToString())
                    .Take(Math.Abs(change))
                    .ToListAsync();

                foreach (var copy in copiesToRemove)
                {
                    copy.BookCopyStatus = Status.InActive.ToString();
                    _context.Update(copy).State = EntityState.Modified;
                }
            }

            // Save changes to the database and handle potential concurrency exceptions
            try
            {
                await _context.SaveChangesAsync();
                return ServiceResult<bool>.SuccessResult(true);
            }
            catch (DbUpdateConcurrencyException)
            {
                // Check if the book still exists in case of concurrency issues
                if (!await _context.Books.AnyAsync(e => e.BookId == id))
                {
                    return ServiceResult<bool>.FailureResult("Book not found");
                }
                else
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// Helper method to manage cross-table relationships for books.
        /// </summary>
        /// <param name="bookRequest">The book request containing related data.</param>
        /// <param name="book">The book entity to update.</param>
        /// <returns>The updated book entity.</returns>
        private Book CrossTable(BookRequest bookRequest, Book newBook)
        {
            // Handle authors
            if (bookRequest.AuthorIds != null && bookRequest.AuthorIds.Any())
            {
                newBook.AuthorBooks = bookRequest.AuthorIds.Select(authorId => new AuthorBook
                {
                    AuthorsId = authorId,
                    BooksId = newBook.BookId
                }).ToList();
            }


            // Handle languages
            if (bookRequest.LanguageIds != null && bookRequest.LanguageIds.Any())
            {
                newBook.BookLanguages = bookRequest.LanguageIds.Select(languageId => new BookLanguage
                {
                    LanguagesId = languageId,
                    BooksId = newBook.BookId
                }).ToList();
            }

            // Handle subcategories
            if (bookRequest.SubCategoryIds != null && bookRequest.SubCategoryIds.Any())
            {
                newBook.BookSubCategories = bookRequest.SubCategoryIds.Select(subCategoryId => new BookSubCategory
                {
                    SubCategoriesId = subCategoryId,
                    BooksId = newBook.BookId
                }).ToList();
            }

            return newBook;
        }

        /// <summary>
        /// Retrieves books based on the specified filter predicate.
        /// </summary>
        /// <param name="filterPredicate">The filter predicate to apply.</param>
        /// <returns>A list of books matching the filter.</returns>
        private async Task<IEnumerable<BookResponse>> GetBooksAsync(Expression<Func<Book, bool>> filterPredicate)
        {
            var books = await _context.Books!
                .Where(filterPredicate)
                .Include(book => book.Publisher)
                .Include(book => book.Location)
                .Include(book => book.BookSubCategories)!
                    .ThenInclude(bookSubCategory => bookSubCategory.SubCategory)
                .Include(book => book.AuthorBooks)!
                    .ThenInclude(authorBook => authorBook.Author)
                .Include(book => book.BookLanguages)!
                    .ThenInclude(bookLanguage => bookLanguage.Language)
                .Include(book => book.BookCopies)
                .Select(b => new BookResponse
                {
                    BookId = b.BookId,
                    ISBN = b.ISBN,
                    Title = b.Title,
                    PageCount = b.PageCount,
                    PublishingYear = b.PublishingYear,
                    Description = b.Description,
                    PrintCount = b.PrintCount,
                    CopyCount = (short)b.BookCopies!.Where(bc => bc.BookCopyStatus == Status.Active.ToString()).ToList().Count,
                    BookStatus = b.BookStatus,
                    Rating = b.Rating,
                    Location = b.Location!.ShelfNumber + "/ " + b.Location.AisleCode + "/ " + b.Location.SectionCode,
                    PublisherName = b.Publisher!.Name,
                    SubCategoryNames = b.BookSubCategories!.Select(sb => sb.SubCategory!.Name).ToList(),
                    CategoryName = b.BookSubCategories!.Select(sb => sb.SubCategory!.Category!.Name).First(),
                    LanguageNames = b.BookLanguages!.Select(bl => bl.Language!.Name).ToList(),
                    AuthorNames = b.AuthorBooks!.Select(ab => ab.Author!.FullName).ToList(),
                })
                .ToListAsync();

            return books;
        }

        /// <summary>
        /// Retrieves books based on the specified filter predicate.
        /// </summary>
        /// <param name="filterPredicate">The filter predicate to apply.</param>
        /// <returns>A list of books matching the filter.</returns>
        private async Task<IEnumerable<BookCopyResponse>> GetBookCopiesAsync(Expression<Func<BookCopy, bool>> filterPredicate)
        {
            var bookCopies = await _context.BookCopies!
                .Where(filterPredicate)
                .Include(b => b.Book)
                .Select(b => new BookCopyResponse
                {
                    BookId = b.BookId,
                    ISBN = b.Book!.ISBN,
                    Title = b.Book.Title,
                    BookCopyId = b.BookCopyId,
                    BookCopyStatus = b.BookCopyStatus
                })
                .ToListAsync();

            return bookCopies;
        }
    }
}
