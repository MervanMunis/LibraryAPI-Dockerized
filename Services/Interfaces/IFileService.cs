using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace LibraryAPI.Services.Interfaces
{
    public interface IFileService
    {
        Task<string> SaveFileAsync(IFormFile file, string folderName);
        Task<bool> DeleteFileAsync(string filePath);
        Task<byte[]> GetImageByAuthorIdAsync(long authorId);
        Task<byte[]> GetImageByBookIdAsync(long bookId);
    }
}
