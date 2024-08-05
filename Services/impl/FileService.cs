using LibraryAPI.Data;
using LibraryAPI.Services.Interfaces;

namespace LibraryAPI.Services.impl
{
    public class FileService : IFileService
    {
        private readonly string _baseDirectory;
        private readonly LibraryAPIContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileService"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        public FileService(LibraryAPIContext context)
        {
            _context = context;
            _baseDirectory = Path.Combine(Directory.GetCurrentDirectory(), "LibraryAPI");
        }

        /// <summary>
        /// Saves a file asynchronously to a specified folder.
        /// </summary>
        /// <param name="file">The file to be saved.</param>
        /// <param name="folderName">The folder name where the file will be saved.</param>
        /// <returns>The file name of the saved file.</returns>
        public async Task<string> SaveFileAsync(IFormFile file, string folderName)
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("File is invalid");
            }

            // Create the folder if it doesn't exist
            var folderPath = Path.Combine(_baseDirectory, folderName);
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            // Generate a unique file name
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(folderPath, fileName);

            // Save the file to the specified path
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
                stream.Flush();
            }

            return fileName;
        }

        /// <summary>
        /// Deletes a file asynchronously.
        /// </summary>
        /// <param name="filePath">The path of the file to be deleted.</param>
        /// <returns>True if the file is deleted successfully; otherwise, false.</returns>
        public Task<bool> DeleteFileAsync(string filePath)
        {
            var fullPath = Path.Combine(_baseDirectory, filePath);

            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
                return Task.FromResult(true);
            }

            return Task.FromResult(false);
        }

        /// <summary>
        /// Retrieves the image of an author by author ID.
        /// </summary>
        /// <param name="authorId">The ID of the author.</param>
        /// <returns>The image as a byte array.</returns>
        /// <exception cref="FileNotFoundException">Thrown when the image is not found.</exception>
        public async Task<byte[]> GetImageByAuthorIdAsync(long authorId)
        {
            var author = await _context.Authors!.FindAsync(authorId);
            if (author == null || string.IsNullOrEmpty(author.ImageFileName))
            {
                throw new FileNotFoundException("Image not found for the specified author.");
            }

            var filePath = Path.Combine(_baseDirectory, "AuthorImages", author.ImageFileName);
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("Image not found for the specified author.");
            }

            return await File.ReadAllBytesAsync(filePath);
        }

        /// <summary>
        /// Retrieves the image of a book by book ID.
        /// </summary>
        /// <param name="bookId">The ID of the book.</param>
        /// <returns>The image as a byte array.</returns>
        public async Task<byte[]> GetImageByBookIdAsync(long bookId)
        {
            var book = await _context.Books!.FindAsync(bookId);
            if (book == null || string.IsNullOrEmpty(book.CoverFileName))
            {
                throw new FileNotFoundException("Image not found for the specified book.");
            }

            var filePath = Path.Combine(_baseDirectory, "BookImages", book.CoverFileName);
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("Image not found for the specified book.");
            }

            return await File.ReadAllBytesAsync(filePath);
        }
    }
}
