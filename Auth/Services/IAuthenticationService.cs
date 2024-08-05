using LibraryAPI.Exceptions;
using LibraryAPI.Models.DTOs.Request;

namespace LibraryAPI.Auth.Services
{
    public interface IAuthenticationService
    {
        Task<ServiceResult<string>> ForgotPasswordAsync(ForgotPasswordRequest forgotPasswordRequest);
        Task<ServiceResult<string>> ResetPasswordAsync(ResetPasswordRequest resetPasswordRequest);
    }
}
