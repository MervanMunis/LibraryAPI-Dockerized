using LibraryAPI.Exceptions;
using LibraryAPI.Models.DTOs.Request;
using LibraryAPI.Models.DTOs.Response;

namespace LibraryAPI.Services.Interfaces
{
    public interface IBookService
    {
        Task<ServiceResult<IEnumerable<BookResponse>>> GetAllBooksAsync();

        Task<ServiceResult<IEnumerable<BookResponse>>> GetAllActiveBooksAsync();

        Task<ServiceResult<IEnumerable<BookResponse>>> GetAllInActiveBooksAsync();

        Task<ServiceResult<IEnumerable<BookResponse>>> GetAllBannedBooksAsync();

        Task<ServiceResult<IEnumerable<BookCopyResponse>>> GetAllActiveBookCopiesAsync();

        Task<ServiceResult<IEnumerable<BookCopyResponse>>> GetAllInActiveBookCopiesAsync();

        Task<ServiceResult<IEnumerable<BookCopyResponse>>> GetAllBorrowedBookCopiesAsync();

        Task<ServiceResult<BookResponse>> GetBookByIdAsync(long id);

        Task<ServiceResult<string>> AddBookAsync(BookRequest bookRequest);

        Task<ServiceResult<bool>> UpdateBookAsync(long id, BookRequest bookRequest);

        Task<ServiceResult<bool>> SetBookStatusAsync(long id, string status);

        Task<ServiceResult<bool>> UpdateBookImageAsync(long id, IFormFile coverImage);

        Task<ServiceResult<byte[]>> GetBookImageAsync(long bookId);

        Task<ServiceResult<bool>> UpdateBookRatingAsync(long id, float rating, string memberId);

        Task<ServiceResult<bool>> UpdateBookCopiesAsync(long id, short change);
    }
}
